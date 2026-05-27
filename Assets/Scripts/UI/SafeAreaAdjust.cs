using UnityEngine;

/// <summary>
/// Ajusta um painel à safe-area, mas ciente do letterbox: como o conteúdo agora
/// vive dentro do frame 16:9 (com barras pretas), o notch/recorte que cai sobre
/// as barras NÃO gera inset. O inset só ocorre na borda onde o frame encosta na
/// borda física da tela (eixo sem barra), e apenas pela parte do safe-area que
/// realmente invade o frame.
///
/// Premissa: o pai imediato deste painel preenche o frame 16:9 (ex.: é o
/// SafeFrame, ou um filho esticado dele). As âncoras são calculadas relativas a
/// esse frame.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class SafeAreaAdjust : MonoBehaviour
{
    RectTransform panel;
    Rect lastSafeArea = new Rect(0, 0, 0, 0);
    int lastW, lastH;

    void Awake()
    {
        panel = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    void Update()
    {
        if (lastSafeArea != Screen.safeArea || lastW != Screen.width || lastH != Screen.height)
            ApplySafeArea();
    }

    void ApplySafeArea()
    {
        lastSafeArea = Screen.safeArea;
        lastW = Screen.width;
        lastH = Screen.height;

        float sw = Screen.width;
        float sh = Screen.height;
        if (sw <= 0f || sh <= 0f) return;

        // Frame 16:9 (letterbox) em pixels, centralizado na tela.
        Rect frameN = LetterboxController.ComputeRect();
        float frameXmin = frameN.x * sw;
        float frameYmin = frameN.y * sh;
        float frameW = frameN.width * sw;
        float frameH = frameN.height * sh;
        if (frameW <= 0f || frameH <= 0f) return;
        float frameXmax = frameXmin + frameW;
        float frameYmax = frameYmin + frameH;

        // Interseção entre safe-area e o frame: onde a barra cobre o notch, a
        // interseção bate na borda do frame -> inset 0 naquele eixo.
        Rect sa = Screen.safeArea;
        float effXmin = Mathf.Max(frameXmin, sa.x);
        float effYmin = Mathf.Max(frameYmin, sa.y);
        float effXmax = Mathf.Min(frameXmax, sa.x + sa.width);
        float effYmax = Mathf.Min(frameYmax, sa.y + sa.height);

        // Âncoras normalizadas relativas ao frame (== pai SafeFrame).
        Vector2 anchorMin = new Vector2(
            (effXmin - frameXmin) / frameW,
            (effYmin - frameYmin) / frameH);
        Vector2 anchorMax = new Vector2(
            (effXmax - frameXmin) / frameW,
            (effYmax - frameYmin) / frameH);

        panel.anchorMin = anchorMin;
        panel.anchorMax = anchorMax;
    }
}
