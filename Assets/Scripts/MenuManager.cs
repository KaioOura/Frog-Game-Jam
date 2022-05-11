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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadSceneByIndex(int levelBuildIndex)
    {
        SceneManager.LoadScene(levelBuildIndex);
    }

    public void QuitConfirmation()
    {
        Application.Quit();
    }

}
