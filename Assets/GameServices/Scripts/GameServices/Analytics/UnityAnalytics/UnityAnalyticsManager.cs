using System.Collections.Generic;
using UnityEngine;

#if GAMESERVICES_UNITY_ANALYTICS
using Unity.Services.Analytics;
using Unity.Services.Core;
#endif

namespace GameServices.Analytics
{
    public class UnityAnalyticsManager : MonoBehaviour
    {
        private AnalyticsConfig config;
        private bool initialized;

        public void SetConfig(AnalyticsConfig analyticsConfig)
        {
            config = analyticsConfig;
        }

        public async void Initialize()
        {
            if (initialized)
                return;

#if GAMESERVICES_UNITY_ANALYTICS
            try
            {
                if (UnityServices.State == ServicesInitializationState.Uninitialized)
                    await UnityServices.InitializeAsync();

                AnalyticsService.Instance.StartDataCollection();
                initialized = true;
                Debug.Log("[UnityAnalyticsManager] Initialized.");
            }
            catch (System.Exception exception)
            {
                Debug.LogError($"[UnityAnalyticsManager] Initialize failed: {exception.Message}");
            }
#else
            Debug.LogWarning("[UnityAnalyticsManager] GAMESERVICES_UNITY_ANALYTICS define is missing.");
#endif
        }

        public void LevelStart(int levelNumber, string levelName = null)
        {
#if GAMESERVICES_UNITY_ANALYTICS
            if (!initialized) return;
            AnalyticsService.Instance.CustomData("level_start", new Dictionary<string, object>
            {
                { "level_number", levelNumber },
                { "level_name", Safe(levelName) }
            });
#endif
        }

        public void LevelEnd(int levelNumber, bool completed, string levelName = null)
        {
#if GAMESERVICES_UNITY_ANALYTICS
            if (!initialized) return;
            AnalyticsService.Instance.CustomData("level_end", new Dictionary<string, object>
            {
                { "level_number", levelNumber },
                { "level_name", Safe(levelName) },
                { "completed", completed }
            });
#endif
        }

        public void IAPPackPurchase(string packId, string currencyCode, decimal price, string transactionId = null)
        {
#if GAMESERVICES_UNITY_ANALYTICS
            if (!initialized) return;
            AnalyticsService.Instance.CustomData("iap_pack_purchase", new Dictionary<string, object>
            {
                { "pack_id", Safe(packId) },
                { "currency", Safe(currencyCode) },
                { "price", decimal.ToDouble(price) },
                { "transaction_id", Safe(transactionId) }
            });
#endif
        }

        public void RewardedAdWatched(string placement)
        {
#if GAMESERVICES_UNITY_ANALYTICS
            if (!initialized) return;
            AnalyticsService.Instance.CustomData("rewarded_ad_watched", new Dictionary<string, object>
            {
                { "placement", Safe(placement) }
            });
#endif
        }

        private static string Safe(string value)
        {
            return string.IsNullOrEmpty(value) ? "default" : value;
        }
    }
}
