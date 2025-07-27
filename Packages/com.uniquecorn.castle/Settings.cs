using System.IO;
using Castle.Core;
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
        public static string Identifier => ZString.Format(IdentifierFormat, PlayerSettings.companyName.Slugged(), ProductName);
#endif
        public bool deleteBurstDebugInfo,deleteil2cppDebugInfo;
        public float QuickTapTimerThreshold = 0.2f;
        public float QuickTapDistanceThreshold = 3.5f;
        public int HourOfDayStart = 8;
        public static string ProjectName => Application.dataPath.Split('/')[^2];
        public static string ProjectPath => Application.dataPath.Replace("Assets", "");
        public static string CastlePath => Path.Combine("Packages", "Castle", "Editor");
        public static string ProductName => (Instance.UseAltProductName ? Instance.altProductName : Application.productName).Slugged();
        private const string IdentifierFormat = "com.{0}.{1}";
        [ShowInInspector,HideIf("UseAltProductName"),PropertyOrder(-1)]
        public bool UseAltProductName
        {
            get => !string.IsNullOrEmpty(altProductName);
            set => altProductName = true ? Application.productName.Slugged() : "";
        }
        [ShowIf("UseAltProductName"),Delayed]
        public string altProductName;
        public bool CreateAotForSave => !string.IsNullOrEmpty(saveTypeString);
        [TypeDrawerSettings(BaseType = typeof(CastleSave)),ShowInInspector]
        public System.Type SaveTypeForAot
        {
            get => System.Type.GetType(saveTypeString);
            set => saveTypeString = value == null ? "" : value.AssemblyQualifiedName;
        }
        [HideInInspector]
        public string saveTypeString;
        public string[] frameworks;
        public string[] capabilitiesToAdd;
        public string[] associatedDomains;
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