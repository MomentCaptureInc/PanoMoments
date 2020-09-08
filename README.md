## Introduction
This repo contains an [SDK](https://github.com/MomentCaptureInc/PanoMoments/tree/master/SDK) and [documentation](https://github.com/MomentCaptureInc/PanoMoments/wiki) for creating and viewing PanoMoments. If you have any questions or comments, feel free to reach out to info@panomoments.com and make sure to check out https://www.panomoments.com for more info about personal creative use and commercial licensing.

## PanoMoments SDK
The PanoMoments SDK provides core download and rendering functionality for viewing PanoMoments. To get started, [create an account](https://my.panomoments.com/me) and then clone this repo. There are four supported platforms: Unity, Android, iOS, and Web. Below is a list of parameters required across all platforms:

- `string moment_id` of the PanoMoment you want to render.
- `int variation` set to `100` for SD, `101` for HD, and `102` for UHD
- `string public_api_key` Located on https://my.panomoments.com/me/edit (web only requires public key)
- `string private_api_key`Located on https://my.panomoments.com/me/edit (native rendering requires both public and private keys)

### Unity (iOS and Android only)
We've created a bridge interface that makes things a bit easier to get started in Unity. Note you'll need to add both the `PanoMoments.framework` and `PanoMomentsUnity.framework` to the XCode Embedded Binaries section after building in Unity. Below is the basic code to display a PanoMoment. Note that the `material` you're rendering to must be passed into the constructor:

```
Identifier identifier = new Identifier(public_api_key, private_api_key, moment_id, Quality.SD);

PanoMoment panoMoment = new PanoMoment(
	identifier,
	material,
	frameCallback,
	readyCallback,
	loadedCallback,
	errorCallback
);

void errorCallback (string error) {}

void frameCallback (int renderedFrame, Texture texure) {
	material.mainTexture = texure;
}

void loadedCallback () {}

void readyCallback (Resolution resolution) {}
```

And then once either the `loadedCallback` is called or `panoMoment.FrameCount > 0` call `panoMoment.Render(int frame)`:

```
private int frameIndex = 0;
void Update () {
	if (panoMoment.FrameCount > 0)
		panoMoment.Render(frameIndex = (frameIndex + 1) % panoMoment.FrameCount);
}
```

To dispose of the current PanoMoment instance:

```
panoMoment.Dispose();
panoMoment = null;
```

### Android
*Documentation coming soon*

### iOS
*Documentation coming soon*

### Web
```
var videoInDOM = false;
var index = 0;

var identifier = {
  moment_id: "",
  variation: 100,
  public_api_key: ""
};

var panoMoment = new PanoMoments(identifier, renderCallback, readyCallback, loadedCallback);

function animate() {
	index = (index + 1) % panoMoment.FrameCount;
	panoMoment.render(index);
	window.requestAnimationFrame(animate);
}

function renderCallback (video, momentData) {
	if (!videoInDOM) {
		videoInDOM = true;
		document.body.appendChild(video);
	}
}

function readyCallback (video, momentData) {
	index = panoMoment.currentIndex;
	console.log("PanoMoment Ready for Rendering");
}

function loadedCallback (video, momentData) {
	console.log("PanoMoment Download Complete");
}
```

### Known Native (iOS/Android/Unity) Issues
1. Calling dispose before the `loadedCallback` will crash the SDK