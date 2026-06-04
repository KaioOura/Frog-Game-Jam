using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla o SafeFrame do Canvas (filho direto com AspectRatioFitter):
///
/// • Quando NÃO há letterbox (celulares comuns): ARF desativado, anchors esticadas
///   (0,0)→(1,1) — SafeFrame preenche o canvas inteiro.
///
/// • Quando HÁ letterbox (iPad / Z Fold): ARF ativado, anchors centralizadas
///   (0.5,0.5) — ARF aplica FitInParent 16:9 corretamente sem conflito de layout.
///
/// Deve estar no mesmo GameObject que o <see cref="AspectRatioFitter"/>.
/// </summary>
[RequireComponent(typeof(AspectRatioFitter))]
[RequireComponent(typeof(RectTransform))]
public class SafeFrameController : MonoBehaviour
{
    AspectRatioFitter _arf;
    RectTransform     _rt;

    void Awake()
    {
        _arf = GetComponent<AspectRatioFitter>();
        _rt  = GetComponent<RectTransform>();
        Apply(LetterboxController.ComputeRect());
    }

    void OnEnable()  => LetterboxController.OnRectChanged += Apply;
    void OnDisable() => LetterboxController.OnRectChanged -= Apply;

    void Apply(Rect r)
    {
        bool hasLetterbox = r != new Rect(0f, 0f, 1f, 1f);

        if (hasLetterbox)
        {
            // ARF FitInParent exige anchors centrais — evita conflito de layout.
            _rt.anchorMin        = new Vector2(0.5f, 0.5f);
            _rt.anchorMax        = new Vector2(0.5f, 0.5f);
            _rt.pivot            = new Vector2(0.5f, 0.5f);
            _rt.anchoredPosition = Vector2.zero;
            _arf.enabled         = true;
        }
        else
        {
            // Sem letterbox: ARF desativado, SafeFrame preenche o canvas.
            _arf.enabled  = false;
            _rt.anchorMin = Vector2.zero;
            _rt.anchorMax = Vector2.one;
            _rt.offsetMin = Vector2.zero;
            _rt.offsetMax = Vector2.zero;
        }
    }
}
