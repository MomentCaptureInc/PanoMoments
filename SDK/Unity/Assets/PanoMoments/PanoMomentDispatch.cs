/* 
*   PanoMoments
*   Copyright (c) 2019 Moment Capture Inc.
*/


namespace PanoMoments.Core.Internal {

    using AOT;
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Threading;

    public static class Dispatcher {

        #region --Client API--
        public enum Target {
            MainThread = 0, RenderThread
        }

        public static void Dispatch (Target target, Action workload) {
            switch (target) {
                case Target.MainThread:
                    lock ((mainQueue as ICollection).SyncRoot)
                        mainQueue.Enqueue(workload);
                    break;
                case Target.RenderThread:
                    GL.IssuePluginEvent(renderDelegateHandle, ((IntPtr)GCHandle.Alloc(workload, GCHandleType.Normal)).ToInt32());
                    break;
            }
        }

        public static event Action onFrame;
        #endregion


        #region --Operations--

        private static readonly IntPtr renderDelegateHandle;
        private static readonly Queue<Action> mainQueue;

        static Dispatcher () {
            // Setup main dispatch
            mainQueue = new Queue<Action>();
            Camera.onPostRender += DequeueMain;
            // Setup render dispatch
            renderDelegateHandle = Marshal.GetFunctionPointerForDelegate((UnityRenderingEvent)DequeueRender);
        }

        private static void DequeueMain (Camera _) {
            for (;;)
                lock ((mainQueue as ICollection).SyncRoot)
                    if (mainQueue.Count > 0)
                        mainQueue.Dequeue()();
                    else
                        break;
            if (onFrame != null)
                onFrame();
        }

        [MonoPInvokeCallback(typeof(UnityRenderingEvent))]
        private static void DequeueRender (int context) {
            GCHandle handle = (GCHandle)(IntPtr)context;
            Action target = handle.Target as Action;
            handle.Free();
            target();
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void UnityRenderingEvent (int context);
        #endregion
    }
}