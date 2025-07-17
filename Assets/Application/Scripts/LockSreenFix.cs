using UnityEngine;

public class LockScreenFix : MonoBehaviour
{
    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (var window = activity.Call<AndroidJavaObject>("getWindow"))
        {
            int flagShowWhenLocked = 524288;
            int flagTurnScreenOn = 2097152;
            window.Call("clearFlags", flagShowWhenLocked | flagTurnScreenOn);
        }
#endif
    }
}