using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    public GameObject pausePage;
    bool isPause = false;
    
    private TimeScaler _timeScaler;
    
    public void Initialize(TimeScaler timeScaler)
    {
        _timeScaler = timeScaler;
    }
    
    // Update is called once per frame
    // void Update()
    // {
    //     //TODO: Inscrever em algum evento
    //     if (Input.GetKeyDown(KeyCode.Escape) && (GameManager.instance.gameStates == GameStates.game || GameManager.instance.gameStates == GameStates.pause))
    //     {
    //         PauseGame(!isPause);
    //     }
    // }

    public void PauseGame(bool pause)
    {
        isPause = pause;
        
        if (pause)
        {
            GameManager.instance.gameStates = GameStates.pause;
            //pausePage.SetActive(true);
            _timeScaler.ShouldStopTime(true);
        }
        else
        {
            _timeScaler.ShouldStopTime(false);
            GameManager.instance.gameStates = GameStates.game;
            //pausePage.SetActive(false);
        }
    }

    public void LoadLevelByIndex(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }
}
