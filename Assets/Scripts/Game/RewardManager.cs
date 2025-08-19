using System;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    public Action OnReceivedReward; //Criar sistema com rewards customizadas. Desafio: Saber quais metodos chamar
    private AdManager _adManager;

    private void Awake()
    {
        _adManager = FindAnyObjectByType<AdManager>();
    }

    public void RewardAd()
    {
        //Debug.Log("Showing Reward Ad");
        _adManager.ShowRewardedAd(Reward);
    }
    
    private void Reward()
    {
        //Debug.Log("Giving Reward");
        OnReceivedReward?.Invoke();
    }
}
