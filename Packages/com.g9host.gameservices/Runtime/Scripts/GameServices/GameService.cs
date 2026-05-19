using System;
using UnityEngine;
using GameServices.Ads;
using GameServices.Analytics;

#if GAMESERVICES_LEVELPLAY
using GameServices.LevelPlayAds;
#endif

#if GAMESERVICES_ADMOB
using GameServices.AdMobAds;
#endif

#if GAMESERVICES_APPLOVIN
using GameServices.AppLovinAds;
#endif

#if GAMESERVICES_UNITY_IAP
using GameServices.IAP;
#endif

namespace GameServices.Core
{
    public class GameService : MonoBehaviour
    {
        public static GameService Instance { get; private set; }

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

#if GAMESERVICES_GAMEANALYTICS
        [Header("Analytics Managers")]
        [SerializeField] private GameAnalyticsManager gameAnalyticsManager;
#endif

#if GAMESERVICES_BYTEBREW
        [SerializeField] private ByteBrewAnalyticsManager byteBrewAnalyticsManager;
#endif

#if GAMESERVICES_FIREBASE
        [SerializeField] private FirebaseAnalyticsManager firebaseAnalyticsManager;
#endif

#if GAMESERVICES_UNITY_ANALYTICS
        [SerializeField] private UnityAnalyticsManager unityAnalyticsManager;
#endif
        
#if GAMESERVICES_UNITY_IAP
[SerializeField] private IAPManager iapManager;
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

            CreateRequiredAdManagers();
            CreateRequiredAnalyticsManagers();
            InitializeAds();
            InitializeIAP();
            InitializeAnalytics();
        }

        private Transform GetOrCreateChild(string childName)
        {
            return GetOrCreateChild(transform, childName);
        }

        private static Transform GetOrCreateChild(Transform parent, string childName)
        {
            var child = parent.Find(childName);
            if (child != null)
                return child;

            var go = new GameObject(childName);
            go.transform.SetParent(parent, false);
            return go.transform;
        }

        private void CreateRequiredAdManagers()
        {
            var adsRoot = GetOrCreateChild("Ads");
#if GAMESERVICES_LEVELPLAY
            if (levelPlayAdsManager == null)
            {
                var holder = GetOrCreateChild(adsRoot, "LevelPlayAdsManager");
                levelPlayAdsManager = holder.GetComponent<LevelPlayAdsManager>();
                if (levelPlayAdsManager == null) levelPlayAdsManager = holder.gameObject.AddComponent<LevelPlayAdsManager>();
            }
#endif

#if GAMESERVICES_ADMOB
            if (adMobAdsManager == null)
            {
                var holder = GetOrCreateChild(adsRoot, "AdMobAdsManager");
                adMobAdsManager = holder.GetComponent<AdMobAdsManager>();
                if (adMobAdsManager == null) adMobAdsManager = holder.gameObject.AddComponent<AdMobAdsManager>();
            }
#endif

#if GAMESERVICES_APPLOVIN
            if (appLovinAdsManager == null)
            {
                var holder = GetOrCreateChild(adsRoot, "AppLovinAdsManager");
                appLovinAdsManager = holder.GetComponent<AppLovinAdsManager>();
                if (appLovinAdsManager == null) appLovinAdsManager = holder.gameObject.AddComponent<AppLovinAdsManager>();
            }
#endif
            
        }

        private void CreateRequiredAnalyticsManagers()
        {
            var analyticsRoot = GetOrCreateChild("Analytics");
#if GAMESERVICES_GAMEANALYTICS
            if (gameAnalyticsManager == null)
            {
                var holder = GetOrCreateChild(analyticsRoot, "GameAnalyticsManager");
                gameAnalyticsManager = holder.GetComponent<GameAnalyticsManager>();
                if (gameAnalyticsManager == null) gameAnalyticsManager = holder.gameObject.AddComponent<GameAnalyticsManager>();
            }
#endif

#if GAMESERVICES_BYTEBREW
            if (byteBrewAnalyticsManager == null)
            {
                var holder = GetOrCreateChild(analyticsRoot, "ByteBrewAnalyticsManager");
                byteBrewAnalyticsManager = holder.GetComponent<ByteBrewAnalyticsManager>();
                if (byteBrewAnalyticsManager == null) byteBrewAnalyticsManager = holder.gameObject.AddComponent<ByteBrewAnalyticsManager>();
            }
#endif

#if GAMESERVICES_FIREBASE
            if (firebaseAnalyticsManager == null)
            {
                var holder = GetOrCreateChild(analyticsRoot, "FirebaseAnalyticsManager");
                firebaseAnalyticsManager = holder.GetComponent<FirebaseAnalyticsManager>();
                if (firebaseAnalyticsManager == null) firebaseAnalyticsManager = holder.gameObject.AddComponent<FirebaseAnalyticsManager>();
            }
#endif

#if GAMESERVICES_UNITY_ANALYTICS
            if (unityAnalyticsManager == null)
            {
                var holder = GetOrCreateChild(analyticsRoot, "UnityAnalyticsManager");
                unityAnalyticsManager = holder.GetComponent<UnityAnalyticsManager>();
                if (unityAnalyticsManager == null) unityAnalyticsManager = holder.gameObject.AddComponent<UnityAnalyticsManager>();
            }
#endif
        }

