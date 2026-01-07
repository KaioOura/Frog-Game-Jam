using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class UIManager : MonoBehaviour
{
    //TODO: Dividir em sistemas separados. Por exemplo: ScoreUI, HealthUI, MenuUI

    [field: SerializeField] public LeaderboardUI LeaderboardUI { get; private set; }
    [field: SerializeField] public MobileInputUI MobileInputUI { get; private set; }
    [field: SerializeField] public BellyDisplayUI BellyDisplayUI { get; private set; }
    [field: SerializeField] public SecondChanceUI SecondChanceUI { get; private set; }
    
    public Image[] lifeImages;

    public TextMeshProUGUI currentScoreTMP, finalScoreTMP;
    public TextMeshProUGUI finalHighScoreTMP;
    [SerializeField] private TextMeshProUGUI secondChanceScoreTMP;
    [SerializeField] private TextMeshProUGUI secondChanceHighScoreTMP;
    
    public GameObject menu, game,postGame, secondChance, pauseUI;
    
    public void UpdateScore(int scoreToUpdate)
    {
        currentScoreTMP.text = scoreToUpdate.ToString();
        finalScoreTMP.text = scoreToUpdate.ToString();
        secondChanceScoreTMP.text = scoreToUpdate.ToString();
    }
    
    public void UpdateHighScore(int scoreToUpdate)
    {
        finalHighScoreTMP.text = scoreToUpdate.ToString();
        secondChanceHighScoreTMP.text = scoreToUpdate.ToString();
    }

    public void UpdateLives(int life)
    {
        for (var index = 0; index < lifeImages.Length; index++)
        {
            var item = lifeImages[index];
            
            item.gameObject.SetActive(index < life);
            
        }
    }

    public void ShowMenu(bool shouldShow)
    {
        if (shouldShow)
        {
            menu.SetActive(true);
            game.SetActive(false);
        }
        else
        {
            menu.SetActive(false);
            game.SetActive(true);
        }
    }

    public void ShowHidePostGame(bool shouldShow)
    {
        if (shouldShow)
        {
            postGame.SetActive(true);
            game.SetActive(false);
        }
        else
        {
            postGame.SetActive(false);
        }
    }
    
    public void ShowSecondChance(bool shouldShow)
    {
        secondChance.SetActive(shouldShow);
    }

    public void ShowPauseInGame(bool shouldShow)
    {
        pauseUI.SetActive(shouldShow);
    }
    
}
