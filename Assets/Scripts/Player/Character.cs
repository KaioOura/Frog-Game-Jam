using System;
using UnityEngine;

public class Character : MonoBehaviour
{
    public CharState CharState => charState;
    
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
       bellyFrog.Initialize(gameManager);
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
