using System.Collections;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FireBaseInitializer : MonoBehaviour
{
    public static bool FirebaseReady = false;
    public static DatabaseReference databaseReference;
    public static string UserID;
    [SerializeField] private string ID;

    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private TextMeshProUGUI debugText;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
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
                StartCoroutine(AwaitInitialization());
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
    
    private IEnumerator AwaitInitialization()
    {
        yield return new WaitUntil(() => FirebaseReady);
        yield return new WaitUntil(() => AdManager.IsReady);
        yield return new WaitForSeconds(1);
        
        sceneLoader.LoadScene();
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