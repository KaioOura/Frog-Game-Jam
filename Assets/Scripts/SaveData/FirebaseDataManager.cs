using System;
using System.Collections;
using Firebase.Auth;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using SaveData;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class PlayerData
{
    public string nickname;
    public int score;
    public int coins;
    public List<string> items;
    public int level;

    public string userID;
}

public class FirebaseDataManager : MonoBehaviour
{
    DatabaseReference dbRef;

    public PlayerData playerData;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => FirebaseAuthManager.FirebaseReady);

        dbRef = FirebaseAuthManager.FirebaseDatabase.GetReference("players");
    }

    [ContextMenu("NextScene")]
    public void NextScene()
    {
        SceneManager.LoadScene(sceneBuildIndex: 2);
    }
    
    [ContextMenu("Save")]
    public void SavePlayerData()
    {
        string uid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        
        string json = JsonUtility.ToJson(playerData);
        dbRef.Child("players").Child(playerData.userID).SetRawJsonValueAsync(json);
    }

    [ContextMenu("Load")]
    public void LoadPlayerData()
    {
        dbRef.Child("players").Child(playerData.userID).GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("Erro ao ler dados: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                string json = snapshot.GetRawJsonValue();
                if (!string.IsNullOrEmpty(json))
                {
                    PlayerData data = JsonUtility.FromJson<PlayerData>(json);
                    playerData = data;
                    Debug.Log("Score: " + data.score + ", Coins: " + data.coins);
                }
                else
                {
                    Debug.LogWarning("Nenhum dado encontrado para esse jogador.");
                }
            }
        });
    }

    public void GetLeaderboard()
    {
        dbRef.Child("players").OrderByChild("Score").LimitToLast(10).GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("Erro ao carregar leaderboard: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                List<PlayerData> leaderboard = new List<PlayerData>();

                foreach (DataSnapshot child in snapshot.Children)
                {
                    string json = child.GetRawJsonValue();
                    PlayerData player = JsonUtility.FromJson<PlayerData>(json);
                    leaderboard.Add(player);
                }

                // Reverter para ordem decrescente (Firebase retorna do menor pro maior)
                leaderboard.Reverse();

                foreach (var p in leaderboard)
                {
                    Debug.Log($"{p.nickname}: {p.score}");
                }
            }
        });
    }
}