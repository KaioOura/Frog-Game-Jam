using Firebase.Crashlytics;
using UnityEngine;

public static class GameLogger
{
    public static void Log(string message)
    {
        Debug.Log(message);
        Crashlytics.Log(message);
    }

    public static void Warning(string message)
    {
        Debug.LogWarning(message);
        Crashlytics.Log("[Warning] " + message);
    }

    public static void Error(string message)
    {
        Debug.LogError(message);
        Crashlytics.Log("[Error] " + message);
    }

    public static void Exception(System.Exception e)
    {
        Debug.LogException(e);
        Crashlytics.LogException(e);
    }

    public static void SetUser(string userId)
    {
        Crashlytics.SetUserId(userId);
    }

    public static void SetKey(string key, object value)
    {
        Crashlytics.SetCustomKey(key, value.ToString());
    }
}