using TMPro;
using UnityEngine;

/// <summary>
/// Exibição de pontuação (score atual e final) e high score.
/// Extraído do antigo UIManager. Ligado pelo ScoreManager via OnScoreChanged/OnHighScoreChanged.
/// </summary>
public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentScoreTMP;
    [SerializeField] private TextMeshProUGUI finalScoreTMP;
    [SerializeField] private TextMeshProUGUI secondChanceScoreTMP;

    [Space]
    [SerializeField] private TextMeshProUGUI finalHighScoreTMP;
    [SerializeField] private TextMeshProUGUI secondChanceHighScoreTMP;

    public void UpdateScore(int scoreToUpdate)
    {
        currentScoreTMP.text = scoreToUpdate.ToString();
        finalScoreTMP.text = scoreToUpdate.ToString();
        secondChanceScoreTMP.text = scoreToUpdate.ToString();
    }

    public void UpdateHighScore(int scoreToUpdate)
    {
        finalHighScoreTMP.text = scoreToUpdate.ToString();
        secondChanceHighScoreTMP.text = scoreToUpdate.ToString();
    }
}
