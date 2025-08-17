using System;
using UnityEngine;

public class Character : MonoBehaviour
{
    public CharState CharState => charState;
    public Health Health => health;

    [SerializeField] private Health health;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerTongueAction playerTongueAction;
    [SerializeField] private BellyFrog bellyFrog;
    
    private CharState charState;


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
        playerMovement.Initialize(gameManager, charState);
        bellyFrog.Initialize(gameManager, health);
        health.OnDeath += gameManager.OnDie;
        health.OnUpdateHealth += gameManager.UIManager.UpdateLives;
        gameManager.OrderManager.OnOrderExpired += health.TakeDamage;
    }

    public void ChangeState(CharState newState)
    {
        charState = newState;
    }
    
}

public enum CharState
{
    Idle,
    UsingTongue
}
