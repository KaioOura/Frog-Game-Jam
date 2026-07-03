using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Exibição das vidas do jogador. Extraído do antigo UIManager.
/// Ligado por Health.OnUpdateHealth via Character.InitializeComponents.
/// </summary>
public class HealthUI : MonoBehaviour
{
    [SerializeField] private Image[] lifeImages;

    public void UpdateLives(int life)
    {
        for (var index = 0; index < lifeImages.Length; index++)
            lifeImages[index].gameObject.SetActive(index < life);
    }
}
