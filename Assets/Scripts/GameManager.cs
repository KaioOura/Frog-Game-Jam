using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;

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
    [SerializeField] private RenderPipelineAsset[] qualityLevels;
    [SerializeField] private GameObject cameraUI;

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
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
 Debug.unityLogger.logEnabled = false;
#endif

        ResetScore();
        ResetLife();

        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        character.InitializeComponents(this);

        joystick.gameObject.SetActive(false);
        deliverButton.SetActive(false);
        actionButton.SetActive(false);
    }

    public void ChangeQuality(int value)
    {
        QualitySettings.SetQualityLevel(value);
        QualitySettings.renderPipeline = qualityLevels[value];
    }

    public void StartGame()
    {
        gameStates = GameStates.game;
        cameraUI.SetActive(false);

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

        if (isMobile)
        {
            joystick.gameObject.SetActive(true);
            deliverButton.SetActive(true);
            actionButton.SetActive(true);
        }
    }

    public void LoseGame()
    {
        cameraUI.SetActive(true);
        ingredientSpawner.StopIngredientSpawn();
        var rotationVector = new Vector3(0, 180, 0);
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

        joystick.gameObject.SetActive(false);
        deliverButton.SetActive(false);
        actionButton.SetActive(false);
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

public enum GameStates
{
    menu,
    pause,
    game,
    finish
}