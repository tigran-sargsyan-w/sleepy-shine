using System;
using UnityEngine;

public static class Flashlight
{
    #region Fields

    static AndroidJavaObject camera;
    static string cameraId;

    #endregion

    #region Methods

    public static void Enable()
    {
        if (AndroidUnityBridge.IsAndroidVersionNew)
        {
            EnableNewFlashlight();
        }
        else
        {
            EnableOldFlashlight();
        }
    }
    
    public static void Disable()
    {
        if (AndroidUnityBridge.IsAndroidVersionNew)
        {
            DisableNewFlashlight();
        }
        else
        {
            DisableOldFlashlight();
        }
    }

    #endregion

    #region Enable Flashlight

    private static void EnableOldFlashlight()
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
                using (var cameraAjc = new AndroidJavaClass("android.hardware.Camera"))
                {
                    var cameraAjo = cameraAjc.CallStaticAjo("open");
                    var cameraParams = cameraAjo.CallAjo("getParameters");
                    cameraParams.Call("setFlashMode", "torch");
                    cameraAjo.Call("setParameters", cameraParams);
                    cameraAjo.Call("startPreview");
                    camera = cameraAjo;
                }
            });
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
        }
    }

    private static void EnableNewFlashlight()
    {
        try
        {
            cameraId = AndroidUnityBridge.CameraService.Call<string[]>("getCameraIdList")[0];
            AndroidUnityBridge.CameraService.Call("setTorchMode", cameraId, true);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
        }
    }

    #endregion
    
    #region Disable Flashlight

    private static void DisableOldFlashlight()
    {
        if (camera == null) return;
        try
        {
            AndroidUnityBridge.RunOnUiThread(() =>
            {
                camera.Call("stopPreview");
                camera.Call("release");
                camera.Dispose();
                camera = null;
            });
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
        }
    }
    
    private static void DisableNewFlashlight()
    {
        try
        {
            AndroidUnityBridge.CameraService.Call("setTorchMode", cameraId, false);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
        }
    }

    #endregion
}
