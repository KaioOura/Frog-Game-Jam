using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private bool startTutorial = true;
    [SerializeField] private Tutorial tutorialDebug;
    [SerializeField] private TutorialController tutorialController;
    
    private ActionManager _actionManager;

    public void Initialize(TimeScaler timeScaler, ActionManager actionManager)
    {
        tutorialController.Initialize(timeScaler, actionManager.ActionDataBase);

        tutorialController.OnTutorialEnded += UnsubscribeEvents;

        _actionManager = actionManager;
    }

    [ContextMenu("Start Tutorial")]
    public void StartTutorial()
    {
        SubscribeEvents();
        tutorialController.StartTutorial(tutorialDebug);
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


