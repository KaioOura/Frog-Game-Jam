using UnityEngine;
using Firebase.Crashlytics;
using System;

public class GlobalExceptionHandler : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Application.logMessageReceived += HandleException;
    }

    private void HandleException(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Exception)
        {
            Crashlytics.Log(condition);
            Crashlytics.Log(stackTrace);
            Crashlytics.LogException(new Exception(condition + "\n" + stackTrace));
        }
    }
}