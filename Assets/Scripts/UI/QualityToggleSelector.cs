using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Liga um conjunto de Toggles aos níveis de qualidade do <see cref="QualitySetter"/>.
/// Cada toggle representa um nível (índice no array = nível de qualidade): ao abrir
/// o menu o toggle do nível atual fica marcado, e selecionar outro troca a qualidade.
/// Os toggles devem compartilhar um ToggleGroup (comportamento de rádio).
/// </summary>
public class QualityToggleSelector : MonoBehaviour
{
    [SerializeField] private QualitySetter qualitySetter;

    [Tooltip("Toggles ordenados do menor para o maior nível de qualidade (índice = nível)")]
    [SerializeField] private Toggle[] qualityToggles;

    private void Awake()
    {
        for (int i = 0; i < qualityToggles.Length; i++)
        {
            int level = i; // captura local para o listener
            qualityToggles[i].onValueChanged.AddListener(isOn =>
            {
                if (isOn) SelectLevel(level);
            });
        }
    }

    private void OnEnable()
    {
        RefreshSelection();
    }

    /// <summary>
    /// Troca a qualidade e deixa só o toggle do nível escolhido marcado
    /// (comportamento de rádio, sem depender de um ToggleGroup).
    /// </summary>
    private void SelectLevel(int level)
    {
        qualitySetter.ChangeQuality(level);
        SetActiveToggle(level);
    }

    /// <summary>
    /// Marca o toggle correspondente à qualidade atual sem disparar a troca.
    /// Chamado ao habilitar o menu; exposto caso o painel não use SetActive.
    /// </summary>
    public void RefreshSelection()
    {
        int current = Mathf.Clamp(qualitySetter.CurrentQuality, 0, qualityToggles.Length - 1);
        SetActiveToggle(current);
    }

    private void SetActiveToggle(int level)
    {
        for (int i = 0; i < qualityToggles.Length; i++)
            qualityToggles[i].SetIsOnWithoutNotify(i == level);
    }
}
