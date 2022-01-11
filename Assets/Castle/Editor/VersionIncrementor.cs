using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Castle.Tools;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class VersionIncrementor : IPreprocessBuildWithReport
{
    private static string ProjectName => Application.dataPath.Split('/')[^2];
    private static string ProductName => CastleTools.SlugKey(Application.productName.Split(' ')[^1]);
    private static string KeystoreFileName => ProductName + ".keystore";
    private static string ShortKeystorePath => Path.Combine("Store",KeystoreFileName);
    private static string Alias => ProductName+"key";
    private static string KeystoreDirectory => Path.Combine(Application.dataPath.Replace("Assets", ""), "Store");
    private static string KeyDetails => Path.Combine(KeystoreDirectory, "keyDetails.txt");
    private static string KeystorePath => Path.GetFullPath(Path.Combine(KeystoreDirectory, KeystoreFileName));
    private static bool KeystoreExists => File.Exists(KeystorePath);
    private const string IdentifierFormat = "com.{0}.{1}";
    private static string Identifier => string.Format(IdentifierFormat, CastleTools.SlugKey(PlayerSettings.companyName), ProductName);
    private static string BuildPath(BuildTarget target) => Path.Combine(Application.dataPath.Replace("Assets", ""), "Build" + target);

    private static (int,string) IncrementVersion(int majorIncr, int minorIncr, int buildIncr)
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

    public int callbackOrder => -1;
    public void OnPreprocessBuild(BuildReport report)
    {
        if (!KeystoreExists)
        {
            throw new BuildFailedException("No keystore!");
        }
        SetKey();
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

    public static bool BuildGame(BuildTarget target = BuildTarget.NoTarget)
    {
        if (target == BuildTarget.NoTarget)
        {
            target = EditorUserBuildSettings.activeBuildTarget;
        }

        var buildPath = BuildPath(target);
        if (!Directory.Exists(buildPath))
        {
            Directory.CreateDirectory(buildPath);
        }

        buildPath += Application.productName;
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = EditorBuildSettings.scenes.Where(x => x.enabled).Select(x => x.path).ToArray();
        if (buildPlayerOptions.target == BuildTarget.Android)
        {
            string extension = "apk";
            if (!EditorUserBuildSettings.buildAppBundle)
            {
                var dialog =
                    EditorUtility.DisplayDialogComplex("Build Regular APK",
                        "Building version " + PlayerSettings.bundleVersion + " as an APK?", "OK!",
                        "Build as AAB instead!", "Wait...");
                switch(dialog)
                {
                    case 0:
                        break;
                    case 1:
                        EditorUserBuildSettings.buildAppBundle = true;
                        break;
                    case 2:
                        return false;
                }
            }
            else
            {
                if (!EditorUtility.DisplayDialog("Building App Bundle",
                        "Building version " + PlayerSettings.bundleVersion + " as an AAB?", "OK!","Wait..."))
                {
                    return false;
                }
            }
            if (EditorUserBuildSettings.buildAppBundle)
            {
                buildPlayerOptions.targetGroup = BuildTargetGroup.Android;
                extension = "aab";
            }
            else
            {
                buildPlayerOptions.options = BuildOptions.ShowBuiltPlayer;
            }
            buildPath += PlayerSettings.bundleVersion + "(" + PlayerSettings.Android.bundleVersionCode + ")."+extension;
        }
        else if (buildPlayerOptions.target == BuildTarget.iOS)
        {
            buildPlayerOptions.targetGroup = BuildTargetGroup.iOS;
            buildPlayerOptions.options = BuildOptions.ShowBuiltPlayer;
        }
        buildPlayerOptions.locationPathName = buildPath;
        if (buildPlayerOptions.target == BuildTarget.Android && EditorUserBuildSettings.buildAppBundle)
        {

        }
        else
        {
            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            Debug.Log(report.summary.result);
            return report.summary.result == BuildResult.Succeeded;
        }
        return false;
    }
    [MenuItem("Build/Rebuild %b", false, 10000)]
    private static void Rebuild()
    {
        IncrementVersion(0, 0, 0);
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
    [MenuItem("Build/Set Keystore Values")]
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
        var details = JsonUtility.FromJson<Key>(CastleTools.ReadTextFile(KeyDetails));
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
        if (Application.productName != ProjectName)
        {
            switch (EditorUtility.DisplayDialogComplex("Which product name do you want to use?",
                    "It can either be " + ProjectName + " or " + Application.productName + ".",
                    string.Format(IdentifierFormat, CastleTools.SlugKey(Application.companyName),CastleTools.SlugKey(ProjectName.Split(' ')[^1])),
                    string.Format(IdentifierFormat, CastleTools.SlugKey(Application.companyName),ProductName),
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
            PlayerSettings.SetApplicationIdentifier(EditorUserBuildSettings.selectedBuildTargetGroup,Identifier);
        }
        if (KeystoreExists)
        {
            if (EditorUtility.DisplayDialog("Keystore already exists!", "Overwrite?",
                    "YES!", "No..."))
            {
                File.Delete(KeystorePath);
            }
            else
            {
                return false;
            }
        }
        var path = Path.GetFullPath(KeystoreDirectory);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        var shell = Path.Combine(Application.dataPath,"Castle", "Editor","keycreator.sh");
        if (File.Exists(shell))
        {
            
            string storePass = CastleTools.SlugKey(PlayerSettings.companyName) + CastleTools.RandomString(8);
            string keyPass = CastleTools.RandomString(8);
            CastleTools.RunSh(shell, Alias, KeystorePath, storePass, keyPass, PlayerSettings.companyName,Environment.UserName,RegionInfo.CurrentRegion.EnglishName,RegionInfo.CurrentRegion.Name);
            var key = new Key
            {
                Alias = Alias,
                StorePass = storePass,
                KeyPass = keyPass,
                Company = PlayerSettings.companyName,
                CD = Environment.UserName,
                Region = RegionInfo.CurrentRegion.EnglishName + " (" + RegionInfo.CurrentRegion.Name + ")"
            };
            CastleTools.WriteTextFile(KeyDetails,JsonUtility.ToJson(key));
            return true;
        }
        else
        {
            Debug.LogError("Keycreator not found!");
            return false;
        }
    }
    [Serializable]
    public struct Key
    {
        public string Alias,StorePass,KeyPass,Company,CD,Region;
    }
}
