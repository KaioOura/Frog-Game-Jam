using System.Linq;
using Firebase.Analytics;
using UnityEngine;

public static class GameAnalyticsManager
{
    public static void Track(string eventName, Parameter[] parameters = null)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            Debug.LogWarning("[Analytics] Tentativa de logar evento com nome vazio.");
            return;
        }

        if (parameters == null || parameters.Length == 0)
        {
            FirebaseAnalytics.LogEvent(eventName);
            Debug.Log($"[Analytics] Evento: {eventName}");
        }
        else
        {
            FirebaseAnalytics.LogEvent(eventName, parameters);

            // Mostra os parâmetros no console (útil para debug)
            //string paramList = parameters.Select()
            //Debug.Log($"[Analytics] Evento: {eventName} com parâmetros: {paramList}");
        }
    }

}