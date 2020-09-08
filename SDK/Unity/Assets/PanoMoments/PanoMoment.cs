/* 
*   PanoMoments
*   Copyright (c) 2019 Moment Capture Inc.
*/

namespace PanoMoments.Core {
    
    using UnityEngine;
    using System;
    using Internal;

    public sealed class PanoMoment : IPanoMoment {

        #region --Metadata--
        public string ID {
            get { return implementation.GetMetadata<string>("id"); }
        }
        public string Title {
            get { return implementation.GetMetadata<string>("title"); }
        }
        public float StartFrame {
            get { return implementation.GetMetadata<float>("start_frame"); }
        }
        public float MinHorizontalFOV {
            get { return implementation.GetMetadata<float>("min_horizontal_fov"); }
        }
        public float MinVerticalFOV {
            get { return implementation.GetMetadata<float>("min_vertical_fov"); }
        }
        public float MaxHorizontalFOV {
            get { return implementation.GetMetadata<float>("max_horizontal_fov"); }
        }
        public float MaxVerticalFOV {
            get { return implementation.GetMetadata<float>("max_vertical_fov"); }
        }
        public bool Processed {
            get { return implementation.GetMetadata<bool>("processed"); }
        }
        public bool AllowStreaming {
            get { return implementation.GetMetadata<bool>("allow_streaming"); }
        }
        public int MomentType {
            get { return implementation.GetMetadata<int>("moment_type"); }
        }
        public int Rotation {
            get { return implementation.GetMetadata<int>("rotation"); }
        }
        public int RotationLimit {
            get { return implementation.GetMetadata<int>("rotation_limit"); }
        }
        public bool Draft {
            get { return implementation.GetMetadata<bool>("draft"); }
        }
        public bool Embeddable {
            get { return implementation.GetMetadata<bool>("embeddable"); }
        }
        public string Username {
            get { return implementation.GetMetadata<string>("username"); }
        }
        public string Slug {
            get { return implementation.GetMetadata<string>("slug"); }
        }
        public string Description {
            get { return implementation.GetMetadata<string>("description"); }
        }
        public string Location {
            get { return implementation.GetMetadata<string>("location"); }
        }
        public bool Clockwise {
            get { return implementation.GetMetadata<bool>("clockwise"); }
        }
        public bool ContainsParallax {
            get { return implementation.GetMetadata<bool>("contains_parallax"); }
        }
        public bool EmbedDelayedLoad {
            get { return implementation.GetMetadata<bool>("embed_delayed_load"); }
        }
        public bool Aligned {
            get { return implementation.GetMetadata<bool>("aligned"); }
        }
        public float AspectRatio {
            get { return implementation.GetMetadata<float>("aspect_ratio"); }
        }
        public float TrimStart {
            get { return implementation.GetMetadata<float>("trim_start"); }
        }
        public float TrimEnd {
            get { return implementation.GetMetadata<float>("trim_end"); }
        }
        public string Source {
            get { return implementation.GetMetadata<string>("source"); }
        }
        public bool Fade {
            get { return implementation.GetMetadata<bool>("fade"); }
        }
        public float SpeedModifier {
            get { return implementation.GetMetadata<float>("speed_modifier"); }
        }
        public bool ForceHover {
            get { return implementation.GetMetadata<bool>("force_hover"); }
        }
        public bool H265Processed {
            get { return implementation.GetMetadata<bool>("h265_processed"); }
        }
        public int TotalFrameCount {
            get { return implementation.GetMetadata<int>("num_frames"); }
        }
        public bool Hidden {
            get { return implementation.GetMetadata<bool>("hidden"); }
        }
        #endregion


        #region --Properties--
        /// <summary>
        /// Number of frames in the PanoMoment
        /// </summary>
        public int FrameCount {
            get { return implementation.FrameCount; }
        }
        #endregion


        #region --Client API--
        /// <summary>
        /// Create a PanoMoment
        /// </summary>
        /// <param name="identifier">PanoMoment identifier</param>
        /// <param name="renderCallback">Callback invoked with rendered frames</param>
        /// <param name="readyCallback">Optional. Callback invoked when PanoMoment is ready for rendering</param>
        /// <param name="loadedCallback">Optional. Callback invoked when PanoMoment has finished loading</param>
        /// <param name="errorHandler">Optional. Handler invoked if PanoMoment encounters an error</param>
		public PanoMoment (Identifier identifier, Material renderMaterial, Action<int, Texture> renderCallback, Action<Resolution> readyCallback = null, Action loadedCallback = null, Action<string> errorHandler = null) {
            switch (Application.platform) {
                case RuntimePlatform.Android:
					implementation = new PanoMomentAndroid(identifier, renderMaterial, renderCallback, readyCallback, loadedCallback, errorHandler);
                    break;
                case RuntimePlatform.IPhonePlayer:
                    implementation = new PanoMomentiOS(identifier, renderCallback, readyCallback, loadedCallback, errorHandler);
                    break;
                default:
                    Debug.LogError("PanoMoment Error: Cannot fetch on this platform");
                    implementation = null;
                    break;
            }
        }

        /// <summary>
        /// Dispose the PanoMoment and teardown resources
        /// </summary>
        public void Dispose () {
            implementation.Dispose();
        }

        /// <summary>
        /// Render a frame from the PanoMoment
        /// </summary>
        /// <param name="frame">Frame index</param>
        public void Render (int frame) {
            implementation.Render(frame);
        }
        #endregion


        #region --Operations--

        private readonly IPanoMoment implementation;

        T IPanoMoment.GetMetadata<T> (string name) {
            return default(T);
        }

        public static implicit operator bool (PanoMoment moment) {
            return moment != null;
        }
        #endregion
    }
}