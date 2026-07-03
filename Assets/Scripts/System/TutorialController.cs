using System;
using System.Collections;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public event Action OnTutorialEnded;

    private Tutorial _tutorial;
    private TimeScaler _timeScaler;

    private int _currentTutorialStep;
    private ActionDataBase _actionDataBase;
    private bool _canAdvance;
    
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
        ApplyTutorialActions(_tutorial.TutorialSteps[_currentTutorialStep]);
    }

    private void ApplyTutorialActions(TutorialStep tutorialStep)
    {
        _timeScaler.ShouldStopTime(tutorialStep.ShouldStopTime);

        if (tutorialStep.SpotLightFade != null)
            tutorialStep.SpotLightFade.gameObject.SetActive(true);

        if (tutorialStep.TutorialText != null)
            tutorialStep.TutorialText.gameObject.SetActive(true);

        // Só permite avançar depois que o estágio terminou de aparecer (fade-in).
        _canAdvance = false;
        StartCoroutine(RevealRoutine(tutorialStep));
    }

    private IEnumerator RevealRoutine(TutorialStep tutorialStep)
    {
        yield return tutorialStep.PlayAppear();

        // Baseline de contagem só é capturado após o estágio aparecer, para que
        // ações feitas durante o fade não contem para o avanço.
        tutorialStep.SetActionStored(_actionDataBase.GetActionAmount(tutorialStep.InGameAction.Key));
        _canAdvance = true;
    }

    private void ClearTutorialStep(TutorialStep tutorialStep)
    {
        _timeScaler.ShouldStopTime(false);
        
        if (tutorialStep.SpotLightFade != null)
            tutorialStep.SpotLightFade.gameObject.SetActive(false);
        
        if (tutorialStep.TutorialText != null)
            tutorialStep.TutorialText.gameObject.SetActive(false);
    }
    
    public void CheckCurrentTutorialStep(InGameAction inGameAction)
    {
        if (!_canAdvance) return;

        if (!_tutorial.TutorialSteps[_currentTutorialStep].InGameAction.CheckKey(inGameAction)) return;

        int finaValue = _actionDataBase.GetActionAmount(inGameAction.Key) -
                        _tutorial.TutorialSteps[_currentTutorialStep].StartActionCount;
        
        bool hasReachedCount = finaValue >= _tutorial.TutorialSteps[_currentTutorialStep].InGameAction.Amount;
           

        if (!hasReachedCount)
        {
            print($"{inGameAction}_progress");
            return;
        }
        
        ClearTutorialStep(_tutorial.TutorialSteps[_currentTutorialStep]);
        
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
