using System.Collections.Generic;
using System.Linq;
using PowerUp;
using SaveData;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    private List<IPowerUp> powerUps = new List<IPowerUp>();

    private PlayerDataHandler _playerDataHandler;
    
    public void Initialize(PlayerDataHandler playerDataHandler)
    {
        _playerDataHandler = playerDataHandler;

        foreach (Transform child in transform)
        {
            powerUps.Add(child.GetComponent<IPowerUp>());
        }
    }
    
    public void OnReceivePowerUp(PowerUpEnum powerUpType)
    {
        var powerUp = powerUps.First(pu => pu.PowerUpType == powerUpType);

        powerUp.UsePowerUp(level: _playerDataHandler.PowerUps.GetPowerUpLevel(powerUpType));
    }

    [ContextMenu("StopTime")]
    public void TriggerTimeStop()
    {
        OnReceivePowerUp(PowerUpEnum.StopOrderTime);
    }
}

public enum PowerUpEnum
{
    CompleteOrder,
    StopOrderTime
}