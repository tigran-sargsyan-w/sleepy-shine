using System;
using UnityEngine;

public static class FlashLightManager
{
    static AndroidJavaObject camera;
    static string cameraId;
    //Check if device version
    public static void Enable()
    {
        if (AndroidUnityBridge.SDK_INT >= AndroidDeviceInfo.VersionCodes.M)
        {
            TurnOnNew();
        }
        else
        {
            TurnOnOld();
        }
    }
    
    public static void Disable()
    {
        if (AndroidUnityBridge.SDK_INT >= AndroidDeviceInfo.VersionCodes.M)
        {
            TurnOffNew();
        }
        else
        {
            TurnOffOld();
        }
    }

    //Flashlight Enable
    static void TurnOnOld()
    {
        
        if (camera != null)
        {
            Debug.LogWarning("Flashlight is already on");
            return;
        }
        try
        {
            AndroidUnityBridge.RunOnUiThread(() =>
            {
                using (var camAJC = new AndroidJavaClass("android.hardware.Camera"))
                {
                    var cam = camAJC.CallStaticAJO("open");
                    var camParams = cam.CallAJO("getParameters");
                    camParams.Call("setFlashMode", "torch");
                    cam.Call("setParameters", camParams);
                    cam.Call("startPreview");
                    camera = cam;
                }
            });
        }
        catch (Exception e)
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log("Could not enable flashlight:" + e.Message);
            }
        }
    }

    static void TurnOnNew()
    {
        try
        {
            cameraId = AndroidUnityBridge.CameraService.Call<string[]>("getCameraIdList")[0];
            AndroidUnityBridge.CameraService.Call("setTorchMode", cameraId, true);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }
    
    //Flashlight Disable
    static void TurnOffOld()
    {
        if (camera == null)
        {
            return;
        }

        AndroidUnityBridge.RunOnUiThread(() =>
        {
            camera.Call("stopPreview");
            camera.Call("release");
            camera.Dispose();
            camera = null;
        });
    }
    
    static void TurnOffNew()
    {
        try
        {
            AndroidUnityBridge.CameraService.Call("setTorchMode", cameraId, false);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }
}
