using UnityEngine;

class Simcard
{
    public static string GetTwoSmallLetterCountryCodeISO()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR

        AndroidJavaClass jcUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

        AndroidJavaObject joUnityActivity = jcUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        AndroidJavaObject joAndroidPluginAccess = new AndroidJavaObject("com.flipmorris.simcardinfo.SimcardManager");

        return joAndroidPluginAccess.Call<string>("GetTwoSmallLetterCountryCodeISO", joUnityActivity);

        #else

        return string.Empty;

        #endif
    }
}
