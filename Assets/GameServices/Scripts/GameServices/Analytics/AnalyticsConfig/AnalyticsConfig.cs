using UnityEngine;
using GameServices.Core;

namespace GameServices.Analytics
{
    [CreateAssetMenu(
        fileName = "AnalyticsConfig",
        menuName = "Game Services/Analytics Config"
    )]
    public class AnalyticsConfig : ScriptableObject
    {
        [Header("GameAnalytics")]
        public string gameAnalyticsGameKeyAndroid;
        public string gameAnalyticsSecretKeyAndroid;
        public string gameAnalyticsGameKeyIOS;
        public string gameAnalyticsSecretKeyIOS;

        [Header("ByteBrew")]
        public string byteBrewGameIdAndroid;
        public string byteBrewGameIdIOS;
        public string byteBrewSdkKeyAndroid;
        public string byteBrewSdkKeyIOS;

        [Header("Firebase Analytics")]
        [Tooltip("Firebase normally reads config from google-services.json / GoogleService-Info.plist.")]
        public string firebaseAppIdOverride;

        [Header("Unity Analytics")]
        [Tooltip("Unity Analytics normally uses Unity Services project settings.")]
        public string unityEnvironmentName;

        public string GameAnalyticsGameKey
        {
            get
            {
#if UNITY_ANDROID
                return gameAnalyticsGameKeyAndroid;
#elif UNITY_IOS
                return gameAnalyticsGameKeyIOS;
#else
                return string.Empty;
#endif
            }
        }

        public string GameAnalyticsSecretKey
        {
            get
            {
#if UNITY_ANDROID
                return gameAnalyticsSecretKeyAndroid;
#elif UNITY_IOS
                return gameAnalyticsSecretKeyIOS;
#else
                return string.Empty;
#endif
            }
        }

        public string ByteBrewGameId
        {
            get
            {
#if UNITY_ANDROID
                return byteBrewGameIdAndroid;
#elif UNITY_IOS
                return byteBrewGameIdIOS;
#else
                return string.Empty;
#endif
            }
        }

        public string ByteBrewSdkKey
        {
            get
            {
#if UNITY_ANDROID
                return byteBrewSdkKeyAndroid;
#elif UNITY_IOS
                return byteBrewSdkKeyIOS;
#else
                return string.Empty;
#endif
            }
        }
    }
}
