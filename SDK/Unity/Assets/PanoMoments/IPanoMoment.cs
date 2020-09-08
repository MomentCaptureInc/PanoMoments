/* 
*   PanoMoments
*   Copyright (c) 2019 Moment Capture Inc.
*/

namespace PanoMoments.Core {

    using UnityEngine;
    using System;

    public interface IPanoMoment : IDisposable {
        int FrameCount { get; }
        void Render (int frame);
        T GetMetadata<T> (string name) where T : IConvertible;
    }
}