using GameServices.Ads;
using GameServices.Analytics;
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

        [Header("Scripting Define Symbols")]
        [Tooltip("Editor button removes all GameServices symbols first, then adds symbols based on Ads Provider and Analytics Provider above.")]
        public bool removeAllGameServicesSymbolsBeforeApply = true;

        [Header("Configs")] 
        public IAPConfig iapConfig;
        public AdsConfig adsConfig;
        public AnalyticsConfig analyticsConfig;
    }
}