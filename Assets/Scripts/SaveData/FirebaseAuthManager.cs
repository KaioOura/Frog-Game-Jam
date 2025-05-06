using System;
using System.Collections;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

namespace SaveData
{
    public class FirebaseAuthManager : MonoBehaviour
    {
        
        public static bool FirebaseReady = false;
        public static FirebaseDatabase FirebaseDatabase;
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            FirebaseApp app = FirebaseApp.DefaultInstance;
            
            FirebaseDatabase = FirebaseDatabase.GetInstance(app, "https://chefrog-86c0e-default-rtdb.firebaseio.com/");
            
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    FirebaseReady = true;
                    Debug.Log("Firebase inicializado com sucesso.");
                }
                else
                {
                    Debug.LogError($"Erro ao inicializar Firebase: {dependencyStatus}");
                }
            });
        }

        IEnumerator Start()
        {
            yield return new WaitUntil(()=> FirebaseReady == true);
            
            try
            {
                FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.Result == DependencyStatus.Available)
                    {
                        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
                        auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(authTask =>
                        {
                            if (authTask.IsCompleted && !authTask.IsCanceled && !authTask.IsFaulted)
                            {
                                Debug.Log("User signed in: " + auth.CurrentUser.UserId);
                            }
                            else
                            {
                                Debug.LogError("Failed to sign in anonymously.");
                            }
                        });
                    }
                    else
                    {
                        Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
                    }
                });
            }
            catch (Exception e)
            {
                throw; // TODO handle exception
            }
        }
    }
}