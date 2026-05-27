#if GAMESERVICES_LEVELPLAY
using System;
using GameServices.Ads;
using UnityEngine;
using Unity.Services.LevelPlay;
using GameServices.Core;

namespace GameServices.LevelPlayAds
{
    public class LevelPlayAdsManager : MonoBehaviour
    {
        private readonly GameServicesConfig gameConfig;
        [SerializeField] private AdsConfig adsConfig;

        private LevelPlayInterstitialAd interstitialAd;
        private LevelPlayRewardedAd rewardedAd;
        private LevelPlayBannerAd bannerAd;

        private bool initialized;

        private float lastInterstitialShowTime = -9999f;
        private float lastRewardedShowTime = -9999f;
        private float lastFullscreenAdShowTime = -9999f;

        private Action onInterstitialClosed;
        private Action<bool> onRewardedClosed;

        private bool rewardedEarned;
        
        public void SetConfig(AdsConfig config)
        {
            adsConfig = config;
        }
        

        public void Initialize()
        {
            if (adsConfig == null)
            {
                Debug.LogError("[GameServices] AdsConfig missing.");
                return;
            }

            if (string.IsNullOrEmpty(adsConfig.levelPlayAppKey))
            {
                Debug.LogError("[GameServices] LevelPlay App Key missing.");
                return;
            }

            LevelPlay.OnInitSuccess += OnInitSuccess;
            LevelPlay.OnInitFailed += OnInitFailed;

            LevelPlay.Init(adsConfig.levelPlayAppKey);
        }

        private void OnInitSuccess(LevelPlayConfiguration configuration)
        {
            initialized = true;

            CreateInterstitial();
            CreateRewarded();
            CreateBanner();

            if (adsConfig.enableInterstitial && adsConfig.autoLoadInterstitial)
                LoadInterstitial();

            if (adsConfig.enableRewarded && adsConfig.autoLoadRewarded)
                LoadRewarded();

            if (adsConfig.enableBanner && adsConfig.autoLoadBanner)
                LoadBanner();

            Debug.Log("[GameServices] LevelPlay initialized.");
        }

        private void OnInitFailed(LevelPlayInitError error)
        {
            Debug.LogError("[GameServices] LevelPlay init failed: " + error);
        }

        // =========================
        // INTERSTITIAL
        // =========================

        private void CreateInterstitial()
        {
            if (!adsConfig.enableInterstitial)
                return;

            string adUnitId = adsConfig.GetInterstitialId(AdsProvider.LevelPlay);

            if (string.IsNullOrEmpty(adUnitId))
            {
                Debug.LogWarning("[GameServices] LevelPlay interstitial ID missing.");
                return;
            }

            interstitialAd = new LevelPlayInterstitialAd(adUnitId);

            interstitialAd.OnAdLoaded += adInfo =>
            {
                Debug.Log("[GameServices] Interstitial loaded.");
            };

            interstitialAd.OnAdLoadFailed += error =>
            {
                Debug.LogError("[GameServices] Interstitial load failed: " + error);
            };

            interstitialAd.OnAdDisplayed += adInfo =>
            {
                // Interstitial cooldown starts only when an interstitial is shown.
                lastInterstitialShowTime = Time.realtimeSinceStartup;
            };

            interstitialAd.OnAdDisplayFailed += (adInfo, error) =>
            {
                Debug.LogError("[GameServices] Interstitial display failed: " + error);

                if (adsConfig.autoLoadInterstitial)
                    LoadInterstitial();
            };

            interstitialAd.OnAdClosed += adInfo =>
            {
                Debug.Log("[GameServices] Interstitial closed.");

                onInterstitialClosed?.Invoke();
                onInterstitialClosed = null;

                if (adsConfig.autoLoadInterstitial)
                    LoadInterstitial();
            };
        }

        public void LoadInterstitial()
        {
            if (!initialized || !adsConfig.enableInterstitial || interstitialAd == null)
                return;

            interstitialAd.LoadAd();
        }

