using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventChannel", menuName = "Scriptable Objects/EventChannel")]
public class EventChannel : ScriptableObject
{
    private Action listener;
    
    
    public void Register(Action action)
    {
        listener += action;
    }

    public void UnRegister(Action action)
    {
        listener -= action;
    }

    public void RaiseEvent()
    {
        listener?.Invoke();
    }
}
