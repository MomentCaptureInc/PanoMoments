using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.IO;

public class CameraRotation : MonoBehaviour {

	public float totalFrames;
	public bool enableCapture;

	public RenderTexture cubemap;
	public RenderTexture equirectangular;

	private bool captureComplete = false;
	private float currentAngle = 0;
	private int frameCount = 0;

	void Update () {
		transform.RotateAround (Vector3.zero, Vector3.up, 360 / totalFrames);
	}
		
	void LateUpdate() {

		if (enableCapture && !captureComplete && currentAngle < 360) {

			Camera cam = GetComponent<Camera>();

			if (cam == null)
			{
				cam = GetComponentInParent<Camera>();
			}

			if (cam == null)
			{
				Debug.Log("capture node has no camera or parent camera");
			}

			cam.RenderToCubemap(cubemap, 63, Camera.MonoOrStereoscopicEye.Mono);
			cubemap.ConvertToEquirect(equirectangular,Camera.MonoOrStereoscopicEye.Mono);

			string fmt = "0000";
			DumpRenderTexture (equirectangular, Application.dataPath + "/../Recordings/PanoMoment" + frameCount.ToString(fmt) + ".png");
			Debug.Log ("frameCount: " + frameCount);

			currentAngle = currentAngle + 360 / totalFrames;
			frameCount++;

		} else {

			captureComplete = true;
			return;

		}

	}

	public static void DumpRenderTexture(RenderTexture rt, string pngOutPath)
	{
		var oldRT = RenderTexture.active;

		Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);

		RenderTexture.active = rt;
		tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
		tex.Apply();

		byte[] bytes = tex.EncodeToPNG ();
		Texture2D.DestroyImmediate(tex, true);

		File.WriteAllBytes(pngOutPath, bytes);
		RenderTexture.active = oldRT;
	}
}




