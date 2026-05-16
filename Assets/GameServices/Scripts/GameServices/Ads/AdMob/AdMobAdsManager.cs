using System;
using UnityEngine;
#if GAMESERVICES_ADMOB
using GoogleMobileAds.Api;
#endif
using GameServices.Ads;

namespace GameServices.AdMobAds
{
    public class AdMobAdsManager : MonoBehaviour
    {
#if GAMESERVICES_ADMOB
        public static AdMobAdsManager Instance { get; private set; }

        [SerializeField] private AdsConfig adsConfig;

        private InterstitialAd interstitialAd;
        private RewardedAd rewardedAd;
        private BannerView bannerView;

        private bool initialized;

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

            MobileAds.Initialize(initStatus =>
            {
                initialized = true;

                Debug.Log("[AdMobAdsManager] Initialized.");

                if (adsConfig.autoLoadInterstitial)
                    LoadInterstitial();

                if (adsConfig.autoLoadRewarded)
                    LoadRewarded();

                if (adsConfig.enableBanner)
                    CreateBanner();
            });
        }

        // =========================
        // INTERSTITIAL
        // =========================

        public void LoadInterstitial()
        {
            string adUnitId = adsConfig.GetInterstitialId(GameServices.Core.AdsProvider.AdMobDirect);

            InterstitialAd.Load(adUnitId,
                new AdRequest(),
                (ad, error) =>
                {
                    if (error != null || ad == null)
                    {
                        Debug.LogError(error);
                        return;
                    }

                    interstitialAd = ad;

                    interstitialAd.OnAdFullScreenContentClosed += () =>
                    {
                        interstitialCloseCallback?.Invoke();
                        interstitialCloseCallback = null;

                        if (adsConfig.autoLoadInterstitial)
                            LoadInterstitial();
                    };
                });
        }

        public bool IsInterstitialReady()
        {
            return interstitialAd != null;
        }

        public void ShowInterstitial(Action onClosed = null)
        {
            if (!IsInterstitialReady())
            {
                onClosed?.Invoke();
                return;
            }

            interstitialCloseCallback = onClosed;

            interstitialAd.Show();
        }

        // =========================
        // REWARDED
        // =========================

        public void LoadRewarded()
        {
            string adUnitId = adsConfig.GetRewardedId(GameServices.Core.AdsProvider.AdMobDirect);

            RewardedAd.Load(adUnitId,
                new AdRequest(),
                (ad, error) =>
                {
                    if (error != null || ad == null)
                    {
                        Debug.LogError(error);
                        return;
                    }

                    rewardedAd = ad;

                    rewardedAd.OnAdFullScreenContentClosed += () =>
                    {
                        rewardedCloseCallback?.Invoke(rewardEarned);

                        rewardedCloseCallback = null;
                        rewardEarned = false;

                        if (adsConfig.autoLoadRewarded)
                            LoadRewarded();
                    };
                });
        }

        public bool IsRewardedReady()
        {
            return rewardedAd != null;
        }

        public void ShowRewarded(Action<bool> onClosed = null)
        {
            if (!IsRewardedReady())
            {
                onClosed?.Invoke(false);
                return;
            }

            rewardedCloseCallback = onClosed;

            rewardedAd.Show(reward =>
            {
                rewardEarned = true;
            });
        }

        // =========================
        // BANNER
        // =========================

        private void CreateBanner()
        {
            string adUnitId = adsConfig.GetBannerId(GameServices.Core.AdsProvider.AdMobDirect);

            bannerView = new BannerView(
                adUnitId,
                AdSize.Banner,
                GetBannerPosition());

            bannerView.LoadAd(new AdRequest());

            if (!adsConfig.showBannerOnLoad)
                HideBanner();
        }

        public void ShowBanner()
        {
            bannerView?.Show();
        }

        public void HideBanner()
        {
            bannerView?.Hide();
        }

        public void DestroyBanner()
        {
            bannerView?.Destroy();
            bannerView = null;
        }

        private AdPosition GetBannerPosition()
        {
            return adsConfig.bannerPosition == BannerPosition.Top
                ? AdPosition.Top
                : AdPosition.Bottom;
        }
#endif
    }
}