        private void InitializeAnalytics()
        {
            var analyticsConfig = config.analyticsConfig;

#if GAMESERVICES_GAMEANALYTICS
            if (gameAnalyticsManager != null)
            {
                bool active = (config.analyticsProvider & AnalyticsProvider.GameAnalytics) != 0;
                gameAnalyticsManager.SetConfig(analyticsConfig);
                gameAnalyticsManager.gameObject.SetActive(active);
                if (active) gameAnalyticsManager.Initialize();
            }
#endif

#if GAMESERVICES_BYTEBREW
            if (byteBrewAnalyticsManager != null)
            {
                bool active = (config.analyticsProvider & AnalyticsProvider.ByteBrew) != 0;
                byteBrewAnalyticsManager.SetConfig(analyticsConfig);
                byteBrewAnalyticsManager.gameObject.SetActive(active);
                if (active) byteBrewAnalyticsManager.Initialize();
            }
#endif

#if GAMESERVICES_FIREBASE
            if (firebaseAnalyticsManager != null)
            {
                bool active = (config.analyticsProvider & AnalyticsProvider.Firebase) != 0;
                firebaseAnalyticsManager.SetConfig(analyticsConfig);
                firebaseAnalyticsManager.gameObject.SetActive(active);
                if (active) firebaseAnalyticsManager.Initialize();
            }
#endif

#if GAMESERVICES_UNITY_ANALYTICS
            if (unityAnalyticsManager != null)
            {
                bool active = (config.analyticsProvider & AnalyticsProvider.UnityAnalytics) != 0;
                unityAnalyticsManager.SetConfig(analyticsConfig);
                unityAnalyticsManager.gameObject.SetActive(active);
                if (active) unityAnalyticsManager.Initialize();
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
                if (active) levelPlayAdsManager.Initialize();
            }
#endif

#if GAMESERVICES_ADMOB
            if (adMobAdsManager != null)
            {
                var active = config.adsProvider == AdsProvider.AdMobDirect;
                adMobAdsManager.SetConfig(config.adsConfig);
                adMobAdsManager.gameObject.SetActive(active);
                if (active) adMobAdsManager.Initialize();
            }
#endif

#if GAMESERVICES_APPLOVIN
            if (appLovinAdsManager != null)
            {
                var active = config.adsProvider == AdsProvider.AppLovinMax;
                appLovinAdsManager.SetConfig(config.adsConfig);
                appLovinAdsManager.gameObject.SetActive(active);
                if (active) appLovinAdsManager.Initialize();
            }
#endif
        }
        
