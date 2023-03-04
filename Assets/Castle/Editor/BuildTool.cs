using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Castle.Core;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace Castle.Editor
{
    public class BuildTool : IPreprocessBuildWithReport,IPostprocessBuildWithReport
    {
        private const string kColorGamutSetting = "0000000003000000";
        public static string GvhPath => Application.dataPath.Replace("Assets", "ProjectSettings/GvhProjectSettings.xml");
        private static string ProjectName => Application.dataPath.Split('/')[^2];
        private static string ProductName => Settings.Instance.useAltProductName ? Settings.Instance.altProductName : Tools.SlugKey(Application.productName);
        private static string KeystoreFileName => ProductName + ".keystore";
        private static string ShortKeystorePath => Path.Combine("Store",KeystoreFileName);
        private static string Alias => ProductName+"key";
        private static string KeystoreDirectory => Path.Combine(Application.dataPath.Replace("Assets", ""), "Store");
        private static string KeyDetails => Path.Combine(KeystoreDirectory, "keyDetails.txt");
        private static string KeystorePath => Path.GetFullPath(Path.Combine(KeystoreDirectory, KeystoreFileName));
        private static bool KeystoreExists => File.Exists(KeystorePath);
        private const string IdentifierFormat = "com.{0}.{1}";
        private static string Identifier => string.Format(IdentifierFormat, Tools.SlugKey(PlayerSettings.companyName), ProductName);
        public static string BuildPath(BuildTarget target) => Path.Combine(Application.dataPath.Replace("Assets", ""), "Build" + target);
        private static (int,string) IncrementVersion(int majorIncr=0, int minorIncr=0, int buildIncr=0)
        {
            var originalBundleVersion = PlayerSettings.bundleVersion;
            var originalBundleCode = PlayerSettings.Android.bundleVersionCode;
            var lines = PlayerSettings.bundleVersion.Split('.');
            if (lines.Length < 3)
            {
                lines = new[] {"0", "0", "1"};
                SetBundleCode(1,"0.0.1");
            }

            var (majorVersion, minorVersion, patchVersion) = (int.Parse(lines[0]) + majorIncr, int.Parse(lines[1]) + minorIncr, int.Parse(lines[2]) + buildIncr);
            if (majorIncr > 0)
            {
                minorVersion = 0;
                patchVersion = 0;
            }
            if (minorIncr > 0)
            {
                patchVersion = 0;
            }
            PlayerSettings.Android.bundleVersionCode = (majorVersion * 10000) + (minorVersion * 100) + patchVersion;
            PlayerSettings.bundleVersion = majorVersion + "." + minorVersion + "." + patchVersion;
            Debug.Log("Build v" + PlayerSettings.bundleVersion + " (" + PlayerSettings.Android.bundleVersionCode + ")");
            return (originalBundleCode, originalBundleVersion);
        }
        private static void SetBundleCode(int code, string version)
        {
            PlayerSettings.Android.bundleVersionCode = code;
            PlayerSettings.bundleVersion = version;
        }
        public static void AddScriptingDefineSymbol(params string[] symbols)
        {
            var scriptingDefinesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup); 
            var scriptingDefinesStringList = scriptingDefinesString.Split(';').ToList();
            scriptingDefinesStringList.AddRange(symbols.Except(scriptingDefinesStringList));
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", scriptingDefinesStringList.ToArray()));
        }
        public static void RemoveScriptingDefineSymbol(params string[] symbols)
        {
            var scriptingDefinesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup); 
            var scriptingDefinesStringList = scriptingDefinesString.Split(';').Except(symbols).ToArray();
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", scriptingDefinesStringList));
        }

        public static bool TryUpdateColorGamut()
        {
            var settingsFilePath = Path.Combine(Application.dataPath, "..", "ProjectSettings", "ProjectSettings.asset");
 
            if (File.Exists(settingsFilePath))
            {
                var currentSettingsContent = File.ReadAllText(settingsFilePath);
                var updatedSettingsContent = System.Text.RegularExpressions.Regex.Replace(currentSettingsContent, @"m_ColorGamuts: ([0-9]+)", "m_ColorGamuts: " + kColorGamutSetting);
                File.WriteAllText(settingsFilePath, updatedSettingsContent);
 
                Debug.Log("Updated color gamut setting to " + kColorGamutSetting);
                return true;
            }
            else
            {
                Debug.LogError("Couldn't change color gamut setting, ProjectSettings file couldn't be found at " + settingsFilePath);
                return false;
            }
        }
        public static bool BuildGame(BuildTarget target = BuildTarget.NoTarget)
        {
            if (target == BuildTarget.NoTarget) target = EditorUserBuildSettings.activeBuildTarget;
            if (!TryUpdateColorGamut()) return false;
            if (!CastleScriptableObjectManager.Instance.ValidateAllScriptableObjects())
            {
                Debug.LogError("Failed build due to bad scriptable object data!");
                return false;
            }
            if (File.Exists(GvhPath + ".bak")) File.Copy(GvhPath + ".bak", GvhPath, true);
            var buildPath = BuildPath(target);
            if (!Directory.Exists(buildPath)) Directory.CreateDirectory(buildPath);
            buildPath += Application.productName;
            var buildPlayerOptions = new BuildPlayerOptions {scenes = EditorBuildSettings.scenes.Where(x => x.enabled).Select(x => x.path).ToArray() };
            if (buildPlayerOptions.target == BuildTarget.Android)
            {
                buildPlayerOptions.targetGroup = BuildTargetGroup.Android;
                buildPlayerOptions.options = BuildOptions.ShowBuiltPlayer;
                if (!EditorUserBuildSettings.buildAppBundle)
                {
                    var dialog =
                        EditorUtility.DisplayDialogComplex("Build Regular APK",
                            "Building version " + PlayerSettings.bundleVersion + " as an APK?", "OK!",
                            "Build as AAB instead!", "Wait...");
                    switch(dialog)
                    {
                        case 1:
                            EditorUserBuildSettings.buildAppBundle = true;
                            break;
                        case 2:
                            return false;
                    }
                }
                else if (!EditorUtility.DisplayDialog("Building App Bundle",
                             "Building version " + PlayerSettings.bundleVersion + " as an AAB?", "OK!", "Wait..."))
                    return false;
                var extension = EditorUserBuildSettings.buildAppBundle ? "aab" : "apk";
                
                buildPath += PlayerSettings.bundleVersion + "(" + PlayerSettings.Android.bundleVersionCode + ")."+extension;
            }
            else if (buildPlayerOptions.target == BuildTarget.iOS)
            {
                buildPlayerOptions.targetGroup = BuildTargetGroup.iOS;
                buildPlayerOptions.options = BuildOptions.ShowBuiltPlayer;
            }
            buildPlayerOptions.locationPathName = buildPath;
            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            Debug.Log(report.summary.result);
            return report.summary.result == BuildResult.Succeeded;
        }
        [MenuItem("Build/Rebuild %b", false, 10000)]
        private static void Rebuild()
        {
            IncrementVersion();
            BuildGame();
        }
        [MenuItem("Build/Hotfix %#b", false, 10000)]
        private static void Hotfix()
        {
            var (bundleCode, bundleString) = IncrementVersion(0, 0, 1);
            var success = false;
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
            {
                success = BuildGame(BuildTarget.iOS);
            }
            else
            {
                success = BuildGame(BuildTarget.Android);
            }
            if (!success)
            {
                SetBundleCode(bundleCode,bundleString);
            }
        }
        public static bool SetKey()
        {
            PlayerSettings.SplashScreen.show = false;
            if (!KeystoreExists)
            {
                if (!CreateKeystore())
                {
                    return false;
                }
            }
            PlayerSettings.SetApplicationIdentifier(EditorUserBuildSettings.selectedBuildTargetGroup,Identifier);
            var details = JsonUtility.FromJson<Key>(Tools.ReadTextFile(KeyDetails));
            PlayerSettings.Android.useCustomKeystore=true;
            PlayerSettings.Android.keystoreName = ShortKeystorePath;
            PlayerSettings.Android.keystorePass = details.StorePass;
            PlayerSettings.Android.keyaliasName = Alias;
            PlayerSettings.Android.keyaliasPass = details.KeyPass;
            return true;
        }
        [MenuItem("Build/Create Keystore")]
        public static bool CreateKeystore()
        {
            if (PlayerSettings.companyName == "DefaultCompany")
            {
                EditorUtility.DisplayDialog("Can't create keystore!", "Company name is not set in PlayerSettings",
                    "Oops...");
                return false;
            }

            if (Settings.Instance.useAltProductName)
            {
                if (!EditorUtility.DisplayDialog("Use product name " + Settings.Instance.altProductName + "?",
                        "Identifier will be set to " + Identifier, "Okay!", "NO! CANCEL!!!"))
                {
                    return false;
                }
            }
            else if (Application.productName != ProjectName)
            {
                switch (EditorUtility.DisplayDialogComplex("Which product name do you want to use?",
                        "It can either be " + ProjectName + " or " + Application.productName + ".",
                        string.Format(IdentifierFormat, Tools.SlugKey(Application.companyName),Tools.SlugKey(ProjectName)),
                        string.Format(IdentifierFormat, Tools.SlugKey(Application.companyName),ProductName),
                        "Neither! Cancel this thing!!!"))
                {
                    case 0:
                        PlayerSettings.productName = ProjectName;
                        break;
                    case 1:
                        PlayerSettings.productName = Application.productName;
                        break;
                    case 2:
                        return false;
                }
            }
            PlayerSettings.SetApplicationIdentifier(EditorUserBuildSettings.selectedBuildTargetGroup,Identifier);
            if (KeystoreExists)
            {
                if (EditorUtility.DisplayDialog("Keystore already exists!", "Overwrite?",
                        "YES!", "No..."))
                {
                    File.Delete(KeystorePath);
                }
                else return false;
            }
            var path = Path.GetFullPath(KeystoreDirectory);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var shell = Path.Combine(Application.dataPath,"Castle", "Editor","keycreator.sh");
            if (File.Exists(shell))
            {
                string storePass = Tools.SlugKey(PlayerSettings.companyName) + Tools.RandomString(8);
                string keyPass = Tools.RandomString(8);
                Tools.RunSh(shell, Alias, KeystorePath, storePass, keyPass, PlayerSettings.companyName,Environment.UserName,RegionInfo.CurrentRegion.EnglishName,RegionInfo.CurrentRegion.Name);
                var key = new Key
                {
                    Alias = Alias,
                    StorePass = storePass,
                    KeyPass = keyPass,
                    Company = PlayerSettings.companyName,
                    CD = Environment.UserName,
                    Region = RegionInfo.CurrentRegion.EnglishName + " (" + RegionInfo.CurrentRegion.Name + ")"
                };
                Tools.WriteTextFile(KeyDetails,JsonUtility.ToJson(key));
                return true;
            }

            Debug.LogError("Keycreator not found!");
            return false;
        }
        [Serializable]
        public struct Key
        {
            public string Alias,StorePass,KeyPass,Company,CD,Region;
        }
        public int callbackOrder => -1;
        public void OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.Android) return;
            SetKey();
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.iOS) return;
            var projPath = report.summary.outputPath + "/Unity-iPhone.xcodeproj/project.pbxproj";
            var proj = new PBXProject();
            proj.ReadFromFile(projPath);
            var mainTargetGuid = proj.GetUnityMainTargetGuid();
            var frameworkTargetGuid = proj.GetUnityFrameworkTargetGuid();
            proj.SetBuildProperty(mainTargetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
            proj.SetBuildProperty(frameworkTargetGuid,"ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");
            proj.SetBuildProperty(new[]{mainTargetGuid,frameworkTargetGuid,proj.ProjectGuid()},"ENABLE_BITCODE","NO");
            proj.WriteToFile(projPath);

            var plistPath = report.summary.outputPath + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));
            PlistElementDict rootDict = plist.root;
            if(rootDict.values.ContainsKey("UIApplicationExitsOnSuspend")) rootDict.values.Remove("UIApplicationExitsOnSuspend");
            rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);
            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }
}
