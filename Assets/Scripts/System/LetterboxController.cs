using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Trava a área visível das câmeras numa proporção fixa (16:9), gerando barras
/// pretas (letterbox em cima/baixo ou pillarbox nas laterais) no restante.
///
/// O letterbox só é ativado em dispositivos com proporção muito diferente do 16:9:
///   - iPads (4:3) e Z Fold aberto (≈1.25:1)  →  aspect &lt; MinAspect (1.55)
///   - Z Fold fechado (≈2.78:1)               →  aspect &gt; MaxAspect (2.40)
///   - Celulares comuns (iPhone, Galaxy, etc.) →  tela cheia, sem barras
///
/// Event-driven: NÃO faz trabalho por-frame. Recalcula o viewport rect apenas em
/// bootstrap, troca de cena, retomada do app e, como rede de segurança para
/// mudanças de orientação, numa checagem de baixa frequência (0.5s).
///
/// Auto-bootstrap via [RuntimeInitializeOnLoadMethod] — não precisa estar em cena.
/// </summary>
[DefaultExecutionOrder(-100)]
public class LetterboxController : MonoBehaviour
{
    /// <summary>Proporção alvo (1920x1080). Ponto único de verdade para mundo e UI.</summary>
    public const float TargetAspect = 16f / 9f;

    /// <summary>
    /// Faixa de aspect ratio considerada "celular comum" — sem letterbox.
    /// Abaixo de MinAspect: iPad/Z Fold aberto → barras cima/baixo.
    /// Acima de MaxAspect: Z Fold fechado     → barras laterais.
    /// </summary>
    public const float MinAspect = 1.55f;
    public const float MaxAspect = 2.40f;

    /// <summary>
    /// Disparado sempre que o viewport rect das câmeras é recalculado
    /// (inclusive quando muda para Rect(0,0,1,1) = tela cheia).
    /// A UI (<see cref="SafeFrameController"/>) assina este evento.
    /// </summary>
    public static event System.Action<Rect> OnRectChanged;

    private static LetterboxController _instance;
    private int _lastW, _lastH;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (_instance != null) return;
        var go = new GameObject("[LetterboxController]");
        _instance = go.AddComponent<LetterboxController>();
        DontDestroyOnLoad(go);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        ApplyToAllCameras();
        StartCoroutine(LowFreqResolutionWatch());
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnApplicationPause(bool paused)
    {
        if (!paused) ApplyToAllCameras();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => ApplyToAllCameras();

    private IEnumerator LowFreqResolutionWatch()
    {
        var wait = new WaitForSecondsRealtime(0.5f); // NÃO é por-frame
        while (true)
        {
            yield return wait;
            if (Screen.width != _lastW || Screen.height != _lastH)
                ApplyToAllCameras();
        }
    }

    private void ApplyToAllCameras()
    {
        _lastW = Screen.width;
        _lastH = Screen.height;

        Rect r = ComputeRect();
        foreach (var cam in Camera.allCameras)
        {
            // Não mexe na câmera de fundo (que pinta as barras de preto).
            if (cam.GetComponent<LetterboxBackground>() != null) continue;
            cam.rect = r;
        }

        OnRectChanged?.Invoke(r);
    }

    /// <summary>
    /// Retângulo (normalizado 0..1) do frame 16:9 centralizado na tela atual.
    /// Retorna Rect(0,0,1,1) quando o aspect ratio está na faixa "celular comum"
    /// (sem letterbox). Estático e sem estado — pode ser chamado por qualquer sistema.
    /// </summary>
    public static Rect ComputeRect()
    {
        if (Screen.height <= 0) return new Rect(0f, 0f, 1f, 1f);

        float windowAspect = (float)Screen.width / Screen.height;

        // Celulares comuns: sem barras, tela cheia.
        if (windowAspect >= MinAspect && windowAspect <= MaxAspect)
            return new Rect(0f, 0f, 1f, 1f);

        float scaleHeight = windowAspect / TargetAspect;

        if (scaleHeight < 1f) // janela mais "alta" que o alvo -> barras em cima/baixo
            return new Rect(0f, (1f - scaleHeight) / 2f, 1f, scaleHeight);

        // janela mais "larga" que o alvo -> barras nas laterais
        float scaleWidth = 1f / scaleHeight;
        return new Rect((1f - scaleWidth) / 2f, 0f, scaleWidth, 1f);
    }
}
