using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TutorialStep : MonoBehaviour
{
    public bool ShouldStopTime => shouldStopTime;
    public InGameAction InGameAction => inGameAction;
    public int StartActionCount => _startActionCount;
    public RectTransform SpotLightFade => spotLightFade;
    public TextMeshProUGUI TutorialText => tutorialText;
    public float AppearDuration => appearDuration;

    [SerializeField] private InGameAction inGameAction;
    [SerializeField] private bool shouldStopTime;
    [SerializeField] private RectTransform spotLightFade;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private EventChannelAction eventChannelAction;
    [SerializeField] private float appearDuration = 0.4f;

    private int _startActionCount;
    private CanvasGroup _canvasGroup;

    private CanvasGroup CanvasGroup
    {
        get
        {
            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
                if (_canvasGroup == null)
                    _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            return _canvasGroup;
        }
    }

    // Fade-in do estágio. Usa unscaledDeltaTime porque passos com shouldStopTime
    // deixam Time.timeScale = 0, o que congelaria um fade baseado em deltaTime.
    public IEnumerator PlayAppear()
    {
        CanvasGroup canvasGroup = CanvasGroup;
        canvasGroup.alpha = 0f;

        float t = 0f;
        while (t < appearDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Clamp01(t / appearDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    public void SetActionStored(int value)
    {
        _startActionCount = value;
    }

    public void OnScreenTouch()
    {
        eventChannelAction.RaiseEvent(new InGameAction(GameAction.ScreenTouch, 1));
    }
}