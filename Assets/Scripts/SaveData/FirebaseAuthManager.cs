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
        public static bool LoggedIn = false;
        
        public void Start()
        {
            //StartCoroutine(AnonymouslyRoutine());
        }
        
        IEnumerator AnonymouslyRoutine()
        {
            yield return new WaitUntil(()=> FireBaseInitializer.FirebaseReady);
            
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
                                LoggedIn = true;
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