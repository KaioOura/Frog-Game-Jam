using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public Image[] lifeImages;

    public TextMeshProUGUI currentScoreTMP, finalScoreTMP, highScoreTMP;

    public GameObject menu, postGame;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateCurrentScore(int scoreToUpdate)
    {
        currentScoreTMP.text = scoreToUpdate.ToString();
    }

    public void UpdateCurrentFinalScore(int scoreToUpdate)
    {
        finalScoreTMP.text = scoreToUpdate.ToString();
    }

    public void UpdateCurrentHighScore(int scoreToUpdate)
    {
        highScoreTMP.text = scoreToUpdate.ToString();
    }

    public void UpdateLives(int life)
    {
        int lifeImagesActive = 0;

        foreach (var item in lifeImages)
        {
            if (item.gameObject.activeSelf)
            {
                lifeImagesActive++;
            }
        }

        if (lifeImagesActive > life)
        {
            foreach (var item in lifeImages)
            {
                item.gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < life; i++)
        {
            lifeImages[i].gameObject.SetActive(true);
        }

    }

    public void ShowHideMenu(bool shouldShow)
    {
        if (shouldShow)
        {
            menu.SetActive(true);
        }
        else
        {
            menu.SetActive(false);
        }
    }

    public void ShowHidePostGame(bool shouldShow)
    {
        if (shouldShow)
        {
            postGame.SetActive(true);
        }
        else
        {
            postGame.SetActive(false);
        }
    }
}
