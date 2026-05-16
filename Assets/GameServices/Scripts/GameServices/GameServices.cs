using System;
using UnityEngine;
using GameServices.Ads;

#if GAMESERVICES_LEVELPLAY
using GameServices.LevelPlayAds;
#endif

#if GAMESERVICES_ADMOB
using GameServices.AdMobAds;
#endif

#if GAMESERVICES_APPLOVIN
using GameServices.AppLovinAds;
#endif

namespace GameServices.Core
{
    public class GameServices : MonoBehaviour
    {
        public static GameServices Instance { get; private set; }

        [SerializeField] private GameServicesConfig config;

#if GAMESERVICES_LEVELPLAY
        [Header("Ads Managers")]
        [SerializeField] private LevelPlayAdsManager levelPlayAdsManager;
#endif

#if GAMESERVICES_ADMOB
        [SerializeField] private AdMobAdsManager adMobAdsManager;
#endif

#if GAMESERVICES_APPLOVIN
        [SerializeField] private AppLovinAdsManager appLovinAdsManager;
#endif

        public GameServicesConfig Config => config;

        public AdsProvider AdsProvider =>
            config != null
                ? config.adsProvider
                : AdsProvider.None;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            DontDestroyOnLoad(gameObject);

            Initialize();
        }

        private void Initialize()
        {
            if (config == null)
            {
                Debug.LogError("[GameServices] Config missing.");
                return;
            }

            GetAdObjects();
            InitializeAds();
        }

        private void GetAdObjects()
        {
#if GAMESERVICES_LEVELPLAY
            if (levelPlayAdsManager == null)
            {
                levelPlayAdsManager = GetComponentInChildren<LevelPlayAdsManager>();
            }
#endif

#if GAMESERVICES_ADMOB
            if (adMobAdsManager == null)
            {
               adMobAdsManager = GetComponentInChildren<AdMobAdsManager>();
            }
#endif

#if GAMESERVICES_APPLOVIN
            if (appLovinAdsManager == null)
            {
                appLovinAdsManager = GetComponentInChildren<AppLovinAdsManager>();
            }
#endif
            
        }

        private void InitializeAds()
        {
#if GAMESERVICES_LEVELPLAY
            if (levelPlayAdsManager != null)
            {
                var active = config.adsProvider == AdsProvider.LevelPlay;
                levelPlayAdsManager.SetConfig(config.adsConfig);
                levelPlayAdsManager.gameObject.SetActive(active);
                levelPlayAdsManager.Initialize();
            }
#endif

#if GAMESERVICES_ADMOB
            if (adMobAdsManager != null)
            {
                var active = config.adsProvider == AdsProvider.AdMobDirect;
                adMobAdsManager.SetConfig(config.adsConfig);
                adMobAdsManager.gameObject.SetActive(active);
                adMobAdsManager.Initialize();
            }
#endif

#if GAMESERVICES_APPLOVIN
            if (appLovinAdsManager != null)
            {
                var active = config.adsProvider == AdsProvider.AppLovinMax;
                appLovinAdsManager.SetConfig(config.adsConfig);
                appLovinAdsManager.gameObject.SetActive(active);
                appLovinAdsManager.Initialize();
            }
#endif
        }

        // =====================================================
        // BANNER
        // =====================================================

        public static void ShowBanner()
        {
            if (Instance == null)
                return;

            switch (Instance.AdsProvider)
            {
#if GAMESERVICES_LEVELPLAY
                case AdsProvider.LevelPlay:
                    Instance.levelPlayAdsManager?.ShowBanner();
                    break;
#endif

#if GAMESERVICES_ADMOB
                case AdsProvider.AdMobDirect:
                    Instance.adMobAdsManager?.ShowBanner();
                    break;
#endif

#if GAMESERVICES_APPLOVIN
                case AdsProvider.AppLovinMax:
                    Instance.appLovinAdsManager?.ShowBanner();
                    break;
#endif
            }
        }

        public static void HideBanner()
        {
            if (Instance == null)
                return;

            switch (Instance.AdsProvider)
            {
#if GAMESERVICES_LEVELPLAY
                case AdsProvider.LevelPlay:
                    Instance.levelPlayAdsManager?.HideBanner();
                    break;
#endif

#if GAMESERVICES_ADMOB
                case AdsProvider.AdMobDirect:
                    Instance.adMobAdsManager?.HideBanner();
                    break;
#endif

#if GAMESERVICES_APPLOVIN
                case AdsProvider.AppLovinMax:
                    Instance.appLovinAdsManager?.HideBanner();
                    break;
#endif
            }
        }

        // =====================================================
        // INTERSTITIAL
        // =====================================================

        public static void ShowInterstitial(
            string placement = null,
            Action onClosed = null)
        {
            if (Instance == null)
            {
                onClosed?.Invoke();
                return;
            }

            switch (Instance.AdsProvider)
            {
#if GAMESERVICES_LEVELPLAY
                case AdsProvider.LevelPlay:
                    Instance.levelPlayAdsManager?.ShowInterstitial(
                        placement,
                        onClosed);
                    break;
#endif

#if GAMESERVICES_ADMOB
                case AdsProvider.AdMobDirect:
                    Instance.adMobAdsManager?.ShowInterstitial(
                        onClosed);
                    break;
#endif

#if GAMESERVICES_APPLOVIN
                case AdsProvider.AppLovinMax:
                    Instance.appLovinAdsManager?.ShowInterstitial(
                        onClosed);
                    break;
#endif

                default:
                    onClosed?.Invoke();
                    break;
            }
        }

        // =====================================================
        // REWARDED
        // =====================================================

        public static void ShowRewarded(
            string placement = null,
            Action<bool> onClosed = null)
        {
            if (Instance == null)
            {
                onClosed?.Invoke(false);
                return;
            }

            switch (Instance.AdsProvider)
            {
#if GAMESERVICES_LEVELPLAY
                case AdsProvider.LevelPlay:
                    Instance.levelPlayAdsManager?.ShowRewarded(
                        placement,
                        onClosed);
                    break;
#endif

#if GAMESERVICES_ADMOB
                case AdsProvider.AdMobDirect:
                    Instance.adMobAdsManager?.ShowRewarded(
                        onClosed);
                    break;
#endif

#if GAMESERVICES_APPLOVIN
                case AdsProvider.AppLovinMax:
                    Instance.appLovinAdsManager?.ShowRewarded(
                        onClosed);
                    break;
#endif

                default:
                    onClosed?.Invoke(false);
                    break;
            }
        }
    }
}