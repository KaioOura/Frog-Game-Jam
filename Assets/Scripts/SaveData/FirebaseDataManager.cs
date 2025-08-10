using System;
using System.Collections;
using Firebase.Auth;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using SaveData;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

[Serializable]
public class PlayerData
{
    public string UserID;
    public string Username;
    public int Highschore;
    public int Coins;
    public List<string> Items = new List<string> { "sword", "potion", "shield" };
    //public Dictionary<PowerUpEnum, int> Upgrades = new Dictionary<PowerUpEnum, int>();
    public int Level;

    public PlayerData()
    {
        UserID = string.Empty;
        Username = string.Empty;
        Coins = 0;
        Level = 0;
        Highschore = 0;
        Items = new List<string> { "sword", "potion", "shield" };
        //Upgrades = new Dictionary<PowerUpEnum, int>();
        Level = 0;
    }
}

public class FirebaseDataManager : MonoBehaviour
{
    public event Action OnFailDataLoad;
    public event Action OnSuccessfulDataLoad;


    public PlayerData PlayerData => _playerData;
    public string UserID => _userID;

    private PlayerData _playerData;
    private DatabaseReference dbRef;
    private string _userID; //It's used to access user's database on FireBase to retrieve and send data

    private LeaderboardManager _leaderboardManager;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    [ContextMenu("Clear UserID")]
    public void ClearUserID()
    {
        PlayerPrefs.DeleteKey("UserID");
    }

    IEnumerator Start()
    {
        yield return new WaitUntil(() => FireBaseInitializer.FirebaseReady);
        //yield return new WaitUntil(() => FirebaseAuthManager.LoggedIn);

        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        dbRef = FireBaseInitializer.databaseReference.Child("Players");

// #if UNITY_EDITOR
//         if (PlayerPrefs.HasKey("UserID"))
//         {
//             _userID = PlayerPrefs.GetString("UserID");
//             Debug.Log("PlayerPrefs UserID found, using it...");
//         }
//         else
//         {
//             _userID = $"{auth.CurrentUser.UserId}"; //This ID is set manually in FireBaseInitializer while playing on Unity
//             PlayerPrefs.SetString("UserID", _userID);
//         }
//
// #else
        

        if (auth.CurrentUser != null)
        {
            Debug.Log("Usuário já está logado: " + auth.CurrentUser.UserId);
            _userID = auth.CurrentUser.UserId;
            // Pode seguir usando o Database normalmente
        }
        else
        {
            Debug.Log("Nenhum usuário logado, autenticando anonimamente...");
            auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Erro ao autenticar: " + task.Exception);
                    return;
                }
                
                _userID = task.Result.User.UserId;
                Debug.Log("Novo usuário anônimo criado: " + _userID);
            });
        }
        
//#endif

        print("FirebaseDatabaseReference found successfully.");

        _leaderboardManager = new LeaderboardManager();

        LoadPlayerData();
    }

    [ContextMenu("NextScene")]
    public void NextScene()
    {
        SceneManager.LoadScene(sceneBuildIndex: 2);
    }

    public void InitialSavePlayerData(PlayerData playerData, Action onSuccessfulComplete = null,
        Action onFailComplete = null)
    {
        _playerData = playerData;
        SavePlayerData(_playerData, onSuccessfulComplete, onFailComplete);
    }

    [ContextMenu("Save")]
    public void SavePlayerData()
    {
        SavePlayerData(_playerData);
    }

    private void SavePlayerData(PlayerData playerData, Action onSuccessfulComplete = null, Action onFailComplete = null)
    {
        //string json = JsonConvert.SerializeObject(_playerData);
        string json = JsonUtility.ToJson(playerData);

        dbRef.Child(_userID).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                Debug.Log("Data saved successfully.");
                onSuccessfulComplete?.Invoke(); // true = sucesso
            }
            else
            {
                string errorMsg = task.Exception?.Flatten().Message ?? "Erro desconhecido ao salvar.";
                Debug.LogError("Error saving data.: " + errorMsg);
                onFailComplete?.Invoke(); // false = falhou
            }
        });

        _leaderboardManager.SaveToLeaderboard(_userID, playerData);
    }

    [ContextMenu("Load")]
    public void LoadPlayerData()
    {
        print("Trying to locate player data.");

        dbRef.Child(_userID).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            print($"Searching for player {_userID} data...");
            if (task.IsFaulted)
            {
                Debug.LogError("Error accessing the database: " + task.Exception);
                return;
            }

            if (task.IsCanceled)
            {
                Debug.LogWarning("Reading canceled.");
                return;
            }

            if (!task.IsCompleted) return;

            DataSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                //Debug.LogError($"User: {_userID} data found.");
                print($"User: {_userID} data found.");

                string json = snapshot.GetRawJsonValue();
                if (string.IsNullOrEmpty(json)) return;
                PlayerData data = JsonUtility.FromJson<PlayerData>(json);
                //PlayerData data = JsonConvert.DeserializeObject<PlayerData>(json);
                _playerData = data;
                print("UserId: " + _playerData.UserID);
                print($"Data loaded successfully.");
                OnSuccessfulDataLoad?.Invoke();
            }
            else
            {
                Debug.Log($"No data found for this id: {_userID}, you need to create an offline account");
                OnFailDataLoad?.Invoke();
            }
        });
    }


    [ContextMenu("LoadLeaderboard")]
    public void GetLeaderboard()
    {
        _leaderboardManager.LoadTop10Leaderboard();
    }
}