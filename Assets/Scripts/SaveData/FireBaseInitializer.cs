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
    public static bool FirebaseFailed = false;
    public static DatabaseReference databaseReference;
    public static string UserID;
    [SerializeField] private string ID;
#if UNITY_EDITOR
    [Header("Debug (Editor)")]
    [Tooltip("Força o modo offline (Firebase indisponível) para testar o fallback de convidado/leaderboard-fakes.")]
    [SerializeField] private bool forceOfflineInEditor;
#endif
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
        
        loaderUI.AddStep("FireBase", "Connecting wires", 1, () => FirebaseReady || FirebaseFailed);
        loaderUI.AddStep("AdManager", "Getting orders", 1, () => AdManager.IsReady);
        loaderUI.Initialize(() => _screenFader.LoadSceneWithFade("Login", SceneManager.GetActiveScene().name));
        
        UserID = ID;
        
        debugText.text = "Trying to initialize...";

#if UNITY_EDITOR
        if (forceOfflineInEditor)
        {
            debugText.text = "[Firebase] Modo offline forçado (editor).";
            GameLogger.Log("[Firebase] Modo offline forçado (editor).");
            FirebaseFailed = true;
            return;
        }
#endif

        // Firebase exige verificar/consertar dependências ANTES de usar Auth/Database.
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.Result != DependencyStatus.Available)
            {
                string status = task.IsFaulted
                    ? task.Exception?.Flatten().Message
                    : task.Result.ToString();

                debugText.text = "[Firebase] Falha nas dependências: " + status;
                GameLogger.Error($"[Firebase] Falha nas dependências: {status}");
                FirebaseFailed = true;
                return;
            }

            Authenticate();
        });
    }

    private void Authenticate()
    {
        FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    debugText.text = "Erro de autenticação: " + task.Exception?.Flatten().Message;
                    GameLogger.Error($"[Firebase] Erro de autenticação: {task.Exception}");
                    FirebaseFailed = true;
                    return;
                }

                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
                debugText.text = "[Firebase] Inicializado com sucesso!";
                GameLogger.Log("[Firebase] Inicializado com sucesso!");
                FirebaseReady = true;
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