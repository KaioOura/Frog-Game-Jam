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

    public void Initialize(TimeScaler timeScaler, ActionManager actionManager, PlayerDataHandler playerDataHandler)
    {
        tutorialController.Initialize(timeScaler, actionManager.ActionDataBase);

        tutorialController.OnTutorialEnded += UnsubscribeEvents;
        tutorialController.OnTutorialEnded += () => _playerDataHandler.Tutorial.SetTutorial(false);

        _actionManager = actionManager;
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


