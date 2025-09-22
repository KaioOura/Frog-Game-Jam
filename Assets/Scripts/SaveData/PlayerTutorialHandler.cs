using UnityEngine;

public class PlayerTutorialHandler
{
    private PlayerData _playerData;
    
    public PlayerTutorialHandler(PlayerData playerData)
    {
        _playerData = playerData;
    }
    
    public void SetTutorial(bool isTutorial)
    {
        _playerData.IsTutorial = isTutorial;
    }
        
    public bool GetTutorial()
    {
        return _playerData.IsTutorial;
    }
}
