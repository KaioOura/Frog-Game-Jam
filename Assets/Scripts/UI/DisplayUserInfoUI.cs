using SaveData;
using TMPro;
using UnityEngine;

public class DisplayUserInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI infos;
    [SerializeField] private TextMeshProUGUI logInfos;
    private PlayerDataHandler _playerDataHandler;


    public void Initialize(PlayerDataHandler playerDataHandler)
    {
        _playerDataHandler = playerDataHandler;
        UpdateInfo();
    }

    public void UpdateInfo()
    {
        string info = $"UserID: {_playerDataHandler.Identity.GetUserID()} | Username: {_playerDataHandler.Identity.GetUsername()} \n" +
                      $"Coins: {_playerDataHandler.Currency.GetCoins()} | HighScore: {_playerDataHandler.Progress.GetHighScore()}";

        infos.text = info;
    }

    public void ShowRewardLogs(string log)
    {
        logInfos.text += $"\n {log}";
    }
}