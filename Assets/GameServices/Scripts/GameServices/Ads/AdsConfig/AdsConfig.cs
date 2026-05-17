using UnityEngine;
using GameServices.Core;

namespace GameServices.Ads
{
    public enum BannerPosition
    {
        Top,
        Bottom
    }

    public enum BannerSize
    {
        Standard,
        Large,
        MediumRectangle,
        Adaptive
    }

    [CreateAssetMenu(
        fileName = "AdsConfig",
        menuName = "Game Services/Ads Config"
    )]
    public class AdsConfig : ScriptableObject
    {
        // =========================================================
        // COMMON SETTINGS
        // =========================================================

        [Header("Common Settings")]

        [Tooltip("Enable / Disable Interstitial Ads")]
        public bool enableInterstitial = true;

        [Tooltip("Automatically load next interstitial ad")]
        public bool autoLoadInterstitial = true;

        [Space]

        [Tooltip("Enable / Disable Rewarded Ads")]
        public bool enableRewarded = true;

        [Tooltip("Automatically load next rewarded ad")]
        public bool autoLoadRewarded = true;

        [Space]

        [Tooltip("Enable / Disable Banner Ads")]
        public bool enableBanner = false;

        [Tooltip("Automatically load banner ads")]
        public bool autoLoadBanner = true;

        [Tooltip("Show banner automatically after loading")]
        public bool showBannerOnLoad = false;

        [Space]

        [Tooltip("Banner screen position")]
        public BannerPosition bannerPosition = BannerPosition.Bottom;

        [Tooltip("Banner size type")]
        public BannerSize bannerSize = BannerSize.Standard;

        [Tooltip("Respect safe area/notch on devices")]
        public bool respectSafeArea = true;

        // =========================================================
        // LEVELPLAY
        // =========================================================

        [Header("LevelPlay")]

        [Tooltip("LevelPlay App Key")]
        public string levelPlayAppKey;

       
        public string levelPlayInterstitialAndroidId;
        public string levelPlayInterstitialIOSId;

        
        public string levelPlayRewardedAndroidId;
        public string levelPlayRewardedIOSId;

        
        public string levelPlayBannerAndroidId;
        public string levelPlayBannerIOSId;

        
        public string levelPlayRewardedPlacement = "DefaultRewarded";

        // =========================================================
        // ADMOB
        // =========================================================

        [Header("AdMob")]

        [Tooltip("AdMob App ID")]
        public string admobAppId;

        
        public string admobInterstitialAndroidId;
        public string admobInterstitialIOSId;

        
        public string admobRewardedAndroidId;
        public string admobRewardedIOSId;

        
        public string admobBannerAndroidId;
        public string admobBannerIOSId;

        // =========================================================
        // APPLOVIN MAX
        // =========================================================

        [Header("AppLovin MAX")]

        [Tooltip("AppLovin MAX SDK Key")]
        public string appLovinSdkKey;

        
        public string appLovinInterstitialAndroidId;
        public string appLovinInterstitialIOSId;

       
        public string appLovinRewardedAndroidId;
        public string appLovinRewardedIOSId;

       
        public string appLovinBannerAndroidId;
        public string appLovinBannerIOSId;
        
        
        [Header("Cooldowns")]
        [Tooltip("Minimum seconds between two interstitial ads")]
        public float interstitialCooldown = 60f;

        [Tooltip("Minimum seconds between two rewarded ads")]
        public float rewardedCooldown = 10f;

        [Tooltip("Minimum seconds between any full-screen ads: interstitial or rewarded")]
        public float globalFullscreenAdCooldown = 30f;


        // =========================================================
        // HELPERS
        // =========================================================

        public string GetInterstitialId(AdsProvider provider)
        {
            switch (provider)
            {
                case AdsProvider.LevelPlay:
#if UNITY_ANDROID
                    return levelPlayInterstitialAndroidId;
#elif UNITY_IOS
                    return levelPlayInterstitialIOSId;
#else
                    return string.Empty;
#endif

                case AdsProvider.AdMobDirect:
#if UNITY_ANDROID
                    return admobInterstitialAndroidId;
#elif UNITY_IOS
                    return admobInterstitialIOSId;
#else
                    return string.Empty;
#endif

                case AdsProvider.AppLovinMax:
#if UNITY_ANDROID
                    return appLovinInterstitialAndroidId;
#elif UNITY_IOS
                    return appLovinInterstitialIOSId;
#else
                    return string.Empty;
#endif

                default:
                    return string.Empty;
            }
        }

        public string GetRewardedId(AdsProvider provider)
        {
            switch (provider)
            {
                case AdsProvider.LevelPlay:
#if UNITY_ANDROID
                    return levelPlayRewardedAndroidId;
#elif UNITY_IOS
                    return levelPlayRewardedIOSId;
#else
                    return string.Empty;
#endif

                case AdsProvider.AdMobDirect:
#if UNITY_ANDROID
                    return admobRewardedAndroidId;
#elif UNITY_IOS
                    return admobRewardedIOSId;
#else
                    return string.Empty;
#endif

                case AdsProvider.AppLovinMax:
#if UNITY_ANDROID
                    return appLovinRewardedAndroidId;
#elif UNITY_IOS
                    return appLovinRewardedIOSId;
#else
                    return string.Empty;
#endif

                default:
                    return string.Empty;
            }
        }

        public string GetBannerId(AdsProvider provider)
        {
            switch (provider)
            {
                case AdsProvider.LevelPlay:
#if UNITY_ANDROID
                    return levelPlayBannerAndroidId;
#elif UNITY_IOS
                    return levelPlayBannerIOSId;
#else
                    return string.Empty;
#endif

                case AdsProvider.AdMobDirect:
#if UNITY_ANDROID
                    return admobBannerAndroidId;
#elif UNITY_IOS
                    return admobBannerIOSId;
#else
                    return string.Empty;
#endif

                case AdsProvider.AppLovinMax:
#if UNITY_ANDROID
                    return appLovinBannerAndroidId;
#elif UNITY_IOS
                    return appLovinBannerIOSId;
#else
                    return string.Empty;
#endif

                default:
                    return string.Empty;
            }
        }

        public string GetAppKey(AdsProvider provider)
        {
            switch (provider)
            {
                case AdsProvider.LevelPlay:
                    return levelPlayAppKey;

                case AdsProvider.AdMobDirect:
                    return admobAppId;

                case AdsProvider.AppLovinMax:
                    return appLovinSdkKey;

                default:
                    return string.Empty;
            }
        }
    }
}