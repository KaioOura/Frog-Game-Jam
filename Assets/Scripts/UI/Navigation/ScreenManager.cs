using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gerencia qual tela está visível e o histórico de navegação (pilha de overlays).
/// Pode ficar em qualquer GameObject da cena (ex.: um objeto "Managers"): ele encontra
/// todas as UIScreen da cena — mesmo inativas e espalhadas por vários canvases.
/// Botões abrem telas via ScreenButton -> Open(id) e voltam via Back().
/// </summary>
public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance { get; private set; }

    [SerializeField] private ScreenId initialScreen = ScreenId.Menu;

    private readonly Dictionary<ScreenId, UIScreen> _screens = new();
    private readonly Stack<UIScreen> _overlayStack = new();
    private UIScreen _currentRoot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[ScreenManager] Já existe uma instância. Ignorando {name}.", this);
            return;
        }

        Instance = this;
        RegisterScreens();
    }

    private void Start()
    {
        // Estado limpo: tudo fechado, abre só a tela inicial (Root).
        foreach (var screen in _screens.Values)
            screen.gameObject.SetActive(false);

        _currentRoot = null;
        _overlayStack.Clear();
        Open(initialScreen);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void RegisterScreens()
    {
        _screens.Clear();

        // Encontra todas as telas da cena (inclusive inativas), em qualquer canvas.
        var screens = FindObjectsByType<UIScreen>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var screen in screens)
        {
            if (_screens.ContainsKey(screen.Id))
            {
                Debug.LogWarning($"[ScreenManager] ScreenId duplicado: {screen.Id} em {screen.name}. Ignorando.", screen);
                continue;
            }

            _screens.Add(screen.Id, screen);
        }
    }

    /// <summary>
    /// Abre uma tela. Root troca a tela principal (fechando a anterior e limpando overlays);
    /// Overlay abre por cima da Root atual e entra na pilha de histórico.
    /// </summary>
    public void Open(ScreenId id)
    {
        if (!_screens.TryGetValue(id, out var target))
        {
            Debug.LogWarning($"[ScreenManager] Tela não registrada: {id}.", this);
            return;
        }

        if (target.Layer == ScreenLayer.Root)
            OpenRoot(target);
        else
            OpenOverlay(target);
    }

    private void OpenRoot(UIScreen target)
    {
        // Fecha overlays abertos e a Root atual antes de trocar.
        while (_overlayStack.Count > 0)
            _overlayStack.Pop().Close();

        if (_currentRoot != null && _currentRoot != target)
            _currentRoot.Close();

        _currentRoot = target;
        target.Open();
    }

    private void OpenOverlay(UIScreen target)
    {
        // Evita empilhar a mesma tela duas vezes seguidas.
        if (_overlayStack.Count > 0 && _overlayStack.Peek() == target)
            return;

        _overlayStack.Push(target);
        target.Open();
    }

    /// <summary>
    /// Volta para a tela anterior: fecha o overlay no topo da pilha.
    /// Sem overlays abertos, não faz nada (telas Root não têm "voltar").
    /// </summary>
    public void Back()
    {
        if (_overlayStack.Count == 0)
            return;

        _overlayStack.Pop().Close();
    }

    /// <summary> Alias de Back(): fecha o overlay no topo. </summary>
    public void CloseTop() => Back();
}
