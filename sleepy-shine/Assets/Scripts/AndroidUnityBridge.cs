using System;
using UnityEngine;

public static class AndroidUnityBridge
{
    static AndroidJavaObject activity;
    static AndroidJavaObject cameraService;
    
    const int PERMISSION_GRANTED = 0;
    const int PERMISSION_DENIED = -1;

    //Check if device is Android
    public static bool IsNotAndroid()
    {
        return Application.platform != RuntimePlatform.Android;
    }

    //Check if Device han a flashlight
    public static bool HasFlashlight => HasSystemFeature("android.hardware.camera.flash");

    public static bool HasSystemFeature(string feature)
    {
        using (var pm = PackageManager)
        {
            return pm.CallBool("hasSystemFeature", feature);
        }
    }

    public static AndroidJavaObject PackageManager => Activity.CallAJO("getPackageManager");

    public static bool CallBool(this AndroidJavaObject ajo, string methodName, params object[] args)
    {
        return ajo.Call<bool>(methodName, args);
    }


    public static AndroidJavaObject Activity
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

    public static AndroidJavaObject CallAJO(this AndroidJavaObject ajo, string methodName, params object[] args)
    {
        return ajo.Call<AndroidJavaObject>(methodName, args);
    }
    
    //Check if app has a permission
    public static bool IsPermissionGranted(string permission)
    {
        if (string.IsNullOrEmpty(permission))
        {
            return false;
        }

        try
        {
            using (var c = new AndroidJavaClass("androidx.core.content.ContextCompat"))
            {
                return c.CallStaticInt("checkSelfPermission", Activity, permission) == PERMISSION_GRANTED;
            }
        }
        catch (Exception ex)
        {
            if (Debug.isDebugBuild)
            {
                Debug.LogWarning(
                    "Could not check if runtime permission is granted. Check if Android version is 6.0 (API level 23) or higher. " +
                    ex.Message);
            }

            return false;
        }
    }
    public static int CallStaticInt(this AndroidJavaClass ajc, string methodName, params object[] args)
    {
        return ajc.CallStatic<int>(methodName, args);
    }

    //Check device sdk version
    
    public static int SDK_INT => GetDeviceStrProperty<int>("android.os.Build$VERSION", "SDK_INT");
    
    static T GetDeviceStrProperty<T>(string className, string propertyName)
    {
        try
        {
            using (var version = new AndroidJavaClass(className))
            {
                return version.GetStatic<T>(propertyName);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Failed to get property: {propertyName} of class {className}, reason: {e.Message}");
            return default(T);
        }
    }
    //common method to call flashlight
    public static AndroidJavaObject CameraService => 
        cameraService ?? (cameraService = GetSystemService("camera", "android.hardware.camera2.CameraManager"));
    
    static AndroidJavaObject GetSystemService(string name, string serviceClass)
    {
        try
        {
            var serviceObj = Activity.CallAJO("getSystemService", name);
            return serviceObj.Cast(serviceClass);
        }
        catch (Exception e)
        {
            if (Debug.isDebugBuild)
            {
                Debug.LogWarning("Failed to get " + name + " service. Error: " + e.Message);
            }

            return null;
        }
    }
    
    public static AndroidJavaObject Cast(this AndroidJavaObject source, string destClass)
    {
        using (var destClassAJC = ClassForName(destClass))
        {
            return destClassAJC.Call<AndroidJavaObject>("cast", source);
        }
    }
    
    public static AndroidJavaObject ClassForName(string className)
    {
        using (var clazz = new AndroidJavaClass("java.lang.Class"))
        {
            return clazz.CallStaticAJO("forName", className);
        }
    }
    
    public static AndroidJavaObject CallStaticAJO(this AndroidJavaClass ajc, string methodName, params object[] args)
    {
        return ajc.CallStatic<AndroidJavaObject>(methodName, args);
    }
    
    public static void RunOnUiThread(Action action)
    {
        Activity.Call("runOnUiThread", new AndroidJavaRunnable(action));
    }
    
}