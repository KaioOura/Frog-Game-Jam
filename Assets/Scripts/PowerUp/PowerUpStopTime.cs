using System;
using UnityEngine;

namespace PowerUp
{
    public class PowerUpStopTime : MonoBehaviour, IPowerUp
    {
        public event Action<float> OnStopTime;
        public PowerUpEnum PowerUpType { get; }
        private OrderManager _orderManager;

        public void Initialize(OrderManager orderManager)
        {
            _orderManager = orderManager;
        }

        public void UsePowerUp(int level)
        {
            //Pegar tempo de freeze
            int time = level;
            
            OnStopTime?.Invoke(time);
        }
    }
}