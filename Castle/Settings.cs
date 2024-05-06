using System.IO;
using Cysharp.Text;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using Sirenix.OdinInspector.Editor;
#endif
namespace Castle
{
    [GlobalConfig("Assets/Resources/Castle"),CreateAssetMenu]
    public class Settings : GlobalConfig<Settings>
    {
        #if UNITY_EDITOR
        private static Editor editor;
        #endif
        public float QuickTapTimerThreshold = 0.2f;
        public float QuickTapDistanceThreshold = 3.5f;
        public int HourOfDayStart = 8;
        public static string ProjectName => Application.dataPath.Split('/')[^2];
        public static string ProjectPath => Application.dataPath.Replace("Assets", "");
        public static string CastlePath => Path.Combine(Application.dataPath, "Castle", "Editor");
        public static string ProductName => (Instance.UseAltProductName ? Instance.altProductName : Application.productName).Slugged();

        private const string IdentifierFormat = "com.{0}.{1}";
        public static string Identifier => ZString.Format(IdentifierFormat, PlayerSettings.companyName.Slugged(), ProductName);
        [ShowInInspector,HideIf("UseAltProductName"),PropertyOrder(-1)]
        public bool UseAltProductName
        {
            get => !string.IsNullOrEmpty(altProductName);
            set => altProductName = true ? Application.productName.Slugged() : "";
        }
        [ShowIf("UseAltProductName"),Delayed]
        public string altProductName;
        [ShowInInspector,HideIf("UseAltXcodeTeam"),PropertyOrder(0)]
        public bool UseAltXcodeTeam
        {
            get => !string.IsNullOrEmpty(altXcodeTeamID);
            set => altXcodeTeamID = true ? (string.IsNullOrEmpty(PlayerSettings.iOS.appleDeveloperTeamID) ? "TEAM" : PlayerSettings.iOS.appleDeveloperTeamID) : "";
        }
        [ShowIf("UseAltXcodeTeam"),Delayed]
        public string altXcodeTeamID;
        public static string XCodeTeamID => Instance.UseAltXcodeTeam ? Instance.altXcodeTeamID : PlayerSettings.iOS.appleDeveloperTeamID;
        public string[] frameworks;
        public Target EmbedSwiftStandardLibraries;
        public bool autoOpenXcode;
        [System.Flags]
        public enum Target
        {
            None = 0,
            Main = 1 << 0,
            Framework = 1 << 1,
        }
#if UNITY_EDITOR
        [SettingsProvider]
        public static SettingsProvider CreatePreferencesGUI()
        {
            return new SettingsProvider("Project/Castle", SettingsScope.Project)
            {
                guiHandler = ( searchContext ) => PreferencesGUI()
            };
        }
        static void PreferencesGUI()
        {
            if (editor == null) editor = Editor.CreateEditor(Instance);
            OdinEditor.ForceHideMonoScriptInEditor = true;
            try
            {
                editor.OnInspectorGUI();
            }
            finally
            {
                OdinEditor.ForceHideMonoScriptInEditor = false;
            }
        }
#endif
    }
}