using SaveData;
using TMPro;
using UnityEngine;

public class DisplayUserInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI infos;
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
}