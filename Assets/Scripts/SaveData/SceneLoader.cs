using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;

    public void LoadScene()
    {
        print($"Loading: {sceneToLoad} Scene");
        
        SceneManager.LoadScene(sceneToLoad);
    }
}