        public bool IsInterstitialReady()
        {
            return initialized &&
                   adsConfig.enableInterstitial &&
                   interstitialAd != null &&
                   interstitialAd.IsAdReady();
        }

        private bool CanShowInterstitial(out string reason)
        {
            if (!initialized)
            {
                reason = "LevelPlay is not initialized yet.";
                return false;
            }

            if (!adsConfig.enableInterstitial)
            {
                reason = "Interstitial ads are disabled in AdsConfig.";
                return false;
            }

            if (interstitialAd == null)
            {
                reason = "Interstitial ad object was not created. Check the LevelPlay interstitial ad unit id.";
                return false;
            }

            if (!interstitialAd.IsAdReady())
            {
                reason = "Interstitial ad is not loaded yet.";
                return false;
            }

            float interstitialCooldownRemaining =
                adsConfig.interstitialCooldown - (Time.realtimeSinceStartup - lastInterstitialShowTime);

            if (interstitialCooldownRemaining > 0f)
            {
                reason = $"Interstitial cooldown active. Remaining: {interstitialCooldownRemaining:0.0}s";
                return false;
            }

            float fullscreenCooldownRemaining =
                adsConfig.globalFullscreenAdCooldown - (Time.realtimeSinceStartup - lastRewardedShowTime);

            if (fullscreenCooldownRemaining > 0f)
            {
                reason = $"Fullscreen cooldown after rewarded active. Remaining: {fullscreenCooldownRemaining:0.0}s";
                return false;
            }

            reason = null;
            return true;
        }

        public bool CanShowInterstitial()
        {
            return CanShowInterstitial(out _);
        }

        public void ShowInterstitial()
        {
            ShowInterstitial(null, null);
        }

        public void ShowInterstitial(string placementName, Action onClosed)
        {
            if (!CanShowInterstitial(out string reason))
            {
                Debug.LogWarning("[GameServices] Interstitial cannot show: " + reason);
                onClosed?.Invoke();
                return;
            }

            onInterstitialClosed = onClosed;

            if (string.IsNullOrEmpty(placementName))
                interstitialAd.ShowAd();
            else
                interstitialAd.ShowAd(placementName);
        }

        // =========================
        // REWARDED
        // =========================

        private void CreateRewarded()
        {
            if (!adsConfig.enableRewarded)
                return;

            string adUnitId = adsConfig.GetRewardedId(AdsProvider.LevelPlay);

            if (string.IsNullOrEmpty(adUnitId))
            {
                Debug.LogWarning("[GameServices] LevelPlay rewarded ID missing.");
                return;
            }

            rewardedAd = new LevelPlayRewardedAd(adUnitId);

            rewardedAd.OnAdLoaded += adInfo =>
            {
                Debug.Log("[GameServices] Rewarded loaded.");
            };

            rewardedAd.OnAdLoadFailed += error =>
            {
                Debug.LogError("[GameServices] Rewarded load failed: " + error);
            };

            rewardedAd.OnAdDisplayed += adInfo =>
            {
                rewardedEarned = false;
                // Rewarded ads are always available, but this time is used
                // to block interstitials for globalFullscreenAdCooldown seconds.
                lastRewardedShowTime = Time.realtimeSinceStartup;
            };

            rewardedAd.OnAdDisplayFailed += (adInfo, error) =>
            {
                Debug.LogError("[GameServices] Rewarded display failed: " + error);
                
                onRewardedClosed = null;

                if (adsConfig.autoLoadRewarded)
                    LoadRewarded();
            };

            rewardedAd.OnAdRewarded += (adInfo, reward) =>
            {
                rewardedEarned = true;
                onRewardedClosed?.Invoke(rewardedEarned);
                onRewardedClosed = null;
                Debug.Log("[GameServices] Reward earned: " + reward.Name + " x" + reward.Amount);
            };

            rewardedAd.OnAdClosed += adInfo =>
            {
                Debug.Log("[GameServices] Rewarded closed. Reward earned: " + rewardedEarned);

                if (!rewardedEarned)
                {
                    onRewardedClosed?.Invoke(false);
                    onRewardedClosed = null;
                }

                if (adsConfig.autoLoadRewarded)
                    LoadRewarded();
            };
        }

