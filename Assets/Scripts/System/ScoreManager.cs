using System;
using SaveData;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public Action<int> OnScoreChanged; 
    public Action<int> OnHighScoreChanged;
    
    public int currentScore;
    public int lastScore;
    
    private PlayerDataHandler _playerDataHandler;

    public void Initialize(PlayerDataHandler playerDataHandler, UIManager uIManager)
    {
        _playerDataHandler = playerDataHandler;
        OnScoreChanged += uIManager.UpdateScore;
        OnHighScoreChanged += uIManager.UpdateHighScore;
    }
    
    public void AddScore(int scoreToAdd)
    {
        currentScore += scoreToAdd;
        UpdateScore();
    }

    public void UpdateScore()
    {
        OnScoreChanged?.Invoke(currentScore);
    }
    
    public void ResetScore()
    {
        AddScore(-currentScore);
    }

    public void CalculateFinalScore()
    {
        CheckScore();
        
        OnScoreChanged?.Invoke(currentScore);
        OnHighScoreChanged?.Invoke( _playerDataHandler.Progress.GetHighScore());
        
        SaveScore();
    }

    private void CheckScore()
    {
        int highScore = _playerDataHandler.Progress.GetHighScore();
        
        if (currentScore > highScore)
            _playerDataHandler.Progress.SetHighScore(currentScore);
    }
    
    private void SaveScore()
    {
        int highScore = _playerDataHandler.Progress.GetHighScore();
        PlayerPrefs.SetFloat(PlayerPrefsSettings.highScore, highScore);
    }
}
