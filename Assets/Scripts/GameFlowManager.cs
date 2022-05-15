using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    public GameObject pausePage;
    bool isPause = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && (GameManager.instance.gameStates == GameManager.GameStates.game || GameManager.instance.gameStates == GameManager.GameStates.pause))
        {
            PauseGame(!isPause);
        }
    }

    public void PauseGame(bool pause)
    {
        isPause = !isPause;
        if (pause)
        {
            GameManager.instance.gameStates = GameManager.GameStates.pause;
            pausePage.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
            GameManager.instance.gameStates = GameManager.GameStates.game;
            pausePage.SetActive(false);
        }
    }

    public void LoadLevelByIndex(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }
}
