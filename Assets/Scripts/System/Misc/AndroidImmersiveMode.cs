using UnityEngine;

public class AndroidImmersiveMode : MonoBehaviour
{
#if UNITY_ANDROID && !UNITY_EDITOR
    void Start()
    {
        ApplyImmersiveMode();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
            ApplyImmersiveMode();
    }

    void ApplyImmersiveMode()
    {
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject window = activity.Call<AndroidJavaObject>("getWindow");
            AndroidJavaObject decorView = window.Call<AndroidJavaObject>("getDecorView");

            int flags =
                0x00000400 | // LAYOUT_FULLSCREEN
                0x00000200 | // LAYOUT_HIDE_NAVIGATION
                0x00000100 | // LAYOUT_STABLE
                0x00000002 | // HIDE_NAVIGATION
                0x00000004 | // FULLSCREEN
                0x00001000;  // IMMERSIVE_STICKY

            decorView.Call("setSystemUiVisibility", flags);
        }
    }
#endif
}