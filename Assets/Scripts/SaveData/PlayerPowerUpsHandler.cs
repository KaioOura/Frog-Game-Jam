namespace SaveData
{
    public class PlayerPowerUpsHandler
    {
        private PlayerData _playerData;
    
        public PlayerPowerUpsHandler(PlayerData playerData)
        {
            _playerData = playerData;
        }
    
        public void SetPowerUpLevel(PowerUpEnum powerUpType, int level)
        {
            if (_playerData != null)
            {
                //_playerData.Upgrades[powerUpType] = level;
            }
        }

        public int GetPowerUpLevel(PowerUpEnum powerUpType)
        {
            // if (_playerData != null && _playerData.Upgrades.TryGetValue(powerUpType, out int level))
            // {
            //     return level;
            // }

            return 0; // ou outro valor padrão apropriado
        }
    }
}
