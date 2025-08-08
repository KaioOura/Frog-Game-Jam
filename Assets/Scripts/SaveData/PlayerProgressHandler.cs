using UnityEngine;

public class PlayerProgressHandler
{
    private PlayerData _playerData;
    
    public PlayerProgressHandler(PlayerData playerData)
    {
        _playerData = playerData;
    }
    
    public int GetHighScore()
    {
        return _playerData.Highschore;
    }

    public void SetHighScore(int newHighScore)
    {
        _playerData.Highschore = newHighScore;
    }
    
}
