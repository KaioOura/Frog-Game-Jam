using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;
    public static bool IsReady;

    public bool IsBuildTest;
    
    private static RewardedAd _rewardedAd;
    private Action _pendingRewardCallback;
    private bool _pendingReward;
    private Reward _lastReward;
    private string _rewardAdEndGameID;
    

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
            //Debug.LogError($"User earned reward: {_lastReward.Amount} {_lastReward.Type}");
        }
    }

    private void InitializeRewardAd()
    {
#if UNITY_ANDROID
        _rewardAdEndGameID = IsBuildTest 
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
        RewardedAd.Load(_rewardAdEndGameID, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                GameLogger.Error("Failed to load rewarded ad: " + error);
                return;
            }
            _rewardedAd = ad;
            Debug.Log("Rewarded ad loaded");
        });
    }

    [ContextMenu("Show reward ad")]
    public void ShowRewardAdDebug()
    {
        ShowRewardedAd(() => Debug.Log("Reward from debug ad"));
    }

    public void ShowRewardedAd(Action rewardCallback)
    {
         _pendingRewardCallback = rewardCallback;

#if UNITY_EDITOR
        // Mock de editor: anúncio real não carrega no editor, então simulamos a
        // visualização e concedemos a recompensa pelo mesmo caminho (Update).
        if (_rewardedAd == null || !_rewardedAd.CanShowAd())
        {
            Debug.Log("[AdManager] Editor mock: concedendo recompensa sem anúncio real.");
            _pendingReward = true;
            return;
        }
#endif

        if (IsAdAvailable())
        {
            GameAnalyticsManager.Track("start_rewarded_ad");
            Debug.Log($"RewardedAdn exists an can show ad");
            _rewardedAd.Show((Reward reward) =>
            {
                _lastReward = reward;
                _pendingReward = true;
                Debug.Log($"User earned reward: {reward.Amount} {reward.Type}");
                Debug.Log(String.Format("Reward pls", reward.Type, reward.Amount));
            });
            
            _rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Ad closed, reloading...");
                GameAnalyticsManager.Track("close_rewarded_ad");
                LoadRewardAd();
            };
            
            _rewardedAd.OnAdFullScreenContentFailed += (AdError adError) =>
            {
                Debug.Log("Ad failed to show: " + adError);
                GameAnalyticsManager.Track("failed_rewarded_ad");
                LoadRewardAd();
            };
        }
        else
        {
            Debug.Log("Rewarded ad is not ready yet.");
            GameAnalyticsManager.Track("not_ready_rewarded_ad");
        }
    }

    public static bool IsAdAvailable()
    {
#if UNITY_EDITOR
        return true; // mock: mantém o botão de reward habilitado para testar no editor
#else
        return _rewardedAd != null && _rewardedAd.CanShowAd();
#endif
    }
}
