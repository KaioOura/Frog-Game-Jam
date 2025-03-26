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
    [SerializeField] private float rotationSpeed = 5;

    [Header("Sounds")] public AudioSource audioSource;
    public AudioClip turnClip;

    private Joystick _joystick;
    private GameManager _gameManager;

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

    public void Initialize(GameManager gameManager)
    {
        _gameManager = gameManager;
        _joystick = gameManager.Joystick;
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameManager.gameStates == GameStates.game)
            HandleInput();
    }

    void HandleInput()
    {
        if (playerTongue.isUsingTongue)
            return;

        KeyboardTurn();
        MobileTurn();
    }

    void KeyboardTurn()
    {
        //audioSource.PlayOneShot(tongueClip);

        if (newInput)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                TurnLeft();
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
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
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                TurnRight();
            }
        }
    }

    void MobileTurn()
    {
        Vector2 direction = _joystick.Direction;

        // Verifica se há input suficiente para rotacionar
        if (direction.magnitude > 0.1f)
        {
            // Converte o vetor do canvas para o espaço do mundo
            Vector3 worldDirection = new Vector3(direction.x, 0, direction.y);

            // Calcula a rotação desejada
            Quaternion targetRotation = Quaternion.LookRotation(worldDirection, Vector3.up);

            // Suaviza a rotação do personagem
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
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