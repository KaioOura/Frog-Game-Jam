using System;
using System.Collections;
using System.Collections.Generic;
using SaveData;
using UnityEngine;
using UnityEngine.Serialization;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private bool startTutorial = true;
    [SerializeField] private Tutorial tutorialDebug;
    [SerializeField] private TutorialController tutorialController;
    
    private ActionManager _actionManager;
    private PlayerDataHandler _playerDataHandler;

    public void Initialize(GameManager gameManager, PlayerDataHandler playerDataHandler)
    {
        tutorialController.Initialize(gameManager.TimeScaler, gameManager.ActionManager.ActionDataBase);

        tutorialController.OnTutorialEnded += UnsubscribeEvents;
        tutorialController.OnTutorialEnded += gameManager.OnTutorialEnded;

        _actionManager = gameManager.ActionManager;
        _playerDataHandler = playerDataHandler;
    }
    
    private void StartTutorial()
    {
        SubscribeEvents();
        tutorialController.StartTutorial(tutorialDebug);
    }
    
    [ContextMenu("Start Tutorial")]
    public void TryStartTutorial()
    {
        StartCoroutine(TutorialCoroutine());
    }
    
    private IEnumerator TutorialCoroutine()
    {
        if (!_playerDataHandler.Tutorial.GetTutorial())
        {
            Debug.Log("Tutorial already played, continuing to game");
            yield break;
        }
        
        yield return new WaitForSeconds(2f);
        StartTutorial();
    }
    
    private void SubscribeEvents()
    {
        _actionManager.OnActionPerformed += tutorialController.CheckCurrentTutorialStep;
    }

    private void UnsubscribeEvents()
    {
        _actionManager.OnActionPerformed -= tutorialController.CheckCurrentTutorialStep;
    }

    
}


