using UnityEngine;

#if GAMESERVICES_GAMEANALYTICS
using GameAnalyticsSDK;
#endif

namespace GameServices.Analytics
{
    public class GameAnalyticsManager : MonoBehaviour
    {
        private AnalyticsConfig config;
        private bool initialized;

        public void SetConfig(AnalyticsConfig analyticsConfig)
        {
            config = analyticsConfig;
        }

        public void Initialize()
        {
            if (initialized)
                return;

#if GAMESERVICES_GAMEANALYTICS
            if (config == null)
            {
                Debug.LogError("[GameAnalyticsManager] AnalyticsConfig missing.");
                return;
            }

            if (!string.IsNullOrEmpty(config.GameAnalyticsGameKey))
                GameAnalytics.SetCustomId(SystemInfo.deviceUniqueIdentifier);

            GameAnalytics.Initialize();
            initialized = true;
            Debug.Log("[GameAnalyticsManager] Initialized.");
#else
            Debug.LogWarning("[GameAnalyticsManager] GAMESERVICES_GAMEANALYTICS define is missing.");
#endif
        }

        public void LevelStart(int levelNumber, string levelName = null)
        {
#if GAMESERVICES_GAMEANALYTICS
            if (!initialized) return;
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, GetLevelId(levelNumber, levelName));
#endif
        }

        public void LevelEnd(int levelNumber, bool completed, string levelName = null)
        {
#if GAMESERVICES_GAMEANALYTICS
            if (!initialized) return;
            GameAnalytics.NewProgressionEvent(
                completed ? GAProgressionStatus.Complete : GAProgressionStatus.Fail,
                GetLevelId(levelNumber, levelName));
#endif
        }

        public void IAPPackPurchase(string packId, string currencyCode, decimal price, string transactionId = null)
        {
#if GAMESERVICES_GAMEANALYTICS
            if (!initialized) return;
            GameAnalytics.NewDesignEvent($"iap_purchase:{packId}:{currencyCode}", (float)price);
#endif
        }

        public void RewardedAdWatched(string placement)
        {
#if GAMESERVICES_GAMEANALYTICS
            if (!initialized) return;
            GameAnalytics.NewAdEvent(GAAdAction.RewardReceived, GAAdType.RewardedVideo, "GameServices", Safe(placement));
#endif
        }
        
        public void InterstitialAdWatched(string placement)
        {
#if GAMESERVICES_GAMEANALYTICS
            if (!initialized) return;
            GameAnalytics.NewAdEvent(GAAdAction.RewardReceived, GAAdType.Interstitial, "GameServices", Safe(placement));
#endif
        }

        private static string GetLevelId(int levelNumber, string levelName)
        {
            return string.IsNullOrEmpty(levelName) ? $"Level_{levelNumber}" : levelName;
        }

        private static string Safe(string value)
        {
            return string.IsNullOrEmpty(value) ? "default" : value;
        }
    }
}
