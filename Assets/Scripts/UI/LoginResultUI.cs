using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoginResultUI : MonoBehaviour
{
    [SerializeField] private GameObject offlineCreationVisual;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private SceneLoader sceneLoader;
    private FirebaseDataManager _firebaseDataManager;

    private void Awake()
    {
        StartCoroutine(SubscribeEvents());
    }

    private IEnumerator SubscribeEvents()
    {
        while (_firebaseDataManager == null)
        {
            _firebaseDataManager = FindAnyObjectByType<FirebaseDataManager>();
            yield return null;
        }

        _firebaseDataManager.OnFailDataLoad += OnFailDataLoad;
        _firebaseDataManager.OnSuccessfulDataLoad += OnEnterGame;
    }

    private void OnDestroy()
    {
        _firebaseDataManager.OnFailDataLoad -= OnFailDataLoad;
        _firebaseDataManager.OnSuccessfulDataLoad -= OnEnterGame;
    }
    
    void OnFailDataLoad()
    {
        offlineCreationVisual.SetActive(true);
    }

    void OnEnterGame()
    {
        sceneLoader.LoadScene();
    }

    public void ConfirmOfflineAccountCreation()
    {
        PlayerData newPlayerData = new PlayerData
        {
            UserID = _firebaseDataManager.UserID,
            Username = inputField.text,
            Coins = 0,
            Highschore = 0,
            IsTutorial = true,
            Items = new List<string> { "sword", "potion", "shield" },
            Level = 0,
        };
        // foreach (PowerUpEnum powerUpEnum in Enum.GetValues(typeof(PowerUpEnum)))
        // {
        //     newPlayerData.Upgrades.TryAdd(powerUpEnum, 0);
        // }
        
        _firebaseDataManager.InitialSavePlayerData(newPlayerData, OnEnterGame);
    }
}
