using System;
using System.Collections.Generic;
using System.Linq;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class LeaderboardManager
{
    public Action<List<LeaderboardEntry>> OnLeaderboardUpdated;
    
    public void SaveToLeaderboard(string userId, PlayerData playerData)
    {
        LeaderboardEntry leaderboardEntry = new LeaderboardEntry();
        leaderboardEntry.name = playerData.Username;
        leaderboardEntry.score = playerData.Highschore;
        
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        long invertedScore = long.MaxValue - playerData.Highschore;
        
        string sortKey = $"{invertedScore:D20}_{timestamp:D20}";

        leaderboardEntry.sortKey = sortKey;
        
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


    public void GetEntries()
    {
        DatabaseReference dbRefLeaderboard = FireBaseInitializer.databaseReference.Child("Leaderboard");
        List<LeaderboardEntry> leaderboardEntries = new List<LeaderboardEntry>();
        dbRefLeaderboard
            .OrderByChild("sortKey")
            .LimitToLast(10) // <- Pega os 10 maiores scores
            .GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    DataSnapshot snapshot = task.Result;
                    
                    foreach (DataSnapshot child in snapshot.Children)
                    {
                        string json = child.GetRawJsonValue();
                        LeaderboardEntry entry = JsonUtility.FromJson<LeaderboardEntry>(json);
                        leaderboardEntries.Add(entry);
                    }

                    leaderboardEntries = leaderboardEntries.OrderByDescending(entry => entry.score).ToList();
                    OnLeaderboardUpdated?.Invoke(leaderboardEntries);
                    //return ;
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
    public string sortKey;
}