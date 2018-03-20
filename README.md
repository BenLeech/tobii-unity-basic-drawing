# Tobii Unity Basic Drawing
Basic eye gaze drawing application in Unity using the Tobii SDK and Framework.

## Table of Contents
1. [Documentation and References](#documentation-and-references)
2. [Setting up the 4c](#setting-up-the-4c)
3. [Getting started](#getting-started)
4. [Using Tobii SDK in your own projects](#using-tobii-sdk-in-your-own-projects)
5. [Getting the Gaze Point from the Tobii SDK](#getting-the-gaze-point-from-the-tobii-sdk)
6. [Smoothing Algorithm](#smoothing-algorithm)
7. [Known Issues](#known-issues)
8. [Operating System Support and Language Bindings](#operating-system-support-and-language-bindings)
9. [Tobii License Information](#tobii-license-information)

## Documentation and References
[Learn Unity Basics](https://docs.unity3d.com/Manual/UnityBasics.html)

[Tobii Unity SDK Download](https://github.com/Tobii/UnitySDK/releases)

[Tobii Unity User Manual](https://tobii.github.io/UnitySDK/manual)

[Tobii Unity API Documentation](https://tobii.github.io/UnitySDK/scripting-api)

[Tobii Unity SDK Github](https://github.com/Tobii/UnitySDK)

[Tobii Developer Forums](http://developer.tobii.com/community-forums/)

[Troubleshooting](https://tobii.github.io/UnitySDK/troubleshooting)

## Setting up the 4c
1. Position the eye tracker below your screen, angled slightly up.
2. Plug the eye tracker into your computer's USB slot
3. Download the [Tobii Eye Tracking Core Software](https://tobiigaming.com/getstarted/)
4. Open and follow the on-screen instructions to install.
5. Click the Tobii Eye Tracking menu from the bottom right of the Window's taskbar
6. Set-up a profile for each user (for most accurate results) and calibrate

## Getting started
To open or run this application locally, there are two options.

First make sure Unity is downloaded. You can download it [here.](https://unity3d.com/get-unity/download)

1. Download TobiiBasicDrawing.unitypackage from the pre-release [here](https://github.com/BenLeech/tobii-unity-basic-drawing/releases/tag/v0.1)
2. Create a new Unity Project
3. In the top menubar, go to Assets > Import Package > Custom Package
4. Import TobiiBasicDrawing.unitypackage

## Using Tobii SDK in your own projects
If you start a project from scratch, you will need to add the Tobii Core SDK into your project first. 

1. Download the Tobii Unity SDK [here](https://github.com/Tobii/UnitySDK/releases). Save it somewhere easily accessible.
2. Open Unity, and create a New Unity Project. Select Windows as Target Platform.
3. On the top menubar, go to Assets > Import Package -> Custom Package...
4. Import the Tobii Unity SDK package file you just downloaded
5. Import all assets in the package (you can unselect DemoScenes if you don't want the demos)
6. Accept the license agreement

You can now reference the Tobii API in your scripts, or access Tobii assets in Unity.

## Playing Tobii's Unity Demo Scenes
If you haven't imported the Tobii Demo scenes already, follow the steps in the [above section](#using-tobii-sdk-in-your-own-projects). Make sure you import the demo scene assets from the package.

In the project view, go to Assets > Tobii > DemoScenes. There are several different demos for you to try out.

## Getting the Gaze Point from the Tobii SDK
To create any data streams using the Tobii SDK, you simply need to call getGazePoint() from the Tobii API

```
GazePoint gazePoint = TobiiAPI.GetGazePoint();
```

This will get the last recorded gaze point. You can put it in unity's update() method to get a gaze point every frame

```
void Update () {

		GazePoint gazePoint = TobiiAPI.GetGazePoint();

}
```

You can then access the point by calling the Screen variable, which is a Vector2 point for the gaze point.
```
float x = gazePoint.Screen.x;
float y = gazePont.Screen.y;
```

GetGazePoint() lazily loads the gaze point data, so if you explicitly want to start the gaze point stream, you can use TobiiAPI.SubscribeGazePointData().

## Smoothing Algorithm
This application contains basic live data smoothing techniques and algorithms to help smooth the noise of the eye tracking data stream.

If you are interested in possibling improving performance, Professor Manu Kumar from Stanford University has published a paper 
containing pseudocode for the implementation of a smoothing algorithm specifically for eye tracking.

It can be found here:
http://hci.stanford.edu/cstr/reports/2007-03.pdf

## Known Issues
##### Offset gaze coordinates and window scale
   * Window's scale setting must be set at 1:1 or 100%, otherwise gaze coordinates will be offset and not where the user is actually looking.

##### Tobii EyeX is not compatible with Tobii v2.11
   * A complete uninstall is required between each use with 2.11. There are no issues with the Tobii 4c

## Operating System Support and Language Bindings
Operating Systems Supported: Windows7, Windows8, Windows10

Language Bindings: C#

Supported Unity Versions:	4.5+ (API), 5.0+ (Demo Scripts), 5.3.7+ (Demo Scenes)

## Tobii License Information
This application and any applications developed during the hackathon event are under Tobii's Interactive Use license agreement. This means that the SDK can be used freely to develop games or other software where eye tracking data is used as a user input for interactive experiences. 

However, this application and any applications developed during the hackathon event must not be used for analytic use. Tobii defines analytical use as the following.

> Analytical Use is where eye tracking data is either (a) stored; or (b) transferred to another computing device or network; in both cases where the intent is to use or make it possible to use eye tracking data to analyze, record, visualize or interpret behavior or attention.

[Download and read the full license agreement for Tobii SDK](http://developer.tobii.com/?wpdmdl=203)

