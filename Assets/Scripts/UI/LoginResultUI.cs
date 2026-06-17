using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginResultUI : MonoBehaviour
{
    private const int MaxUsernameLength = 24;

    [SerializeField] private GameObject offlineCreationVisual;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private SceneLoader sceneLoader;


    private ScreenFader _screenFader;
    private FirebaseDataManager _firebaseDataManager;

    private void Awake()
    {
        _screenFader = FindAnyObjectByType<ScreenFader>();
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
        if (_firebaseDataManager == null) return;

        _firebaseDataManager.OnFailDataLoad -= OnFailDataLoad;
        _firebaseDataManager.OnSuccessfulDataLoad -= OnEnterGame;
    }
    
    void OnFailDataLoad()
    {
        offlineCreationVisual.SetActive(true);
    }

    void OnEnterGame()
    {
        _screenFader.LoadSceneWithFade("Gameplay", SceneManager.GetActiveScene().name);
    }

    public void ConfirmOfflineAccountCreation()
    {
        string username = inputField.text.Trim();

        if (string.IsNullOrEmpty(username))
        {
            ShowFeedback("Digite um nome de usuário.");
            return;
        }

        if (username.Length > MaxUsernameLength)
        {
            ShowFeedback($"O nome deve ter no máximo {MaxUsernameLength} caracteres.");
            return;
        }

        PlayerData newPlayerData = new PlayerData
        {
            UserID = _firebaseDataManager.UserID,
            Username = username,
            Coins = 0,
            Highschore = 0,
            IsTutorial = true,
            Items = new List<string> { "sword", "potion", "shield" },
            Level = 0,
        };

        _firebaseDataManager.InitialSavePlayerData(newPlayerData, OnEnterGame, OnOfflineSaveFailed);
    }

    private void OnOfflineSaveFailed()
    {
        ShowFeedback("Não foi possível salvar. Verifique sua conexão e tente novamente.");
    }

    private void ShowFeedback(string message)
    {
        if (feedbackText != null)
            feedbackText.text = message;

        Debug.LogWarning("[Login] " + message);
    }
}
