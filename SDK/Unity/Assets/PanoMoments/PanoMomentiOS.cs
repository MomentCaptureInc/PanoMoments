/* 
*   PanoMoments
*   Copyright (c) 2019 Moment Capture Inc.
*/

namespace PanoMoments.Core.Internal {

    using AOT;
    using UnityEngine;
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    public sealed class PanoMomentiOS : IPanoMoment {

        #region --Metadata--

        public int FrameCount {
            get { return panomoment.FrameCount(); }
        }

        #endregion


        #region --PanoMoment--

        public PanoMomentiOS (Identifier identifier, Action<int, Texture> renderCallback, Action<Resolution> readyCallback = null, Action loadedCallback = null, Action<string> errorHandler = null) {
            this.renderCallback = renderCallback;
            this.readyCallback = readyCallback;
            this.loadedCallback = loadedCallback;
            this.errorHandler = errorHandler;
            this.self = GCHandle.Alloc(this, GCHandleType.Normal);
			this.panomoment = PanoMomentBridge.Create(identifier.public_api_key, identifier.private_api_key, identifier.momentID, identifier.quality, OnFrame, OnReady, OnLoaded, OnError, (IntPtr)self);
        }

        public void Dispose () {
            panomoment.Dispose();
            self.Free();
        }

        public void Render (int frame) {
            panomoment.Render(frame);
        }

        T IPanoMoment.GetMetadata<T> (string name) {
            object result = null;
            if (typeof(T) == typeof(string)) {
                var stringBuilder = new StringBuilder(1024);
                panomoment.GetStringMetadata(name, stringBuilder);
                result = stringBuilder.ToString();
            }
            if (typeof(T) == typeof(int))
                result = panomoment.GetIntMetadata(name);
            if (typeof(T) == typeof(float))
                result = panomoment.GetFloatMetadata(name);
            if (typeof(T) == typeof(bool))
                result = panomoment.GetBoolMetadata(name);
            return (T)Convert.ChangeType(result, typeof(T));
        }
        #endregion


        #region --Operations--

        private readonly Action<int, Texture> renderCallback;
        private readonly Action<Resolution> readyCallback;
        private readonly Action loadedCallback;
        private readonly Action<string> errorHandler;
        private readonly GCHandle self;
        private readonly IntPtr panomoment;
        private Texture2D texture;

        [MonoPInvokeCallback(typeof(PanoMomentBridge.RenderCallback))]
        private static void OnFrame (IntPtr context, int frameIndex, IntPtr mtlTexture) {
            var handle = (GCHandle)context;
            var instance = handle.Target as PanoMomentiOS;
            instance.texture.UpdateExternalTexture(mtlTexture);
            instance.renderCallback(frameIndex, instance.texture);
        }

        [MonoPInvokeCallback(typeof(PanoMomentBridge.ReadyCallback))]
        private static void OnReady (IntPtr context, int width, int height) {
            var handle = (GCHandle)context;
            var instance = handle.Target as PanoMomentiOS;
            instance.texture = new Texture2D(width, height, TextureFormat.BGRA32, false, false);
            instance.texture.wrapMode = TextureWrapMode.Clamp;
            if (instance.readyCallback != null)
                instance.readyCallback(new Resolution { width = width, height = height });
        }

        [MonoPInvokeCallback(typeof(PanoMomentBridge.LoadedCallback))]
        private static void OnLoaded (IntPtr context) {
            var handle = (GCHandle)context;
            var instance = handle.Target as PanoMomentiOS;
            if (instance.loadedCallback != null)
                instance.loadedCallback();
        }

        [MonoPInvokeCallback(typeof(PanoMomentBridge.ErrorHandler))]
        private static void OnError (IntPtr context, IntPtr errorStr) {
            var handle = (GCHandle)context;
            var instance = handle.Target as PanoMomentiOS;
            if (instance.errorHandler != null)
                instance.errorHandler(Marshal.PtrToStringAuto(errorStr));
        }
        #endregion
    }
}