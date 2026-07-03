using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Componente colocado no GameObject raiz de cada painel/tela.
/// Declara seu id e camada (Root/Overlay) e expõe os hooks onOpen/onClose,
/// onde se pluga (no inspector) qualquer efeito de estado do jogo — por exemplo,
/// a tela de Pause pluga onOpen -> GameFlowManager.PauseGame(true) e
/// onClose -> GameFlowManager.PauseGame(false). Assim a navegação fica separada
/// do estado do jogo, mas o efeito ainda dispara quando a tela certa abre/fecha.
/// </summary>
public class UIScreen : MonoBehaviour
{
    [SerializeField] private ScreenId id;
    [SerializeField] private ScreenLayer layer = ScreenLayer.Root;

    [Space]
    [SerializeField] private UnityEvent onOpen;
    [SerializeField] private UnityEvent onClose;

    public ScreenId Id => id;
    public ScreenLayer Layer => layer;

    public void Open()
    {
        gameObject.SetActive(true);
        onOpen?.Invoke();
    }

    public void Close()
    {
        onClose?.Invoke();
        gameObject.SetActive(false);
    }
}
