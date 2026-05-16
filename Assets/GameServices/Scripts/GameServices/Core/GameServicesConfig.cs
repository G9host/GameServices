using GameServices.Ads;
using UnityEngine;

namespace GameServices.Core
{
    public enum AdsProvider
    {
        None,
        LevelPlay,
        AdMobDirect,
        AppLovinMax
    }

    [System.Flags]
    public enum AnalyticsProvider
    {
        None            = 0,
        GameAnalytics   = 1 << 0,
        ByteBrew        = 1 << 1,
        Firebase        = 1 << 2,
        UnityAnalytics  = 1 << 3
    }

    [CreateAssetMenu(
        fileName = "GameServicesConfig",
        menuName = "Game Services/Config"
    )]
    public class GameServicesConfig : ScriptableObject
    {
        [Header("Ads")]
        public AdsProvider adsProvider = AdsProvider.None;

        [Header("Analytics")]
        public AnalyticsProvider analyticsProvider = AnalyticsProvider.None;

        [Header("IAP")]
        public bool enableIAP = false;
        
        [Header("Configs")]
        public AdsConfig adsConfig;
    }
}