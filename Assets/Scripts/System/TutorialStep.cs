using System;
using UnityEngine;
using UnityEngine.Serialization;


[Serializable]
public class TutorialStep
{
    public bool ShouldStopTime => shouldStopTime;
    public InGameAction InGameAction => inGameAction;
    public int StartActionCount => _startActionCount;
    
    [SerializeField] private InGameAction inGameAction; 
    [SerializeField] private bool shouldStopTime;

    private int _startActionCount;


    public void SetActionStored(int value)
    {
        _startActionCount = value;
    }
}