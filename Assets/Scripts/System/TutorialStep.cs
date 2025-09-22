using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TutorialStep : MonoBehaviour
{
    public bool ShouldStopTime => shouldStopTime;
    public InGameAction InGameAction => inGameAction;
    public int StartActionCount => _startActionCount;
    public RectTransform SpotLightFade => spotLightFade;
    public TextMeshProUGUI TutorialText => tutorialText;
    
    [SerializeField] private InGameAction inGameAction; 
    [SerializeField] private bool shouldStopTime;
    [SerializeField] private RectTransform spotLightFade;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private EventChannelAction eventChannelAction;

    private int _startActionCount;
    
    public void SetActionStored(int value)
    {
        _startActionCount = value;
    }

    public void OnScreenTouch()
    {
        eventChannelAction.RaiseEvent(new InGameAction(GameAction.ScreenTouch, 1));
    }
}