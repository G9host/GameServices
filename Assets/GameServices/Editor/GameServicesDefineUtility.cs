#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace GameServices.EditorTools
{
    public static class GameServicesDefineUtility
    {
        public const string LevelPlay = "GAMESERVICES_LEVELPLAY";
        public const string AdMob = "GAMESERVICES_ADMOB";
        public const string AppLovin = "GAMESERVICES_APPLOVIN";

        public const string GameAnalytics = "GAMESERVICES_GAMEANALYTICS";
        public const string ByteBrew = "GAMESERVICES_BYTEBREW";
        public const string Firebase = "GAMESERVICES_FIREBASE";
        public const string UnityAnalytics = "GAMESERVICES_UNITY_ANALYTICS";
        
        public const string UnityIAP = "GAMESERVICES_UNITY_IAP";

        public static readonly string[] AllSymbols =
        {
            LevelPlay,
            AdMob,
            AppLovin,
            GameAnalytics,
            ByteBrew,
            Firebase,
            UnityAnalytics,
            UnityIAP
        };

        public static void RemoveAllAndAdd(IEnumerable<string> symbolsToAdd)
        {
            var defines = new HashSet<string>(GetDefines());

            foreach (var symbol in AllSymbols)
                defines.Remove(symbol);

            foreach (var symbol in symbolsToAdd.Where(s => !string.IsNullOrWhiteSpace(s)))
                defines.Add(symbol);

            SetDefines(defines.OrderBy(s => s));
        }

        public static void RemoveAll()
        {
            var defines = new HashSet<string>(GetDefines());

            foreach (var symbol in AllSymbols)
                defines.Remove(symbol);

            SetDefines(defines.OrderBy(s => s));
        }

        private static IEnumerable<string> GetDefines()
        {
#if UNITY_2021_2_OR_NEWER
            var buildTarget = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            return PlayerSettings.GetScriptingDefineSymbols(buildTarget)
                .Split(';')
                .Where(s => !string.IsNullOrWhiteSpace(s));
#else
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup)
                .Split(';')
                .Where(s => !string.IsNullOrWhiteSpace(s));
#endif
        }

        private static void SetDefines(IEnumerable<string> defines)
        {
            var value = string.Join(";", defines);

#if UNITY_2021_2_OR_NEWER
            var buildTarget = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            PlayerSettings.SetScriptingDefineSymbols(buildTarget, value);
#else
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, value);
#endif
        }
    }
}
#endif
