using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;


    public void InitializeComponents(GameManager gameManager)
    {
        playerMovement.Initialize(gameManager);
    }

}
