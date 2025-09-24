using System;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Health Health => health;
    public BellyFrog BellyFrog => bellyFrog;
    public PlayerTongueAction PlayerTongueAction => playerTongueAction;

    [SerializeField] private Health health;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerTongueAction playerTongueAction;
    [SerializeField] private BellyFrog bellyFrog;
    [SerializeField] private CharacterState characterStateState;


    public void Start()
    {
        SubscribeComponents();
    }

    void SubscribeComponents()
    {
        playerTongueAction.OnRequestStateChage += ChangeState;
    }
    
    public void InitializeComponents(GameManager gameManager)
    {
        playerMovement.Initialize(gameManager, gameManager.UIManager.MobileInputUI, characterStateState);
        bellyFrog.Initialize(gameManager, health);
        health.OnDeath += gameManager.OnDie;
        health.OnUpdateHealth += gameManager.UIManager.UpdateLives;
        gameManager.OrderManager.OnOrderExpired += health.TakeDamage;
    }

    public void ChangeState(CharState newState)
    {
        characterStateState.ChangeState(newState);
    }
    
}

public enum CharState
{
    Idle,
    UsingTongue
}
