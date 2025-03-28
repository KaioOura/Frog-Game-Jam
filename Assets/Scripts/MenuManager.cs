using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject controlsPage;
    public GameObject controllsPage;
    public GameObject creditsPage;
    public GameObject exitPage;
    
    public void LoadSceneByIndex(int levelBuildIndex)
    {
        SceneManager.LoadScene(levelBuildIndex);
    }

    public void QuitConfirmation()
    {
        Application.Quit();
    }

}
