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
                FirebaseReady = true;
                sceneLoader.LoadScene();
            });
    }

   
}