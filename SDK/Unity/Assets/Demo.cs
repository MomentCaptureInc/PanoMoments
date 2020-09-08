/* 
*   PanoMoments
*   Copyright (c) 2019 Moment Capture Inc.
*/

namespace PanoMoments.Demos {

    using UnityEngine;
    using UnityEngine.UI;
    using Core;

    public class Demo : MonoBehaviour {

        public RawImage rawImage;
        public AspectRatioFitter aspectFitter;
        
		private PanoMoment panoMoment;
        private int frameIndex = 0;

        void Awake () {
            Application.targetFrameRate = 60;
        }

        void Start () {
			// Note that the current provided public_api_key and moment_id are special demo values. Replace these (and private_api_key) with your own.
			Identifier identifier = new Identifier("63b130be-2f20-40a2-8211-355b607f340a", "PRIVATE_API_KEY", "5c95d9843c344e001f1588f6", Quality.SD);
			panoMoment = new PanoMoment(
				identifier,
				rawImage.material,
				frameCallback,
				readyCallback,
				loadedCallback,
				errorCallback
			);
        }

		void Update () {
			if (panoMoment.FrameCount == 0)
				return;
			switch (Input.touchCount) {
			case 1:
				frameIndex = (frameIndex + 1) % panoMoment.FrameCount;
				panoMoment.Render(frameIndex);
				break;
			case 2:
				frameIndex = (frameIndex - 1) % panoMoment.FrameCount;
				frameIndex = frameIndex < 0 ? frameIndex + panoMoment.FrameCount : frameIndex;
				panoMoment.Render(frameIndex);
				break;
			}
		}

		void errorCallback (string error) {}

		void frameCallback (int renderedFrame, Texture texure) {
			rawImage.texture = texure;
		}

		void loadedCallback () {
			panoMoment.Render(frameIndex);
			Debug.Log("PanoMoment loaded with " + panoMoment.FrameCount + " frames");
		}

		void readyCallback (Resolution resolution) {
			aspectFitter.aspectRatio = (float)resolution.width / resolution.height;
		}

    }
}