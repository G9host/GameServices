using UnityEngine;

#if GAMESERVICES_FIREBASE
using Firebase.Analytics;
#endif

namespace GameServices.Analytics
{
    public class FirebaseAnalyticsManager : MonoBehaviour
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

#if GAMESERVICES_FIREBASE
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            initialized = true;
            Debug.Log("[FirebaseAnalyticsManager] Initialized.");
#else
            Debug.LogWarning("[FirebaseAnalyticsManager] GAMESERVICES_FIREBASE define is missing.");
#endif
        }

        public void LevelStart(int levelNumber, string levelName = null)
        {
#if GAMESERVICES_FIREBASE
            if (!initialized) return;
            FirebaseAnalytics.LogEvent("level_start",
                new Parameter("level_number", levelNumber),
                new Parameter("level_name", Safe(levelName)));
#endif
        }

        public void LevelEnd(int levelNumber, bool completed, string levelName = null)
        {
#if GAMESERVICES_FIREBASE
            if (!initialized) return;
            FirebaseAnalytics.LogEvent("level_end",
                new Parameter("level_number", levelNumber),
                new Parameter("level_name", Safe(levelName)),
                new Parameter("completed", completed ? 1 : 0));
#endif
        }

        public void IAPPackPurchase(string packId, string currencyCode, decimal price, string transactionId = null)
        {
#if GAMESERVICES_FIREBASE
            if (!initialized) return;
            FirebaseAnalytics.LogEvent("iap_pack_purchase",
                new Parameter("pack_id", Safe(packId)),
                new Parameter("currency", Safe(currencyCode)),
                new Parameter("price", decimal.ToDouble(price)),
                new Parameter("transaction_id", Safe(transactionId)));
#endif
        }

        public void RewardedAdWatched(string placement)
        {
#if GAMESERVICES_FIREBASE
            if (!initialized) return;
            FirebaseAnalytics.LogEvent("rewarded_ad_watched",
                new Parameter("placement", Safe(placement)));
#endif
        }

        private static string Safe(string value)
        {
            return string.IsNullOrEmpty(value) ? "default" : value;
        }
    }
}
