using UnityEngine;

public class AndroidImmersiveMode : MonoBehaviour
{
#if UNITY_ANDROID && !UNITY_EDITOR
    AndroidJavaObject decorView;
#endif

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        SetImmersiveMode();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            SetImmersiveMode();
        }
    }

    public void SetImmersiveMode()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject window = activity.Call<AndroidJavaObject>("getWindow");
                decorView = window.Call<AndroidJavaObject>("getDecorView");

                int flags =
                    0x00000004 | // SYSTEM_UI_FLAG_HIDE_NAVIGATION
                    0x00000002 | // SYSTEM_UI_FLAG_FULLSCREEN
                    0x00000100 | // SYSTEM_UI_FLAG_LAYOUT_STABLE
                    0x00000200 | // SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
                    0x00000400 | // SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN
                    0x00001000;  // SYSTEM_UI_FLAG_IMMERSIVE_STICKY

                decorView.Call("setSystemUiVisibility", flags);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Immersive Mode failed: " + e.Message);
        }
#endif
    }
}