using Firebase.Analytics;
using SaveData;
using UnityEngine;

public class ParametersGetter : MonoBehaviour
{
    public static Parameter[] GetDieParameters(PlayerDataHandler playerData, DifficultyManager difficultyManager, ScoreManager scoreManager, int timePlayed, OrderManager orderManager)
    {

        string activeOrders = string.Empty;
        
        for (int i = 0; i < orderManager.ActiveOrders.Count; i++)
        {
            if (i < orderManager.ActiveOrders.Count - 1)
                activeOrders += $"{orderManager.ActiveOrders[i]}.";
            else
                activeOrders += $"{orderManager.ActiveOrders[i]}, ";
        }
        
        Parameter[] parameters = new[]
        {
            new Parameter("currentScore", scoreManager.currentScore),
            new Parameter("currentHighScore", playerData.Progress.GetHighScore()),
            new Parameter("difficulty", difficultyManager.GetDifficulty().ToString()),
            new Parameter("timePlayed", timePlayed),
            new Parameter("lastMealExpired", orderManager.LastMealExpired),
            new Parameter("activeOrders", activeOrders),
        };
        
        return parameters;
    }
    
}
