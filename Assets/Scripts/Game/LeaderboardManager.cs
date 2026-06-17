using System;
using System.Collections.Generic;
using System.Linq;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    public Action<List<LeaderboardEntry>> OnLeaderboardUpdated;
    [SerializeField] private FakePlayerHolderSo fakePlayerHolderSo;

    private const int MaxEntries = 10;

    private List<LeaderboardEntry> _fakeEntries = new List<LeaderboardEntry>();
    private DatabaseReference _dbRefLeaderboard;

    public void Initialize()
    {
        // Offline (databaseReference nulo): mantém _dbRefLeaderboard nulo; o
        // GetEntries cai no caminho fakes-only.
        if (FireBaseInitializer.databaseReference != null)
            _dbRefLeaderboard = FireBaseInitializer.databaseReference.Child("Leaderboard");

        DateTimeOffset fixedDate = new DateTimeOffset(2023, 5, 10, 14, 30, 0, TimeSpan.Zero);
        long timestamp = fixedDate.ToUnixTimeMilliseconds();
        
        foreach (var fakePlayerSo in fakePlayerHolderSo.fakePlayers)
        {
            LeaderboardEntry leaderboardEntry = new LeaderboardEntry();
            leaderboardEntry.name = fakePlayerSo.playerName;
            leaderboardEntry.score = fakePlayerSo.highScore;
            //leaderboardEntry.sortKey = sortKey;
            leaderboardEntry.timeStamp = timestamp;
            
            _fakeEntries.Add(leaderboardEntry);
        }
    }

    public void SaveToLeaderboard(string userId, PlayerData playerData)
    {
        LeaderboardEntry leaderboardEntry = new LeaderboardEntry();
        leaderboardEntry.name = playerData.Username;
        leaderboardEntry.score = playerData.Highschore;
        leaderboardEntry.timeStamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        
        string json = JsonUtility.ToJson(leaderboardEntry);

        _dbRefLeaderboard.Child(userId).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
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
        List<LeaderboardEntry> leaderboardEntries = new List<LeaderboardEntry>();

        if (_dbRefLeaderboard == null)
        {
            // Offline: mostra apenas os jogadores fake.
            OnLeaderboardUpdated?.Invoke(_fakeEntries
                .OrderByDescending(e => e.score)
                .ThenBy(e => e.timeStamp)
                .Take(MaxEntries)
                .ToList());
            return;
        }

        // Ordena e limita no servidor: só baixa os maiores scores (exige
        // ".indexOn": "score" nas regras do Realtime Database).
        _dbRefLeaderboard
            .OrderByChild("score")
            .LimitToLast(MaxEntries)
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

                    leaderboardEntries.AddRange(_fakeEntries);

                    leaderboardEntries = leaderboardEntries
                        .OrderByDescending(e => e.score)
                        .ThenBy(e => e.timeStamp)
                        .Take(MaxEntries)
                        .ToList();

                    OnLeaderboardUpdated?.Invoke(leaderboardEntries);
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
    public long timeStamp;
}