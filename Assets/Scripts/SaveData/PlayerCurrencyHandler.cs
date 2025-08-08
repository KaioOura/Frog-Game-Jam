using UnityEngine;

public class PlayerCurrencyHandler
{
    private PlayerData _playerData;
    
    public PlayerCurrencyHandler(PlayerData playerData)
    {
        _playerData = playerData;
    }
    
    public int GetCoins()
    {
        return _playerData.Coins;
    }

    public void SetCoins(int newCoins)
    {
        _playerData.Coins = newCoins;
    }
}
