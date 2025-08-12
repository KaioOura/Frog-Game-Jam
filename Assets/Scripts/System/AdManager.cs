using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;
    public static bool IsReady;

    private RewardedAd _rewardedAd;
    private Action _pendingRewardCallback;
    private bool _pendingReward;
    private Reward _lastReward;

    private string rewardAdEndGameID;
    public bool IsBuildTest;


    private DisplayUserInfoUI displayUserInfoUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Debug.Log("Initializing Mobile Ads");
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            InitializeRewardAd();
            Debug.Log("Mobile Ads initialized");
            IsReady = true;
        });
    }

    private void Update()
    {
        if (_pendingReward)
        {
            _pendingReward = false;
            _pendingRewardCallback?.Invoke();
            Debug.LogError($"User earned reward: {_lastReward.Amount} {_lastReward.Type}");
        }
    }

    private void InitializeRewardAd()
    {
#if UNITY_ANDROID
        rewardAdEndGameID = IsBuildTest 
            ? "ca-app-pub-3940256099942544/5224354917" // Test ID
            : "ca-app-pub-8669076221541258/2345647997";
#elif UNITY_IPHONE
        // Coloque o ID iOS aqui
#endif
        LoadRewardAd();
    }

    private void LoadRewardAd()
    {
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        Debug.Log("Loading reward ad");

        var adRequest = new AdRequest();
        RewardedAd.Load(rewardAdEndGameID, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                Debug.LogError("Failed to load rewarded ad: " + error);
                return;
            }

            _rewardedAd = ad;
            Debug.Log("Rewarded ad loaded");

            // Eventos do ciclo de vida do anúncio
        });
    }

    [ContextMenu("Show reward ad")]
    public void ShowRewardAdDebug()
    {
        //ShowRewardedAd(() => Debug.Log("Reward from debug ad"));
    }

    public void ShowRewardedAd(Action rewardCallback)
    {
        //displayUserInfoUI = _displayUserInfoUI;
        
         _pendingRewardCallback = rewardCallback;
         
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            //displayUserInfoUI.ShowRewardLogs("RewardedAdn exists an can show ad");
            Debug.Log($"RewardedAdn exists an can show ad");
            _rewardedAd.Show((Reward reward) =>
            {
                _lastReward = reward;
                _pendingReward = true;
                //displayUserInfoUI.ShowRewardLogs($"User earned reward: {reward.Amount} {reward.Type}");
                Debug.Log($"User earned reward: {reward.Amount} {reward.Type}");
                //_pendingRewardCallback?.Invoke();
                //_pendingRewardCallback = null;
                
                Debug.Log(String.Format("Reward pls", reward.Type, reward.Amount));
            });
            
            _rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                //displayUserInfoUI.ShowRewardLogs($"Ad closed, reloading...");
                Debug.Log("Ad closed, reloading...");
                LoadRewardAd();
            };
            
            _rewardedAd.OnAdFullScreenContentFailed += (AdError adError) =>
            {
                //displayUserInfoUI.ShowRewardLogs($"Ad failed to show");
                Debug.Log("Ad failed to show: " + adError);
                LoadRewardAd();
            };
        }
        else
        {
            displayUserInfoUI.ShowRewardLogs($"Rewarded ad is not ready yet.");
            Debug.Log("Rewarded ad is not ready yet.");
        }
    }
}
