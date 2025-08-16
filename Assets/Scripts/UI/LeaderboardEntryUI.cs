using TMPro;
using UnityEngine;

public class LeaderboardEntryUI : MonoBehaviour
{
    [SerializeField] private GameObject rankingIcon;
    [SerializeField] private TextMeshProUGUI rankingText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI scoreText;

    public void Initialize(LeaderboardEntry leaderboardEntry, int ranking)
    {
        ranking += 1;
        nameText.text = leaderboardEntry.name;
        scoreText.text = leaderboardEntry.score.ToString();
        rankingText.text = ranking.ToString();
    }
}
