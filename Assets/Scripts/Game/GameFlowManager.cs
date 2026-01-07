using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    private TimeScaler _timeScaler;
    
    public void Initialize(TimeScaler timeScaler)
    {
        _timeScaler = timeScaler;
    }
    
    public void PauseGame(bool pause)
    {
        if (pause)
        {
            GameManager.instance.gameStates = GameStates.pause;
            _timeScaler.ShouldStopTime(true);
        }
        else
        {
            _timeScaler.ShouldStopTime(false);
            GameManager.instance.gameStates = GameStates.game;
        }
    }

    public void LoadLevelByIndex(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }
}
