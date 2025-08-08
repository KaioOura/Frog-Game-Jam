using System.Collections.Generic;
using System.Linq;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class LeaderboardManager
{
    public void SaveToLeaderboard(string userId, PlayerData playerData)
    {
        LeaderboardEntry leaderboardEntry = new LeaderboardEntry();
        leaderboardEntry.name = playerData.Username;
        leaderboardEntry.score = playerData.Highschore;
        
        DatabaseReference dbRefLeaderboard = FireBaseInitializer.databaseReference.Child("Leaderboard");

        string json = JsonUtility.ToJson(leaderboardEntry);

        dbRefLeaderboard.Child(userId).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                Debug.Log("Dados salvos no leaderboard com sucesso.");
            }
            else
            {
                string errorMsg = task.Exception?.Flatten().Message ?? "Erro desconhecido ao salvar.";
                Debug.LogError("Erro ao salvar leaderboard: " + errorMsg);
            }
        });
    }


    public void LoadTop10Leaderboard()
    {
        DatabaseReference dbRefLeaderboard = FireBaseInitializer.databaseReference.Child("Leaderboard");
        dbRefLeaderboard
            .OrderByChild("score")
            .LimitToLast(10) // <- Pega os 10 maiores scores
            .GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    DataSnapshot snapshot = task.Result;

                    List<LeaderboardEntry> leaderboard = new List<LeaderboardEntry>();

                    foreach (DataSnapshot child in snapshot.Children)
                    {
                        string json = child.GetRawJsonValue();
                        LeaderboardEntry entry = JsonUtility.FromJson<LeaderboardEntry>(json);
                        leaderboard.Add(entry);
                    }
                    
                    leaderboard = leaderboard.OrderByDescending(entry => entry.score).ToList();

                    foreach (var entry in leaderboard)
                    {
                        Debug.Log($"{entry.name}: {entry.score}");
                    }
                }
                else
                {
                    Debug.LogError("Erro ao carregar leaderboard: " + task.Exception?.Flatten().Message);
                }
            });
    }
}

[System.Serializable]
public class LeaderboardEntry
{
    public string name;
    public int score;
}