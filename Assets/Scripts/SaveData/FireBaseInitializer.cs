using System;
using System.Collections;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class FireBaseInitializer : MonoBehaviour
{
    public static bool FirebaseReady = false;
    public static DatabaseReference databaseReference;
    public static string UserID;
    [SerializeField] private string ID;
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private LoaderUI loaderUI;
    [SerializeField] private SceneLoader sceneLoader;
    private ScreenFader _screenFader;

    private void Awake()
    {
        sceneLoader.LoadScene(true);
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        
        _screenFader = FindAnyObjectByType<ScreenFader>();
        
        loaderUI.AddStep("FireBase", "Connecting wires", 1, () => FirebaseReady);
        loaderUI.AddStep("AdManager", "Getting orders", 1, () => AdManager.IsReady);
        loaderUI.Initialize(() => _screenFader.LoadSceneWithFade("Login", SceneManager.GetActiveScene().name));
        
        UserID = ID;
        
        debugText.text = "Trying to initialize...";
        
        FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    
                    debugText.text = "Erro de autenticação: " + task.Exception.Message;
                    Debug.LogError(task.Exception);
                    return;
                }

                debugText.text = "Autenticado com sucesso ";
                //Debug.Log("Usuário autenticado: " + UserID);

                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
                debugText.text = "DatabaseReference Success";
                
                
                CheckDependencies();
            });
    }

    void CheckDependencies()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                debugText.text = "[Firebase] Inicializado com sucesso!";
                GameLogger.Log("[Firebase] Inicializado com sucesso!");
                FirebaseReady = true;
            }
            else
            {
                GameLogger.Error($"[Firebase] Falha nas dependências: {dependencyStatus}");
            }
        });
    }
    
    
    void OnApplicationPause(bool pause)
    {
        if (pause)
            GameAnalyticsManager.Track("player_left_game");
        else
            GameAnalyticsManager.Track("player_returned_game");
    }

    void OnApplicationQuit()
    {
        GameAnalyticsManager.Track("player_quit");
    }
}