        private void InitializeIAP()
        {
#if GAMESERVICES_UNITY_IAP
    if (config.iapConfig == null || !config.iapConfig.enableIAP)
        return;

    if (iapManager == null)
    {
        Transform iapRoot = transform.Find("IAP");

        if (iapRoot == null)
        {
            GameObject obj = new GameObject("IAP");
            obj.transform.SetParent(transform);
            iapRoot = obj.transform;
        }

        iapManager = iapRoot.GetComponent<IAPManager>();

        if (iapManager == null)
            iapManager = iapRoot.gameObject.AddComponent<IAPManager>();
    }

    iapManager.Initialize(config.iapConfig);
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
                    Instance.levelPlayAdsManager?.ShowRewarded(placement, onClosed);
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

        // =====================================================
        // ANALYTICS
        // =====================================================

        public static void LevelStart(int levelNumber, string levelName = null)
        {
            if (Instance == null) return;

#if GAMESERVICES_GAMEANALYTICS
            Instance.gameAnalyticsManager?.LevelStart(levelNumber, levelName);
#endif
#if GAMESERVICES_BYTEBREW
            Instance.byteBrewAnalyticsManager?.LevelStart(levelNumber, levelName);
#endif
#if GAMESERVICES_FIREBASE
            Instance.firebaseAnalyticsManager?.LevelStart(levelNumber, levelName);
#endif
#if GAMESERVICES_UNITY_ANALYTICS
            Instance.unityAnalyticsManager?.LevelStart(levelNumber, levelName);
#endif
        }

        public static void LevelEnd(int levelNumber, bool completed, string levelName = null)
        {
            if (Instance == null) return;

#if GAMESERVICES_GAMEANALYTICS
            Instance.gameAnalyticsManager?.LevelEnd(levelNumber, completed, levelName);
#endif
#if GAMESERVICES_BYTEBREW
            Instance.byteBrewAnalyticsManager?.LevelEnd(levelNumber, completed, levelName);
#endif
#if GAMESERVICES_FIREBASE
            Instance.firebaseAnalyticsManager?.LevelEnd(levelNumber, completed, levelName);
#endif
#if GAMESERVICES_UNITY_ANALYTICS
            Instance.unityAnalyticsManager?.LevelEnd(levelNumber, completed, levelName);
#endif
        }

        public static void IAPPackPurchase(string packId, string currencyCode, decimal price, string transactionId = null)
        {
            if (Instance == null) return;

#if GAMESERVICES_GAMEANALYTICS
            Instance.gameAnalyticsManager?.IAPPackPurchase(packId, currencyCode, price, transactionId);
#endif
#if GAMESERVICES_BYTEBREW
            Instance.byteBrewAnalyticsManager?.IAPPackPurchase(packId, currencyCode, price, transactionId);
#endif
#if GAMESERVICES_FIREBASE
            Instance.firebaseAnalyticsManager?.IAPPackPurchase(packId, currencyCode, price, transactionId);
#endif
#if GAMESERVICES_UNITY_ANALYTICS
            Instance.unityAnalyticsManager?.IAPPackPurchase(packId, currencyCode, price, transactionId);
#endif
        }

        public static void RewardedAdWatched(string placement)
        {
            if (Instance == null) return;

#if GAMESERVICES_GAMEANALYTICS
            Instance.gameAnalyticsManager?.RewardedAdWatched(placement);
#endif
#if GAMESERVICES_BYTEBREW
            Instance.byteBrewAnalyticsManager?.RewardedAdWatched(placement);
#endif
#if GAMESERVICES_FIREBASE
            Instance.firebaseAnalyticsManager?.RewardedAdWatched(placement);
#endif
#if GAMESERVICES_UNITY_ANALYTICS
            Instance.unityAnalyticsManager?.RewardedAdWatched(placement);
#endif
        }
        
        public static void InterstitialAdWatched(string placement)
        {
            if (Instance == null) return;

#if GAMESERVICES_GAMEANALYTICS
            Instance.gameAnalyticsManager?.InterstitialAdWatched(placement);
#endif
#if GAMESERVICES_BYTEBREW
            Instance.byteBrewAnalyticsManager?.InterstitialAdWatched(placement);
#endif
#if GAMESERVICES_FIREBASE
            Instance.firebaseAnalyticsManager?.InterstitialAdWatched(placement);
#endif
#if GAMESERVICES_UNITY_ANALYTICS
            Instance.unityAnalyticsManager?.InterstitialAdWatched(placement);
#endif
        }
        
        public static void Purchase(string productName)
        {
#if GAMESERVICES_UNITY_IAP
    Instance?.iapManager?.Purchase(productName);
#else
            Debug.LogWarning("[GameServices] Unity IAP symbol missing.");
#endif
        }

        public static void RestorePurchases()
        {
#if GAMESERVICES_UNITY_IAP
    Instance?.iapManager?.RestorePurchases();
#endif
        }

        public static bool IsPurchased(string productId)
        {
#if GAMESERVICES_UNITY_IAP
    return Instance != null &&
           Instance.iapManager != null &&
           Instance.iapManager.IsPurchased(productId);
#else
            return false;
#endif
        }

        public static string GetPrice(string productId)
        {
#if GAMESERVICES_UNITY_IAP
    if (Instance == null || Instance.iapManager == null)
        return string.Empty;

    return Instance.iapManager.GetLocalizedPrice(productId);
#else
            return string.Empty;
#endif
        }
    }
}