using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public LookPositions lookPositions;
    public PlayerTongueAction playerTongue;
    
    public float angle;

    public int lookIndex;

    [Header("Sounds")]
    public AudioSource audioSource;
    public AudioClip turnClip;

    private void OnEnable()
    {
        lookPositions = FindObjectOfType<LookPositions>();
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.LookRotation(lookPositions.positions[lookIndex].position);
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        Turn();
    }

    void Turn()
    {
        if (playerTongue.isUsingTongue)
            return;

        //audioSource.PlayOneShot(tongueClip);

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            TurnLeft();
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightAlt))
        {
            TurnRight();
        }
    }

    public void TurnRight()
    {
        lookIndex--;

        lookIndex = lookIndex < 0 ? lookPositions.positions.Count - 1 : lookIndex;

        transform.LookAt(lookPositions.positions[lookIndex].position);
    }

    public void TurnLeft()
    {
        lookIndex++;

        lookIndex = lookIndex > lookPositions.positions.Count - 1 ? 0 : lookIndex;

        transform.LookAt(lookPositions.positions[lookIndex].position);
    }
}
