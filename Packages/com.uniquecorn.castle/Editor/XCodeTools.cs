#if UNITY_IOS || UNITY_STANDALONE_OSX
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;

namespace Castle.Editor
{
    public class XCodeTools : IPostprocessBuildWithReport
    {
        public const string EmbedSwiftStdLib = "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES";
        public const string EnableBitcode = "ENABLE_BITCODE";
        public static bool SetupPBXProject(string buildPath)
        {
            if(!CastlePBX.GetPBX(buildPath,out var pbx)) return false;

            pbx.SetBuildProperty(EnableBitcode,false,CastlePBX.Target.Main | CastlePBX.Target.Framework | CastlePBX.Target.Project);
            pbx.SetBuildProperty(EmbedSwiftStdLib,Settings.Instance.EmbedSwiftStandardLibraries.HasFlag(CastlePBX.Target.Main),CastlePBX.Target.Main);
            pbx.SetBuildProperty(EmbedSwiftStdLib,Settings.Instance.EmbedSwiftStandardLibraries.HasFlag(CastlePBX.Target.Framework),CastlePBX.Target.Framework);

            pbx.AddAssociatedDomains(Settings.Instance.associatedDomains);
            pbx.AddFrameworks(Settings.Instance.frameworks);
            pbx.CopyXcodeAssets();

            pbx.Save();

            SetupPlist(Path.Join(buildPath,"Info.plist"));

            return true;
        }
        public static void SetupPlist(string plistPath)
        {
            var plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));
            var rootDict = plist.root;
            if (rootDict.values.ContainsKey("UIApplicationExitsOnSuspend"))
                rootDict.values.Remove("UIApplicationExitsOnSuspend");
            rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);
            rootDict.SetBoolean("GADIsAdManagerApp", true);
            File.WriteAllText(plistPath, plist.WriteToString());
        }
        public int callbackOrder => 1000;
        public void OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.platform == BuildTarget.iOS)
            {
                SetupPBXProject(report.summary.outputPath);
                if (Settings.Instance.autoOpenXcode)
                {
                    Tools.RunSh(Path.Combine(Settings.CastlePath, "Editor", "xcode.sh"), report.summary.outputPath);
                }
            }
        }
    }
}
#endif