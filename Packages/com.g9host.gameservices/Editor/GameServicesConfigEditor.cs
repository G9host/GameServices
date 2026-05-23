#if UNITY_EDITOR
using System.Collections.Generic;
using GameServices.Core;
using UnityEditor;
using UnityEngine;

namespace GameServices.EditorTools
{
    [CustomEditor(typeof(GameServicesConfig))]
    public class GameServicesConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();

            EditorGUILayout.Space(12);
            EditorGUILayout.LabelField("GameServices Symbols", EditorStyles.boldLabel);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Apply GameServices Symbols", GUILayout.Height(28)))
                {
                    var config = (GameServicesConfig)target;
                    GameServicesDefineUtility.RemoveAllAndAdd(GetSelectedSymbols(config));
                    EditorUtility.SetDirty(config);
                    AssetDatabase.SaveAssets();
                    Debug.Log("[GameServicesConfig] Removed all GameServices symbols, then added symbols from selected Ads + Analytics config.");
                }

                if (GUILayout.Button("Remove All", GUILayout.Height(28)))
                {
                    GameServicesDefineUtility.RemoveAll();
                    Debug.Log("[GameServicesConfig] Removed all GameServices scripting define symbols.");
                }
            }

            EditorGUILayout.HelpBox(
                "Open the GameServicesConfig asset directly to see this button. It will not appear inside the object field on the GameServices component.",
                MessageType.Info);

            serializedObject.ApplyModifiedProperties();
        }

        private static IEnumerable<string> GetSelectedSymbols(GameServicesConfig config)
        {
            switch (config.adsProvider)
            {
                case AdsProvider.LevelPlay:
                    yield return GameServicesDefineUtility.LevelPlay;
                    break;
                case AdsProvider.AdMobDirect:
                    yield return GameServicesDefineUtility.AdMob;
                    break;
                case AdsProvider.AppLovinMax:
                    yield return GameServicesDefineUtility.AppLovin;
                    break;
            }

            if ((config.analyticsProvider & AnalyticsProvider.GameAnalytics) != 0)
                yield return GameServicesDefineUtility.GameAnalytics;

            if ((config.analyticsProvider & AnalyticsProvider.ByteBrew) != 0)
                yield return GameServicesDefineUtility.ByteBrew;

            if ((config.analyticsProvider & AnalyticsProvider.Firebase) != 0)
                yield return GameServicesDefineUtility.Firebase;

            if ((config.analyticsProvider & AnalyticsProvider.UnityAnalytics) != 0)
                yield return GameServicesDefineUtility.UnityAnalytics;
            
            if (config.iapConfig.enableIAP)
                yield return GameServicesDefineUtility.UnityIAP;
            
        }
    }
}
#endif
