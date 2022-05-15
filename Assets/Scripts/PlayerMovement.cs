using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public LookPositions lookPositions;
    public PlayerTongueAction playerTongue;

    public float angle;

    public int lookIndex;

    public bool newInput;

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
        if (!newInput)
            transform.rotation = Quaternion.LookRotation(lookPositions.positions[lookIndex].position);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.gameStates == GameManager.GameStates.game)
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

        if (newInput)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                TurnLeft();
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightAlt))
            {
                TurnRight();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                TurnLeft();
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightAlt))
            {
                TurnRight();
            }
        }

    }

    public void TurnRight()
    {
        if (newInput)
        {
            transform.localEulerAngles += new Vector3(0, angle, 0) * Time.deltaTime;
            return;
        }

        lookIndex--;

        lookIndex = lookIndex < 0 ? lookPositions.positions.Count - 1 : lookIndex;

        transform.LookAt(lookPositions.positions[lookIndex].position);
    }

    public void TurnLeft()
    {

        if (newInput)
        {
            transform.localEulerAngles -= new Vector3(0, angle, 0) * Time.deltaTime;
            return;
        }

        lookIndex++;

        lookIndex = lookIndex > lookPositions.positions.Count - 1 ? 0 : lookIndex;

        transform.LookAt(lookPositions.positions[lookIndex].position);
    }
}
