using System;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public event Action OnTutorialEnded;
    
    private Tutorial _tutorial;
    private TimeScaler _timeScaler;
    
    private int _currentTutorialStep;
    private ActionDataBase _actionDataBase;
    
    public void Initialize(TimeScaler timeScaler, ActionDataBase actionDataBase)
    {
        _timeScaler = timeScaler;
        _actionDataBase = actionDataBase;
    }

    public void StartTutorial(Tutorial tutorial)
    {
        _tutorial = tutorial;
        
        StartTutorialStep();
    }
    private void StartTutorialStep(int index = 0)
    {
        _currentTutorialStep = index;
        //Guardar no tutorial step o valor da action armazenada
        int startValueStored =
            _actionDataBase.GetActionAmount(_tutorial.TutorialSteps[_currentTutorialStep].InGameAction.Key);
        _tutorial.TutorialSteps[_currentTutorialStep].SetActionStored(startValueStored);
        ApplyTutorialActions(_tutorial.TutorialSteps[_currentTutorialStep]); 
    }

    private void ApplyTutorialActions(TutorialStep tutorialStep)
    {
        _timeScaler.ShouldStopTime(tutorialStep.ShouldStopTime);
    }
    
    public void CheckCurrentTutorialStep(string inGameActionKey)
    {
        if (_tutorial.TutorialSteps[_currentTutorialStep].InGameAction.Key != inGameActionKey) return;

        int finaValue = _actionDataBase.GetActionAmount(inGameActionKey) -
                        _tutorial.TutorialSteps[_currentTutorialStep].StartActionCount;
        
        bool hasReachedCount = finaValue >= _tutorial.TutorialSteps[_currentTutorialStep].InGameAction.Amount;
           

        if (!hasReachedCount)
        {
            print($"{inGameActionKey}_progress");
            return;
        }
        
        _currentTutorialStep++;

        if (_currentTutorialStep >= _tutorial.GetStepsCount())
        {
            //Tutorial finalizado
            OnTutorialEnded?.Invoke();
            Debug.Log("Tutorial finished");
            return;
        }
        
        Debug.Log("Step completed");
        StartTutorialStep(_currentTutorialStep);
    }
    

}
