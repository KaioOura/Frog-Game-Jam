using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public Image[] lifeImages;

    public Image bellyFrogImage;

    public TextMeshProUGUI currentScoreTMP, finalScoreTMP;
    public TextMeshProUGUI finalHighScoreTMP;
    [SerializeField] private TextMeshProUGUI secondChanceScoreTMP;
    [SerializeField] private TextMeshProUGUI secondChanceHighScoreTMP;


    public GameObject menu, game,postGame, secondChance;

    private void Awake()
    {
        instance = this;
    }

    public void UpdateCurrentScore(int scoreToUpdate)
    {
        currentScoreTMP.text = scoreToUpdate.ToString();
    }

    public void UpdateCurrentFinalScore(int scoreToUpdate)
    {
        finalScoreTMP.text = scoreToUpdate.ToString();
        secondChanceScoreTMP.text = scoreToUpdate.ToString();
    }

    public void UpdateCurrentHighScore(int scoreToUpdate)
    {
        finalHighScoreTMP.text = scoreToUpdate.ToString();
        secondChanceHighScoreTMP.text = scoreToUpdate.ToString();
    }

    public void UpdateLives(int life)
    {
        int lifeImagesActive = 0;
        
        for (var index = 0; index < lifeImages.Length; index++)
        {
            var item = lifeImages[index];

            item.gameObject.SetActive(index < life);
            
        }
    }

    public void ShowHideMenu(bool shouldShow)
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
        if (shouldShow)
        {
            secondChance.SetActive(true);
        }
        else
        {
            secondChance.SetActive(false);
        }
    }

    public void UpdateBellyFrog(float value, float maxValue)
    {
        bellyFrogImage.fillAmount = value / maxValue;
    }
    
}
