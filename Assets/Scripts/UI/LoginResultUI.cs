using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class LoginResultUI : MonoBehaviour
{
    [SerializeField] private GameObject offlineCreationVisual;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private SceneLoader sceneLoader;
    
    private IEnumerator Start()
    {
        yield return new WaitUntil(() => FirebaseDataManager.Instance);
        FirebaseDataManager.Instance.OnRequestOfflineAccountCreation += OnRequestOfflineAccountCreation;
        FirebaseDataManager.Instance.OnRequestEnterGame += OnEnterGame;
    }

    private void OnDestroy()
    {
        FirebaseDataManager.Instance.OnRequestOfflineAccountCreation -= OnRequestOfflineAccountCreation;
        FirebaseDataManager.Instance.OnRequestEnterGame -= OnEnterGame;
    }
    
    void OnRequestOfflineAccountCreation()
    {
        offlineCreationVisual.SetActive(true);
    }

    void OnEnterGame()
    {
        sceneLoader.LoadScene();
    }

    public void ConfirmOfflineAccountCreation()
    {
        FirebaseDataManager.Instance.PlayerData.Username = inputField.text;
        FirebaseDataManager.Instance.PlayerData.Coins = 0;
        FirebaseDataManager.Instance.PlayerData.Highschore = 999;
        FirebaseDataManager.Instance.SavePlayerData(OnEnterGame);
    }
}
