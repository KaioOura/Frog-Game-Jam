using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;

namespace SaveData
{
    public class FirebaseAuthManager : MonoBehaviour {
        void Start() {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
                if (task.Result == DependencyStatus.Available) {
                    FirebaseAuth auth = FirebaseAuth.DefaultInstance;
                    auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(authTask => {
                        if (authTask.IsCompleted && !authTask.IsCanceled && !authTask.IsFaulted) {
                            Debug.Log("User signed in: " + auth.CurrentUser.UserId);
                        } else {
                            Debug.LogError("Failed to sign in anonymously.");
                        }
                    });
                } else {
                    Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
                }
            });
        }
    }
}
