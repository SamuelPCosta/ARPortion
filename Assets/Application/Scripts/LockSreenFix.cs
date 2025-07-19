using UnityEngine;

public class LockScreenFix : MonoBehaviour
{
#if UNITY_ANDROID && !UNITY_EDITOR

    void Start()
    {
        var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        var window = activity.Call<AndroidJavaObject>("getWindow");

        int FLAG_SHOW_WHEN_LOCKED = 0x00080000;
        int FLAG_TURN_SCREEN_ON = 0x00200000;
        int FLAG_KEEP_SCREEN_ON = 128;

        window.Call("clearFlags", FLAG_SHOW_WHEN_LOCKED | FLAG_TURN_SCREEN_ON | FLAG_KEEP_SCREEN_ON);

        if (AndroidJNI.GetVersion() >= 26)
        {
            activity.Call("setShowWhenLocked", false);
            activity.Call("setTurnScreenOn", false);
        }
    }
#endif
}