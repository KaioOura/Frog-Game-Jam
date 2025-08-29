using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EventChannelTutorialAction", menuName = "Scriptable Objects/EventChannelTutorialAction")]
public class EventChannelType<T> : EventChannel
{
    private event Action<T> listeners;

    public void RaiseEvent(T value)
    {
        listeners?.Invoke(value);
    }

    public void Register(Action<T> listener)
    {
        listeners += listener;
    }

    public void Unregister(Action<T> listener)
    {
        listeners -= listener;
    }
}
