using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FireBaseInitializer : MonoBehaviour
{
    public static bool FirebaseReady = false;
    public static FirebaseDatabase FirebaseDatabase;
    public static string UserID;
    [SerializeField] private string ID;

    [SerializeField] private SceneLoader sceneLoader;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        UserID = ID;

        FirebaseApp app = FirebaseApp.DefaultInstance;

        FirebaseDatabase = FirebaseDatabase.GetInstance(app, "https://chefrog-86c0e-default-rtdb.firebaseio.com/");

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                FirebaseReady = true;
                Debug.Log("Firebase initialized successfully.");
                sceneLoader.LoadScene();
            }
            else
            {
                Debug.LogError($"Error initializing Firebase: {dependencyStatus}");
            }
        });
    }
}