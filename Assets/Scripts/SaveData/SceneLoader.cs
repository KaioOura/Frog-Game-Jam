using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private bool isAdditive;

    public void LoadScene()
    {
        print($"Loading: {sceneToLoad} Scene");
        
        
        SceneManager.LoadScene(sceneToLoad, isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
    }
    
    public AsyncOperation LoadSceneAsync()
    {
        return SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
    }

    public void ChangeSceneToLoad(string sceneName)
    {
        sceneToLoad = sceneName;
    }

    public void UnloadSceneAsync(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);
    }
}