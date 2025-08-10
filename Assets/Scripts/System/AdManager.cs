using System;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;
public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

    
    private RewardedAd _rewardedAd;

    private string rewardAdEndGameID;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(gameObject);
        
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            Debug.Log("Mobile Ads initialized");
            // This callback is called once the MobileAds SDK is initialized.
        });
    }

    private void Start()
    {
        InitializeRewardAd();
    }

    private void InitializeRewardAd()
    {
#if UNITY_ANDROID
        rewardAdEndGameID = "ca-app-pub-8669076221541258/2345647997";
#elif UNITY_IPHONE
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
                Debug.Log("Reward Ads failed initialing");
                return;
            }

            _rewardedAd = ad;
            Debug.Log("Reward Ads loaded");
        });
    }

    [ContextMenu("Show reward ad")]
    public void ShowRewardAdDebug()
    {
        ShowRewardedAd(null);
    }
    
    public void ShowRewardedAd(Action rewardToGive)
    {
        if (!_rewardedAd.CanShowAd())
        {
            Debug.Log("Show Reward Ad failed");
            return;
        }
        
        _rewardedAd.Show((Reward reward) =>
        {
            rewardToGive?.Invoke();
            ReloadAd(_rewardedAd);
        });
    }
    
    void ReloadAd(RewardedAd rewardedAd)
    {
        rewardedAd.OnAdFullScreenContentClosed += LoadRewardAd;
        rewardedAd.OnAdFullScreenContentFailed += (AdError adError) => LoadRewardAd();
    }
    
}
