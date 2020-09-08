/* 
*   PanoMoments
*   Copyright (c) 2019 Moment Capture Inc.
*/


namespace PanoMoments.Core.Internal {
    
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    public static class PanoMomentBridge {

        private const string Assembly =
        #if UNITY_IOS && !UNITY_EDITOR
        @"__Internal";
        #else
        @"PanoMoments";
        #endif

        public delegate void RenderCallback (IntPtr context, int frameIndex, IntPtr texPtr);
        public delegate void ReadyCallback (IntPtr context, int width, int height);
        public delegate void LoadedCallback (IntPtr context);
        public delegate void ErrorHandler (IntPtr context, IntPtr errorString);

        #if UNITY_IOS && !UNITY_EDITOR
        [DllImport(Assembly, EntryPoint = @"PMCreate")]
		public static extern IntPtr Create (string publicApiKey, string privateApiKey, string momentID, Quality quality, RenderCallback renderCallback, ReadyCallback readyCallback, LoadedCallback loadedCallback, ErrorHandler errorHandler, IntPtr context);
        [DllImport(Assembly, EntryPoint = @"PMDispose")]
        public static extern void Dispose (this IntPtr panomoment);
        [DllImport(Assembly, EntryPoint = @"PMRender")]
        public static extern void Render (this IntPtr panomoment, int frame);
        [DllImport(Assembly, EntryPoint = @"PMFrameCount")]
        public static extern int FrameCount (this IntPtr panomoment);
        [DllImport(Assembly, EntryPoint = @"PMGetStringMetadata")]
        public static extern void GetStringMetadata (this IntPtr panomoment, string name, StringBuilder result);
        [DllImport(Assembly, EntryPoint = @"PMGetFloatMetadata")]
        public static extern float GetFloatMetadata (this IntPtr panomoment, string name);
        [DllImport(Assembly, EntryPoint = @"PMGetIntMetadata")]
        public static extern int GetIntMetadata (this IntPtr panomoment, string name);
        [DllImport(Assembly, EntryPoint = @"PMGetBoolMetadata")]
        public static extern bool GetBoolMetadata (this IntPtr panomoment, string name);

        #else
		public static IntPtr Create (string publicApiKey, string privateApiKey, string momentID, Quality quality, RenderCallback renderCallback, ReadyCallback readyCallback, LoadedCallback loadedCallback, ErrorHandler errorHandler, IntPtr context) { return IntPtr.Zero; }
        public static void Dispose (this IntPtr panomoment) {}
        public static void Render (this IntPtr panomoment, int frame) {}
        public static int FrameCount (this IntPtr panomoment) { return 0; }
        public static void GetStringMetadata (this IntPtr panomoment, string name, StringBuilder result) {}
        public static float GetFloatMetadata (this IntPtr panomoment, string name) { return 0; }
        public static int GetIntMetadata (this IntPtr panomoment, string name) { return 0; }
        public static bool GetBoolMetadata (this IntPtr panomoment, string name) { return false; }
        #endif
    }
}