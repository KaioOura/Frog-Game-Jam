using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

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

        if (lives <= 0)
        {
            gameStates = GameStates.finish;

            //Trigar tela de derrota, mostrar score, highscore, etc
        }
    }

    public void ResetLife()
    {
        lives = 3;
        UIManager.instance.UpdateLives(lives);
    }

}