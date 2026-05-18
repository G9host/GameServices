using UnityEngine;

#if GAMESERVICES_BYTEBREW
using ByteBrewSDK;
#endif

namespace GameServices.Analytics
{
    public class ByteBrewAnalyticsManager : MonoBehaviour
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

#if GAMESERVICES_BYTEBREW
            if (config == null)
            {
                Debug.LogError("[ByteBrewAnalyticsManager] AnalyticsConfig missing.");
                return;
            }

            ByteBrew.InitializeByteBrew(config.ByteBrewGameId, config.ByteBrewSdkKey);
            initialized = true;
            Debug.Log("[ByteBrewAnalyticsManager] Initialized.");
#else
            Debug.LogWarning("[ByteBrewAnalyticsManager] GAMESERVICES_BYTEBREW define is missing.");
#endif
        }

        public void LevelStart(int levelNumber, string levelName = null)
        {
#if GAMESERVICES_BYTEBREW
            if (!initialized) return;
            ByteBrew.NewCustomEvent("level_start", LevelPayload(levelNumber, levelName));
#endif
        }

        public void LevelEnd(int levelNumber, bool completed, string levelName = null)
        {
#if GAMESERVICES_BYTEBREW
            if (!initialized) return;
            var payload = LevelPayload(levelNumber, levelName);
            payload += $"|completed:{completed}";
            ByteBrew.NewCustomEvent("level_end", payload);
#endif
        }

        public void IAPPackPurchase(string packId, string currencyCode, decimal price, string transactionId = null)
        {
#if GAMESERVICES_BYTEBREW
            if (!initialized) return;
            ByteBrew.NewCustomEvent("iap_pack_purchase", $"pack_id:{Safe(packId)}|currency:{Safe(currencyCode)}|price:{price}|transaction_id:{Safe(transactionId)}");
#endif
        }

        public void RewardedAdWatched(string placement)
        {
#if GAMESERVICES_BYTEBREW
            if (!initialized) return;
            ByteBrew.NewCustomEvent("rewarded_ad_watched", $"placement:{Safe(placement)}");
#endif
        }

        private static string LevelPayload(int levelNumber, string levelName)
        {
            return $"level_number:{levelNumber}|level_name:{Safe(levelName)}";
        }

        private static string Safe(string value)
        {
            return string.IsNullOrEmpty(value) ? "default" : value;
        }
    }
}
