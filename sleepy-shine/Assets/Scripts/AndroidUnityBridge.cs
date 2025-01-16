using System;
using UnityEngine;

public static class AndroidUnityBridge
{
    private static AndroidJavaObject activity;
    private static AndroidJavaObject cameraService;
    
    private const int PERMISSION_GRANTED = 0;
    private const int PERMISSION_DENIED = -1;
    private const RuntimePlatform PLATFORM_ANDROID = RuntimePlatform.Android;
    private const RuntimePlatform PLATFORM_IOS = RuntimePlatform.IPhonePlayer;
    private const string CAMERA_FLASH = "android.hardware.camera.flash";

    public static bool IsAndroid => CheckPlatform(PLATFORM_ANDROID);
    public static AndroidJavaObject CameraService => TryGetCameraService();
    public static bool IsAndroidVersionNew => IsAndroidVersionAtLeastM();
    public static bool TryCheckPermission(string permission) => IsPermissionGranted(permission);
    public static bool HasFlashlight => HasSystemFeature(CAMERA_FLASH);


    private static AndroidJavaObject Activity
    {
        get
        {
            if (activity == null)
            {
                var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }

            return activity;
        }
    }

    private static bool CheckPlatform(RuntimePlatform platform)
    {
        switch (platform)
        {
            case RuntimePlatform.Android:
                Debug.LogWarning("App run on Android Platform");
                return true;
            case RuntimePlatform.IPhonePlayer:
                Debug.LogWarning("App run on IOS Platform");
                return true;
            default:
                Debug.LogWarning("App run on Unknown Platform");
                return false;
        }
    }

    private static bool IsAndroidVersionAtLeastM()
    {
        return (TryGetSdkVersion() >= AndroidDeviceInfo.VersionCodes.M);
    }
    
    private static bool IsPermissionGranted(string permission)
    {
        if (string.IsNullOrEmpty(permission)) return false;
        try
        {
            using (var c = new AndroidJavaClass("androidx.core.content.ContextCompat"))
            {
                return c.CallStaticInt("checkSelfPermission", Activity, permission) == PERMISSION_GRANTED;
            }
        }
        catch (Exception exception)
        {
            Debug.LogWarning("Could not check permission." + exception.Message);
            return false;
        }
    }
    
    private static int TryGetSdkVersion()
    {
        return GetDeviceStrProperty<int>("android.os.Build$VERSION", "SDK_INT");
    }

    private static T GetDeviceStrProperty<T>(string className, string propertyName)
    {
        try
        {
            using (var version = new AndroidJavaClass(className))
            {
                return version.GetStatic<T>(propertyName);
            }
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Failed to get property:{propertyName} of class {className}, reason: {exception.Message}");
            return default;
        }
    }

    private static AndroidJavaObject TryGetCameraService()
    {
        return cameraService ??= GetSystemService("camera", "android.hardware.camera2.CameraManager");
    }

    private static bool HasSystemFeature(string feature)
    {
        using (var packageManager = Activity.CallAjo("getPackageManager"))
        {
            return packageManager.CallBool("hasSystemFeature", feature);
        }
    }

    private static AndroidJavaObject GetSystemService(string name, string serviceClass)
    {
        try
        {
            var serviceObj = Activity.CallAjo("getSystemService", name);
            return serviceObj.Cast(serviceClass);
        }
        catch (Exception exception)
        {
            Debug.LogWarning("Failed to get " + name + " service. Error: " + exception.Message);
            return null;
        }
    }
    
    
    //common public methods
    public static void RunOnUiThread(Action action)
    {
        Activity.Call("runOnUiThread", new AndroidJavaRunnable(action));
    }

    public static AndroidJavaObject CallAjo(this AndroidJavaObject ajo, string methodName, params object[] args)
    {
        return ajo.Call<AndroidJavaObject>(methodName, args);
    }

    public static AndroidJavaObject CallStaticAjo(this AndroidJavaClass ajc, string methodName, params object[] args)
    {
        return ajc.CallStatic<AndroidJavaObject>(methodName, args);
    }

    
    //common
    private static bool CallBool(this AndroidJavaObject ajo, string methodName, params object[] args)
    {
        return ajo.Call<bool>(methodName, args);
    }
    
    private static int CallStaticInt(this AndroidJavaClass ajc, string methodName, params object[] args)
    {
        return ajc.CallStatic<int>(methodName, args);
    }

    private static AndroidJavaObject Cast(this AndroidJavaObject source, string destClass)
    {
        using (var destinationClassAjc = ClassForName(destClass))
        {
            return destinationClassAjc.Call<AndroidJavaObject>("cast", source);
        }
    }

    private static AndroidJavaObject ClassForName(string className)
    {
        using (var javaClass = new AndroidJavaClass("java.lang.Class"))
        {
            return javaClass.CallStaticAjo("forName", className);
        }
    }
}