using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private bool startTutorial = true;
    [SerializeField] private List<TutorialStep> tutorialSteps = new List<TutorialStep>();
    [SerializeField] private EventChannelTutorialAction eventChannelTutorialAction;
    
    private int _currentTutorialStep;

    private void Start()
    {
        SubscribeEvents();
    }

    public void SubscribeEvents()
    {
        eventChannelTutorialAction.Register(OnReceiveTutorialAction);
    }

    public void UnsubscribeEvents()
    {
        eventChannelTutorialAction.Register(OnReceiveTutorialAction);
    }
    
    public void OnReceiveTutorialAction(TutorialAction tutorialAction)
    {
        CheckCurrentTutorialStep(tutorialAction);
    }

    private void CheckCurrentTutorialStep(TutorialAction tutorialAction) //TODO: Criar um TutorialController para controlar os diferentes estados do tutorial, pausar, trigar UI, etc
    {
        if (tutorialSteps[_currentTutorialStep].TutorialAction != tutorialAction) return;
        
        _currentTutorialStep++;

        if (_currentTutorialStep >= tutorialSteps.Count)
        {
            //Tutorial finalizado
            UnsubscribeEvents();
            Debug.Log("Tutorial finished");
            return;
        }
        
        Debug.Log("Step completed");
    }
}

public enum TutorialAction
{
    ScreenTouch,
    MoveJoystick,
    LaunchTongue,
    ThrowUp,
    DeliveryMeal
}
