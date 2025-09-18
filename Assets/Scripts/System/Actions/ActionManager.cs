using System;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public event Action<string> OnActionPerformed;

    public EventChannelAction EventChannelAction => eventChannelAction;
    public ActionDataBase ActionDataBase => actionDataBase;
    
    [SerializeField] private EventChannelAction eventChannelAction;
    [SerializeField] private ActionDataBase actionDataBase;
    
    void Start()
    {
        eventChannelAction.Register(OnReceiveAction);
    }
    
    private void OnReceiveAction(InGameAction inGameAction)
    {
        actionDataBase.ProcessAction(inGameAction);
        OnActionPerformed?.Invoke(inGameAction.Key);
    }
}
