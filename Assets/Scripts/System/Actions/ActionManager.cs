using System;
using SaveData;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public event Action<InGameAction> OnActionPerformed;

    public EventChannelAction EventChannelAction => eventChannelAction;
    public ActionDataBase ActionDataBase => actionDataBase;
    
    [SerializeField] private EventChannelAction eventChannelAction;
    [SerializeField] private ActionDataBase actionDataBase;
    
    private PlayerDataHandler _playerDataHandler;
    
    void Start()
    {
        eventChannelAction.Register(OnReceiveAction);
    }

    public void Initialize(PlayerDataHandler playerDataHandler)
    {
        _playerDataHandler = playerDataHandler;
    }
    
    private void OnReceiveAction(InGameAction inGameAction)
    {
        actionDataBase.ProcessAction(inGameAction);
        OnActionPerformed?.Invoke(inGameAction);
    }

    public void OnOrderSpawned(MealSo mealSo)
    {
        if (_playerDataHandler.Tutorial.GetTutorial())
        {
            eventChannelAction.RaiseEvent(new InGameAction(GameAction.SpawnOrder, 1));
        }
    }
}
