using System;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public string nickname;
    public int score;
    public int coins;
    public List<string> items;
    public int level;
}

public class FirebaseDataManager : MonoBehaviour
{
    FirebaseFirestore db;

    public PlayerData playerData;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
    }

    [ContextMenu("Save")]
    public void SavePlayerData()
    {
        string uid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        
        db.Collection("players").Document(uid).SetAsync(playerData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted) Debug.Log("Data saved!");
        });
    }

    [ContextMenu("Load")]
    public void LoadPlayerData()
    {
        string uid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        db.Collection("players").Document(uid).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DocumentSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    Dictionary<string, object> data = snapshot.ToDictionary();
                    string nickname = data["nickname"].ToString();
                    int score = int.Parse(data["score"].ToString());
                    int coins = int.Parse(data["coins"].ToString());
                    List<object> itemsList = (List<object>)data["items"];
                    List<string> items = itemsList.ConvertAll(i => i.ToString());
                    int level = int.Parse(data["level"].ToString());

                    Debug.Log($"Loaded Player: {nickname} | Score: {score} | Coins: {coins} | Level: {level}");
                }
                else
                {
                    Debug.LogWarning("No data found for this user.");
                }
            }
            else
            {
                Debug.LogError("Failed to load player data: " + task.Exception);
            }
        });
    }

    public void GetLeaderboard()
    {
        db.Collection("players").OrderByDescending("score").Limit(10).GetSnapshotAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    foreach (var doc in task.Result.Documents)
                    {
                        var data = doc.ToDictionary();
                        Debug.Log($"Player: {data["nickname"]} - Score: {data["score"]}");
                    }
                }
            });
    }
}