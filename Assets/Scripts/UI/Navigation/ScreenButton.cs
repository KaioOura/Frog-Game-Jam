using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Coloque este componente em qualquer Button para que ele abra uma tela.
/// Basta escolher a tela alvo no dropdown (ou marcar "goBack" para virar botão de voltar).
/// Ele se conecta sozinho ao onClick — não precisa arrastar referências nem configurar o evento.
/// </summary>
[RequireComponent(typeof(Button))]
public class ScreenButton : MonoBehaviour
{
    [Tooltip("Se marcado, o botão volta para a tela anterior (ignora 'target').")]
    [SerializeField] private bool goBack;

    [Tooltip("Tela que este botão abre (quando 'goBack' está desmarcado).")]
    [SerializeField] private ScreenId target;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (ScreenManager.Instance == null)
        {
            Debug.LogWarning("[ScreenButton] Nenhum ScreenManager na cena.", this);
            return;
        }

        if (goBack)
            ScreenManager.Instance.Back();
        else
            ScreenManager.Instance.Open(target);
    }
}
