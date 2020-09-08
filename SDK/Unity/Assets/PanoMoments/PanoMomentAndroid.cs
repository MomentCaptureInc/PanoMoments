/* 
*   PanoMoments
*   Copyright (c) 2019 Moment Capture Inc.
*/

namespace PanoMoments.Core.Internal {

    using UnityEngine;
    using UnityEngine.Scripting;
    using System;

    public sealed class PanoMomentAndroid : IPanoMoment {

        #region --Metadata--

        public int FrameCount {
            get { return panomoment != null ? panomoment.Call<int>("frameCount") : 0; }
        }

        #endregion


        #region --PanoMoment--

		public PanoMomentAndroid (Identifier identifier, Material renderMaterial, Action<int, Texture> renderCallback, Action<Resolution> readyCallback = null, Action loadedCallback = null, Action<string> errorHandler = null) {
            GLBlitEncoderClass = GLBlitEncoderClass ?? new AndroidJavaClass(@"com.momentcapture.panomoments.core.PanoMoment");
            // Prepare
            Dispatcher.Dispatch(Dispatcher.Target.RenderThread, () => {
                AndroidJNI.AttachCurrentThread();
                var textureID = GLBlitEncoderClass.CallStatic<int>(@"getExternalTexture");
                Dispatcher.Dispatch(0, () => {
					this.frameHandler = new FrameHandler((IntPtr)textureID, renderMaterial, renderCallback, readyCallback, loadedCallback, errorHandler);
					using (var jIdentifier = new AndroidJavaObject(@"com.momentcapture.panomoments.core.PanoMoment$Identifier", identifier.public_api_key, identifier.private_api_key, identifier.momentID, (int)identifier.quality))
                        this.panomoment = new AndroidJavaObject(@"com.momentcapture.panomoments.core.PanoMoment", jIdentifier, frameHandler.surface, frameHandler);
                });
            });
        }

        public void Dispose () {
            Dispatcher.Dispatch(Dispatcher.Target.RenderThread, () => {
                Dispatcher.Dispatch(0, () => {
                    panomoment.Call(@"release");
                    panomoment.Dispose();
                    frameHandler.Dispose();
                });
            });
        }

        public void Render (int frame) {
            panomoment.Call(@"render", frame);
        }

        T IPanoMoment.GetMetadata<T> (string name) {
            if (panomoment == null)
                return default(T);
            object result = null;
            using (var metadata = panomoment.Call<AndroidJavaObject>("metadata")) {
                if (typeof(T) == typeof(string))
                    result = metadata.Call<string>("optString", name);
                if (typeof(T) == typeof(int))
                    result = metadata.Call<int>("optInt", name);
                if (typeof(T) == typeof(float))
                    result = (float)metadata.Call<double>("optDouble", name);
                if (typeof(T) == typeof(bool))
                    result = metadata.Call<bool>("optBoolean", name);
            }
            return (T)Convert.ChangeType(result, typeof(T));
        }
        #endregion


        #region --Operations--
        
        private AndroidJavaObject panomoment;
        private FrameHandler frameHandler;
        private static AndroidJavaClass GLBlitEncoderClass;

        private sealed class FrameHandler : AndroidJavaProxy, IDisposable {
            
            public readonly AndroidJavaObject surface;
            private readonly AndroidJavaObject surfaceTexture;
            private readonly IntPtr texturePtr;
            private readonly Action<int, Texture> frameHandler;
            private readonly Action<Resolution> readyDelegate;
            private readonly Action loadedDelegate;
            private readonly Action<string> errorHandler;
            private Texture2D nativeTexture;
            private readonly IntPtr transformMatrix;
            private readonly IntPtr getTransformMatrixMethod;
			private readonly Material renderMaterial;
			private bool matrixSet;
			private Matrix4x4 matrix;

            public FrameHandler (IntPtr texturePtr, Material renderMaterial, Action<int, Texture> frameHandler, Action<Resolution> readyDelegate, Action loadedDelegate, Action<string> errorHandler) : base(@"com.momentcapture.panomoments.core.PanoMoment$Callback") {
                this.texturePtr = texturePtr;
                this.surfaceTexture = new AndroidJavaObject(@"android.graphics.SurfaceTexture", texturePtr.ToInt32());
                this.surface = new AndroidJavaObject(@"android.view.Surface", surfaceTexture);
                this.frameHandler = frameHandler;
                this.readyDelegate = readyDelegate;
                this.loadedDelegate = loadedDelegate;
                this.errorHandler = errorHandler;
                this.getTransformMatrixMethod = AndroidJNI.GetMethodID(surfaceTexture.GetRawClass(), "getTransformMatrix", "([F)V");
                this.renderMaterial = renderMaterial;
				this.matrixSet = false;
				this.matrix = new Matrix4x4 ();
				renderMaterial.SetMatrix("_SampleTransform", Matrix4x4.identity);
                var localTransformMatrix = AndroidJNI.NewFloatArray(16);
                this.transformMatrix = AndroidJNI.NewGlobalRef(localTransformMatrix);
                AndroidJNI.DeleteLocalRef(localTransformMatrix);
                Dispatcher.onFrame += OnFrame;
            }

            public void Dispose () {
                Dispatcher.onFrame -= OnFrame;
                surfaceTexture.Call(@"release");
                surface.Call(@"release");
                surfaceTexture.Dispose();
                surface.Dispose();
                Texture2D.Destroy(nativeTexture);
                AndroidJNI.DeleteGlobalRef(transformMatrix);
                Dispatcher.Dispatch(
                    Dispatcher.Target.RenderThread,
                    () => GLBlitEncoderClass.CallStatic(@"releaseTexture", texturePtr.ToInt32())
                );
            }

            [Preserve]
            private void onFrame (int frameIndex) {
                frameHandler(frameIndex, nativeTexture);
				if (!matrixSet) {
	                Dispatcher.Dispatch(
	                    Dispatcher.Target.RenderThread,
						() => {
							surfaceTexture.Call(@"updateTexImage");
							// Update UV transform
							var args = new jvalue[1];
							args [0].l = transformMatrix;
							AndroidJNI.CallVoidMethod (surfaceTexture.GetRawObject (), getTransformMatrixMethod, args);
							// Inject into render material
							var transform = AndroidJNIHelper.ConvertFromJNIArray<float[]> (transformMatrix);
							for (var j = 0; j < 4; j++)
								for (var i = 0; i < 4; i++)
									matrix [i, j] = transform [i + 4 * j];
							renderMaterial.SetMatrix("_SampleTransform", matrix);
							matrixSet = true;
						}
	                );

				}
            }
            
            [Preserve]
            private void onReady (int width, int height) {
                this.nativeTexture = Texture2D.CreateExternalTexture(width, height, TextureFormat.RGBA32, false, false, texturePtr);
                this.nativeTexture.wrapMode = TextureWrapMode.Clamp;
                if (readyDelegate != null)
                    readyDelegate(new Resolution { width = width, height = height });
            }

            [Preserve]
            private void onLoaded () {
                if (loadedDelegate != null)
                    loadedDelegate();
            }

            [Preserve]
            private void onError (string error) {
                if (errorHandler != null)
                    errorHandler(error);
            }

            private void OnFrame () {
				// Update preview
				Dispatcher.Dispatch (
					Dispatcher.Target.RenderThread,
					() => {
						surfaceTexture.Call (@"updateTexImage");
						renderMaterial.SetMatrix ("_SampleTransform", matrix);
					}
				);
            }

        }
        #endregion
    }
}