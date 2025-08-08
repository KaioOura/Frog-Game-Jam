using UnityEngine;

namespace SaveData
{
    public class PlayerDataHandler : MonoBehaviour
    {
        private PlayerData _playerData;
        
        public PlayerIdentityHandler Identity { get; private set; }
        public PlayerCurrencyHandler Currency { get; private set; }
        public PlayerProgressHandler Progress { get; private set; }
        public PlayerPowerUpsHandler PowerUps { get; private set; }
        public void Initialize(PlayerData playerData)
        {
            _playerData = playerData;
            Identity = new PlayerIdentityHandler(_playerData);
            Currency = new PlayerCurrencyHandler(_playerData);
            Progress = new PlayerProgressHandler(_playerData);
            PowerUps = new PlayerPowerUpsHandler(_playerData);
        }

        public void InitializeNotLoggedIn()
        {
            _playerData = new PlayerData();
            _playerData.Username = "NotLoggedIn";
            Identity = new PlayerIdentityHandler(_playerData);
            Currency = new PlayerCurrencyHandler(_playerData);
            Progress = new PlayerProgressHandler(_playerData);
            PowerUps = new PlayerPowerUpsHandler(_playerData);
        }
    }
}

