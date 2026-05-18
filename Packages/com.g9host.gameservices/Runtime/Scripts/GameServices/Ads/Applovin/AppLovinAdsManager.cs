using System;
using UnityEngine;
using GameServices.Ads;
#if GAMESERVICES_APPLOVIN
using MaxSdkBase = MaxSdkBase;
#endif

namespace GameServices.AppLovinAds
{
    public class AppLovinAdsManager : MonoBehaviour
    {
#if GAMESERVICES_APPLOVIN
        public static AppLovinAdsManager Instance { get; private set; }

        [SerializeField] private AdsConfig adsConfig;

        private bool initialized;

        private float lastInterstitialTime = -9999f;
        private float lastRewardedTime = -9999f;
        private float lastFullscreenAdTime = -9999f;

        private Action interstitialCloseCallback;
        private Action<bool> rewardedCloseCallback;

        private bool rewardEarned;

        public void SetConfig(AdsConfig config)
        {
            adsConfig = config;
        }

        private void Awake()
        {
            Instance = this;
        }

        public void Initialize()
        {
            if (initialized)
                return;

            MaxSdk.SetSdkKey(adsConfig.appLovinSdkKey);

            MaxSdkCallbacks.OnSdkInitializedEvent += config =>
            {
                initialized = true;

                InitializeInterstitial();
                InitializeRewarded();
                InitializeBanner();

                Debug.Log("[AppLovinAdsManager] Initialized.");
            };

            MaxSdk.InitializeSdk();
        }

        // =========================
        // INTERSTITIAL
        // =========================

        private void InitializeInterstitial()
        {
            string adUnitId = adsConfig.GetInterstitialId(GameServices.Core.AdsProvider.AppLovinMax);

            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += (unitId, adInfo) =>
            {
                Debug.Log("[AppLovin] Interstitial Loaded");
            };

            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += (unitId, adInfo) =>
            {
                interstitialCloseCallback?.Invoke();
                interstitialCloseCallback = null;

                if (adsConfig.autoLoadInterstitial)
                    LoadInterstitial();
            };

            if (adsConfig.autoLoadInterstitial)
                LoadInterstitial();
        }

        public void LoadInterstitial()
        {
            string adUnitId = adsConfig.GetInterstitialId(GameServices.Core.AdsProvider.AppLovinMax);

            MaxSdk.LoadInterstitial(adUnitId);
        }

        public bool IsInterstitialReady()
        {
            string adUnitId = adsConfig.GetInterstitialId(GameServices.Core.AdsProvider.AppLovinMax);

            return MaxSdk.IsInterstitialReady(adUnitId);
        }

        public void ShowInterstitial(Action onClosed = null)
        {
            if (!IsInterstitialReady())
            {
                onClosed?.Invoke();
                return;
            }

            interstitialCloseCallback = onClosed;

            string adUnitId = adsConfig.GetInterstitialId(GameServices.Core.AdsProvider.AppLovinMax);

            lastInterstitialTime = Time.realtimeSinceStartup;
            lastFullscreenAdTime = Time.realtimeSinceStartup;

            MaxSdk.ShowInterstitial(adUnitId);
        }

        // =========================
        // REWARDED
        // =========================

        private void InitializeRewarded()
        {
            string adUnitId = adsConfig.GetRewardedId(GameServices.Core.AdsProvider.AppLovinMax);

            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent +=
                (unitId, reward, adInfo) =>
                {
                    rewardEarned = true;
                };

            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent +=
                (unitId, adInfo) =>
                {
                    rewardedCloseCallback?.Invoke(rewardEarned);
                    rewardedCloseCallback = null;

                    rewardEarned = false;

                    if (adsConfig.autoLoadRewarded)
                        LoadRewarded();
                };

            if (adsConfig.autoLoadRewarded)
                LoadRewarded();
        }

        public void LoadRewarded()
        {
            string adUnitId = adsConfig.GetRewardedId(GameServices.Core.AdsProvider.AppLovinMax);

            MaxSdk.LoadRewardedAd(adUnitId);
        }

        public bool IsRewardedReady()
        {
            string adUnitId = adsConfig.GetRewardedId(GameServices.Core.AdsProvider.AppLovinMax);

            return MaxSdk.IsRewardedAdReady(adUnitId);
        }

        public void ShowRewarded(Action<bool> onClosed = null)
        {
            if (!IsRewardedReady())
            {
                onClosed?.Invoke(false);
                return;
            }

            rewardedCloseCallback = onClosed;

            string adUnitId = adsConfig.GetRewardedId(GameServices.Core.AdsProvider.AppLovinMax);

            lastRewardedTime = Time.realtimeSinceStartup;
            lastFullscreenAdTime = Time.realtimeSinceStartup;

            MaxSdk.ShowRewardedAd(adUnitId);
        }

        // =========================
        // BANNER
        // =========================

        private void InitializeBanner()
        {
            string adUnitId = adsConfig.GetBannerId(GameServices.Core.AdsProvider.AppLovinMax);

            MaxSdk.CreateBanner(adUnitId, GetBannerPosition());

            if (adsConfig.showBannerOnLoad)
                ShowBanner();
            else
                HideBanner();
        }

        public void ShowBanner()
        {
            string adUnitId = adsConfig.GetBannerId(GameServices.Core.AdsProvider.AppLovinMax);

            MaxSdk.ShowBanner(adUnitId);
        }

        public void HideBanner()
        {
            string adUnitId = adsConfig.GetBannerId(GameServices.Core.AdsProvider.AppLovinMax);

            MaxSdk.HideBanner(adUnitId);
        }

        private MaxSdkBase.BannerPosition GetBannerPosition()
        {
            return adsConfig.bannerPosition == BannerPosition.Top
                ? MaxSdkBase.BannerPosition.TopCenter
                : MaxSdkBase.BannerPosition.BottomCenter;
        }
#endif
    }
}