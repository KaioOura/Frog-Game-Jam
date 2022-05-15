using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public BellyFrog bellyFrog;
    public Animator an;

    public enum GameStates { menu, pause, game, finish}
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

        IngredientScriptable[] ingredients = FindObjectsOfType<IngredientScriptable>();

        foreach (var item in ingredients)
        {
            Destroy(item.gameObject);
        }

        OrderManager.instance.ResetOrders();

        bellyFrog.ResetBellyFrog();

        UIManager.instance.ShowHideMenu(shouldShow: false);

        an.SetTrigger("Game");

        AudioManager.instance.PlayGameMusic();
    }

    public void LoseGame()
    {
        gameStates = GameStates.finish;
        an.SetTrigger("Menu");
        UIManager.instance.ShowHidePostGame(shouldShow: true);

        if (currentScore > highScore)
        {
            highScore = currentScore;
        }

        UIManager.instance.UpdateCurrentFinalScore(currentScore);
        UIManager.instance.UpdateCurrentHighScore(highScore);
    }

    public void GoToMenu()
    {
        AudioManager.instance.PlayMenuMusic();
        UIManager.instance.ShowHideMenu(shouldShow: true);
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
        lives = 3;
        UIManager.instance.UpdateLives(lives);
    }

}
