using Firebase.Analytics;
using UnityEngine;

public static class GameAnalyticsManager
{
    public static void Track(string eventName, params (string key, object value)[] parameters)
    {
        if (parameters == null || parameters.Length == 0)
        {
            FirebaseAnalytics.LogEvent(eventName);
            Debug.Log($"[Analytics] Evento: {eventName}");
        }
        else
        {
            Parameter[] firebaseParams = new Parameter[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                firebaseParams[i] = new Parameter(parameters[i].key, parameters[i].value.ToString());
            }
            FirebaseAnalytics.LogEvent(eventName, firebaseParams);
            Debug.Log($"[Analytics] Evento: {eventName} com {parameters.Length} parâmetros.");
        }
    }
}