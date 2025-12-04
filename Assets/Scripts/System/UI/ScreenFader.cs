using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private SceneLoader sceneLoader;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    public IEnumerator FadeIn(Action onComplete = null)
    {
        canvasGroup.blocksRaycasts = true;
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1;

        onComplete?.Invoke();
    }

    public IEnumerator FadeOut(Action onComplete = null)
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            yield return null;
        }

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0;

        onComplete?.Invoke();
    }

    public void LoadSceneWithFade(string sceneName, string sceneToUnload)
    {
        StartCoroutine(LoadSceneWithFadeRoutine(sceneName, sceneToUnload));
    }
    
    public IEnumerator LoadSceneWithFadeRoutine(string sceneName, string sceneToUnload)
    {
        // 1. Fade IN (vai até preto)
        yield return StartCoroutine(FadeIn());

        // 2. Carregar cena em background
        sceneLoader.ChangeSceneToLoad(sceneName);
        AsyncOperation asyncLoad = sceneLoader.LoadSceneAsync();

        // Impede a cena de ativar sozinha antes do fade
        asyncLoad.allowSceneActivation = false;

        // 3. Esperar carregar 90% (Unity trava em 0.9f antes de ativar)
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // 4. Agora ativamos a cena carregada
        asyncLoad.allowSceneActivation = true;

        if (sceneToUnload != null)
            sceneLoader.UnloadSceneAsync(sceneToUnload);
        
        // Espera até ativar completamente
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        yield return new WaitForSeconds(1);
        
        // 5. Agora que a cena já terminou de carregar → Fade OUT
        yield return StartCoroutine(FadeOut());
    }
}