        public void LoadRewarded()
        {
            if (!initialized || !adsConfig.enableRewarded || rewardedAd == null)
                return;

            rewardedAd.LoadAd();
        }

        public bool IsRewardedReady()
        {
            return initialized &&
                   adsConfig.enableRewarded &&
                   rewardedAd != null &&
                   rewardedAd.IsAdReady();
        }

        private bool CanShowRewarded(out string reason)
        {
            if (!initialized)
            {
                reason = "LevelPlay is not initialized yet.";
                return false;
            }

            if (!adsConfig.enableRewarded)
            {
                reason = "Rewarded ads are disabled in AdsConfig.";
                return false;
            }

            if (rewardedAd == null)
            {
                reason = "Rewarded ad object was not created. Check the LevelPlay rewarded ad unit id.";
                return false;
            }

            if (!rewardedAd.IsAdReady())
            {
                reason = "Rewarded ad is not loaded yet.";
                return false;
            }

            reason = null;
            return true;
        }

        public bool CanShowRewarded()
        {
            return CanShowRewarded(out _);
        }

        public void ShowRewarded(string placementName = null)
        {
            ShowRewarded(placementName, null);
        }

        public void ShowRewarded(string placementName, Action<bool> onClosed)
        {
            if (!CanShowRewarded(out string reason))
            {
                Debug.LogWarning("[GameServices] Rewarded cannot show: " + reason);
                onClosed?.Invoke(false);
                return;
            }

            rewardedEarned = false;
            onRewardedClosed = onClosed;

            if (string.IsNullOrEmpty(placementName))
                placementName = adsConfig.levelPlayRewardedPlacement;

            rewardedAd.ShowAd(placementName);
        }

        // =========================
        // BANNER
        // =========================

        private void CreateBanner()
        {
            if (!adsConfig.enableBanner)
                return;

            string adUnitId = adsConfig.GetBannerId(AdsProvider.LevelPlay);

            if (string.IsNullOrEmpty(adUnitId))
            {
                Debug.LogWarning("[GameServices] LevelPlay banner ID missing.");
                return;
            }

            var bannerConfig = new LevelPlayBannerAd.Config.Builder()
                .SetSize(GetBannerSize())
                .SetPosition(GetBannerPosition())
                .SetDisplayOnLoad(adsConfig.showBannerOnLoad)
                .SetRespectSafeArea(adsConfig.respectSafeArea)
                .Build();

            bannerAd = new LevelPlayBannerAd(adUnitId, bannerConfig);

            bannerAd.OnAdLoaded += adInfo =>
            {
                Debug.Log("[GameServices] Banner loaded.");
            };

            bannerAd.OnAdLoadFailed += error =>
            {
                Debug.LogError("[GameServices] Banner load failed: " + error);
            };
        }

        public void LoadBanner()
        {
            if (!initialized || !adsConfig.enableBanner || bannerAd == null)
                return;

            bannerAd.LoadAd();
        }

        public void ShowBanner()
        {
            if (!initialized || !adsConfig.enableBanner || bannerAd == null)
                return;

            bannerAd.ShowAd();
        }

        public void HideBanner()
        {
            bannerAd?.HideAd();
        }

        public void DestroyBanner()
        {
            bannerAd?.DestroyAd();
            bannerAd = null;
        }

        private LevelPlayAdSize GetBannerSize()
        {
            switch (adsConfig.bannerSize)
            {
                case BannerSize.Large:
                    return LevelPlayAdSize.LARGE;

                case BannerSize.MediumRectangle:
                    return LevelPlayAdSize.MEDIUM_RECTANGLE;

                case BannerSize.Standard:
                case BannerSize.Adaptive:
                default:
                    return LevelPlayAdSize.BANNER;
            }
        }

        private LevelPlayBannerPosition GetBannerPosition()
        {
            return adsConfig.bannerPosition == BannerPosition.Top
                ? LevelPlayBannerPosition.TopCenter
                : LevelPlayBannerPosition.BottomCenter;
        }
    }
}
#endif