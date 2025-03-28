using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Joystick Joystick => joystick;

    public bool isMobile; //TODO: remover isso quando criar um meio de alternar build mobile e web
    [SerializeField] private Joystick joystick;
    [SerializeField] private GameObject actionButton; //TODO: Criar manager de UI
    [SerializeField] private GameObject deliverButton;
    [SerializeField] private Character character;
    [SerializeField] private IngredientSpawner ingredientSpawner;

    public BellyFrog bellyFrog;
    public Animator an;

    
    public GameStates gameStates;

    public int currentScore;
    public int lastScore;
    public int highScore;

    public int lives;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetScore();
        ResetLife();

        //Application.targetFrameRate = 60;

        character.InitializeComponents(this);

        joystick.gameObject.SetActive(isMobile);
        deliverButton.SetActive(isMobile);
        actionButton.SetActive(isMobile);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        gameStates = GameStates.game;
        
        ResetScore();
        ResetLife();
        OrderManager.instance.ResetOrders();

        IngredientScriptable[] ingredients = FindObjectsOfType<IngredientScriptable>();

        foreach (var item in ingredients)
        {
            Destroy(item.gameObject);

        }

        bellyFrog.ResetBellyFrog();

        UIManager.instance.ShowHideMenu(shouldShow: false);

        an.SetTrigger("Game");

        ingredientSpawner.StartIngredientSpawn();
        
        AudioManager.instance.PlayGameMusic();
    }

    public void LoseGame()
    {
        ingredientSpawner.StopIngredientSpawn();
        var rotationVector = new Vector3(0,180,0);
        bellyFrog.gameObject.transform.DORotate(rotationVector, 0.7f, RotateMode.Fast);
        gameStates = GameStates.finish;
        an.SetTrigger("Menu");
        UIManager.instance.ShowHidePostGame(shouldShow: true);

        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetFloat(PlayerPrefsSettings.highScore, highScore);
        }
        else
        {
            highScore = (int)PlayerPrefs.GetFloat(PlayerPrefsSettings.highScore, 0);
        }

        OrderManager.instance.ResetOrders();

        UIManager.instance.UpdateCurrentFinalScore(currentScore);
        UIManager.instance.UpdateCurrentHighScore(highScore);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1;
    }

    public void AddScore(int scoreToAdd)
    {
        currentScore += scoreToAdd;
        UIManager.instance.UpdateCurrentScore(currentScore);
    }

    public void ResetScore()
    {
        AddScore(-currentScore);
    }

    public void ChangeLife(int changeAmount)
    {
        if (changeAmount < 0)
        {
            //Tocar som de dano!
            bellyFrog.PlayHurtSound();
        }

        lives += changeAmount;
        UIManager.instance.UpdateLives(lives);

        if (lives <= 0 && gameStates == GameStates.game)
        {
            LoseGame();

            //Trigar tela de derrota, mostrar score, highscore, etc
        }
    }

    public void ResetLife()
    {
        lives = 6;
        UIManager.instance.UpdateLives(lives);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}

public enum GameStates { menu, pause, game, finish}
