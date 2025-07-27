using System.Collections.Generic;
using System.IO;
using System.Linq;
using Castle.Core.UI;
using Sirenix.Serialization;
using Sirenix.Serialization.Editor;
using Sirenix.Utilities;
using Unity.Android.Types;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Castle.Editor
{
    public class BuildTool : IPostprocessBuildWithReport,IProcessSceneWithReport
    {
        const string VersionFormat = "{0}.{1}.{2}";
        static System.Diagnostics.Stopwatch stopwatch;
        public static string GetBuildPath(BuildTarget target)
        {
            return Path.Combine(Settings.ProjectPath,"Build",target.ToString());
        }
        public int callbackOrder => 0;
        public void OnProcessScene(Scene scene, BuildReport report)
        {
            if(Application.isPlaying) return;
            var rootObjects = scene.GetRootGameObjects();
            foreach (var rootObj in rootObjects)
            {
                if (!rootObj.TryGetComponent<CastlePopupHandler>(out var handler)) continue;
                if (!handler.BuildValidation()) throw new BuildFailedException("Failed build due to validation!");
                break;
            }
        }
        public void OnPostprocessBuild(BuildReport report)
        {
            var buildPath = GetBuildPath(report.summary.platform);
            if (Directory.Exists(AOTGenerationConfig.Instance.AOTFolderPath))
            {
                AssetDatabase.DeleteAsset(AOTGenerationConfig.Instance.AOTFolderPath);
            }
            AssetDatabase.Refresh();
            if (Settings.Instance.deleteBurstDebugInfo)
            {
                var burstDebugInformationDirectoryPath =
                    Path.Combine(buildPath, $"{Application.productName}_BurstDebugInformation_DoNotShip");
                if (Directory.Exists(burstDebugInformationDirectoryPath))
                {
                    Directory.Delete(burstDebugInformationDirectoryPath, true);
                }
            }

            if (Settings.Instance.deleteil2cppDebugInfo)
            {
                var product = Path.GetFileNameWithoutExtension(report.summary.outputPath);
                var il2cppDebugInformationDirectoryPath =
                    Path.Combine(buildPath, $"{product}_BackUpThisFolder_ButDontShipItWithYourGame");
                if (Directory.Exists(il2cppDebugInformationDirectoryPath))
                {
                    Directory.Delete(il2cppDebugInformationDirectoryPath, true);
                }
            }
        }
        private static (int, string) IncrementVersion(int majorIncr, int minorIncr, int buildIncr)
        {
            var originalBundleVersion = PlayerSettings.bundleVersion;
            var originalBundleCode = PlayerSettings.Android.bundleVersionCode;
            var lines = PlayerSettings.bundleVersion.Split('.');
            if (lines.Length < 3)
            {
                PlayerSettings.bundleVersion = "0.0.1";
                return IncrementVersion(majorIncr, minorIncr, buildIncr);
            }
            var (majorVersion, minorVersion, patchVersion) = (int.Parse(lines[0]) + majorIncr,
                int.Parse(lines[1]) + minorIncr, int.Parse(lines[2]) + buildIncr);
            if (majorIncr > 0)
            {
                minorVersion = 0;
                patchVersion = 0;
            }

            if (minorIncr > 0) patchVersion = 0;
            SetBundleCode(majorVersion, minorVersion, patchVersion);
            Debug.Log("Build v" + PlayerSettings.bundleVersion + " (" + PlayerSettings.Android.bundleVersionCode + ")");
            return (originalBundleCode, originalBundleVersion);
        }
        public static void SetBundleCode(int code, string version)
        {
            PlayerSettings.Android.bundleVersionCode = code;
            PlayerSettings.bundleVersion = version;
        }
        public static void SetBundleCode(int majorVersion, int minorVersion, int patchVersion)
        {
            SetBundleCode((majorVersion * 10000) + (minorVersion * 100) + patchVersion,
                string.Format(VersionFormat, majorVersion, minorVersion, patchVersion));
        }
        public static bool BuildGame(BuildTarget target = BuildTarget.NoTarget)
        {
            SaveOpenedScenes();
            if (Settings.Instance.CreateAotForSave)
            {
                if (!ScanAOT(Settings.Instance.SaveTypeForAot))
                {
                    throw new BuildFailedException("AOT generation failed!");
                }
            }
            PlayerSettings.SplashScreen.show = false;
            if (target == BuildTarget.NoTarget) target = EditorUserBuildSettings.activeBuildTarget;
            var targetGroup = target switch
            {
                BuildTarget.Android => BuildTargetGroup.Android,
                BuildTarget.iOS => BuildTargetGroup.iOS,
                _ => BuildTargetGroup.Unknown
            };
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.FromBuildTargetGroup(targetGroup),Settings.Identifier);
            var path = GetBuildPath(target);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            var buildOptions = BuildOptions.ShowBuiltPlayer;
            if (target == BuildTarget.Android)
            {
                KeystoreManager.SetKey();
                UnityEditor.Android.UserBuildSettings.DebugSymbols.level = DebugSymbolLevel.None;
                switch (EditorUtility.DisplayDialogComplex($"Building {(EditorUserBuildSettings.buildAppBundle ? "App Bundle" : "APK")}",
                            $"Building version {PlayerSettings.bundleVersion} as an {(EditorUserBuildSettings.buildAppBundle ? "App Bundle" : "APK")}?" +
                            (EditorUserBuildSettings.development ? "\n\nDEVELOPMENT BUILD" : ""),
                            "OK!", "Wait...",$"Build as {(!EditorUserBuildSettings.buildAppBundle ? "App Bundle" : "APK")} instead!"))
                {
                    case 0:
                        break;
                    case 1:
                        return false;
                    case 2:
                        EditorUserBuildSettings.buildAppBundle = !EditorUserBuildSettings.buildAppBundle;
                        break;
                }
                PlayerSettings.Android.splitApplicationBinary = EditorUserBuildSettings.buildAppBundle;
                var extension = EditorUserBuildSettings.buildAppBundle ? "aab" : "apk";
                path = Path.Combine(path, $"{Application.productName} {PlayerSettings.bundleVersion}({PlayerSettings.Android.bundleVersionCode}).{extension}");
                if (!EditorUserBuildSettings.buildAppBundle)
                {
                    if (File.Exists(path)) buildOptions |= BuildOptions.BuildScriptsOnly;
                    buildOptions |= BuildOptions.AutoRunPlayer;
                    if (EditorUserBuildSettings.development) buildOptions |= BuildOptions.Development;
                    if (EditorUserBuildSettings.allowDebugging) buildOptions |= BuildOptions.AllowDebugging;
                    if (EditorUserBuildSettings.waitForManagedDebugger) buildOptions |= BuildOptions.WaitForPlayerConnection;
                }
            }
            else if (target == BuildTarget.iOS)
            {
                if (!EditorUtility.DisplayDialog("Building XCode Project",
                        $"Building version {PlayerSettings.bundleVersion}",
                        "OK!", "Wait..."))
                {
                    return false;
                }
                Tools.RunSh(Path.Combine(Settings.CastlePath, "Editor", "kill-xcode.sh"));
                Tools.ForceClearDirectory(path);
            }
            var buildPlayerOptions = new BuildPlayerOptions
            {
                target = target,
                targetGroup = targetGroup,
                scenes = GetScenePaths(),
                options = buildOptions,
                locationPathName = path
            };
            if (stopwatch == null)
            {
                stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();
            }
            else
            {
                stopwatch.Restart();
            }

            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            Debug.Log(report.summary.result + ", Time taken:" + stopwatch.Elapsed);
            stopwatch.Stop();
            return report.summary.result == BuildResult.Succeeded;
        }
        private static string[] GetScenePaths()
        {
            var scenes = new List<string>();
            for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
                if (EditorBuildSettings.scenes[i].enabled)
                    scenes.Add(EditorBuildSettings.scenes[i].path);
            return scenes.ToArray();
        }
        public static bool ScanAOT(System.Type typeToScan)
        {
            if (AOTGenerationConfig.Instance.AutomateBeforeBuilds) return false;
            var allTypes = new HashSet<System.Type>();
            Tools.GetAllReferencedTypes(typeToScan, ref allTypes);
            try
            {
                AOTSupportUtilities.GenerateDLL(AOTGenerationConfig.Instance.AOTFolderPath,
                    "Sirenix.Serialization.AOTGenerated", allTypes.ToList());
            }
            catch (System.Exception)
            {
                return false;
            }

            return true;
        }

        public static void SaveOpenedScenes()
        {
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (!scene.isDirty) continue;
                if (EditorBuildSettings.scenes.Any(x => x.enabled && x.path == scene.path))
                {
                    EditorSceneManager.SaveScene(scene);
                }
            }
        }
    }
}