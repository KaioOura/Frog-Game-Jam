using TMPro;
using UnityEngine;

public class DisplayUserInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI infos;
    private PlayerData _playerData;
    private FirebaseDataManager _firebaseDataManager;


    public void Initialize(FirebaseDataManager firebaseDataManager)
    {
        _firebaseDataManager = firebaseDataManager;
        _playerData = firebaseDataManager.PlayerData;
        UpdateInfo();
    }

    public void UpdateInfo()
    {
        if (_playerData == null)
        {
            print("Couldn't find player data, infos will not be updated.");
        }


        string info = $"UserID: {_firebaseDataManager.UserID} | Username: {_playerData.Username} \n" +
                      $"Coins: {_playerData.Coins} | HighScore: {_playerData.Highschore}";

        infos.text = info;
    }
}