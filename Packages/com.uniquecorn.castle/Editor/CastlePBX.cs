#if UNITY_IOS || UNITY_STANDALONE_OSX
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
using UnityEngine;

namespace Castle.Editor
{
    public class CastlePBX : PBXProject
    {
        public string buildPath,pbxPath, mainTargetGuid, testTargetGuid, frameworkTargetGuid, projectGuid;
        [System.Flags]
        public enum Target
        {
            None = 0,
            Main = 1 << 0,
            Framework = 1 << 1,
            Test = 1 << 2,
            Project = 1 << 3
        }

        public static bool GetPBX(string buildPath, out CastlePBX pbx)
        {
            var pbxPath = GetPBXProjectPath(buildPath);
            if (File.Exists(pbxPath))
            {
                pbx = new CastlePBX();
                pbx.Initialize(buildPath,pbxPath);
                return true;
            }

            pbx = default;
            return false;
        }

        public void Initialize(string buildPath,string pbxPath)
        {
            this.buildPath = buildPath;
            this.pbxPath = pbxPath;
            ReadFromFile(pbxPath);
            mainTargetGuid = GetUnityMainTargetGuid();
            testTargetGuid = GetUnityMainTestTargetGuid();
            frameworkTargetGuid = GetUnityFrameworkTargetGuid();
            projectGuid = ProjectGuid();
        }
        public void AddCapabilities(string[] capabilities)
        {
            if (!capabilities.IsSafe()) return;
            // foreach (var capability in capabilities)
            // {
            //     if (capability.StartsWith("com.apple."))
            //     {
            //         AddCapability(mainTargetGuid,PBXCapabilityType.StringToPBXCapabilityType(capability));
            //     }
            //     else
            //     {
            //         AddCapability(mainTargetGuid,PBXCapabilityType.StringToPBXCapabilityType("com.apple." + capability));
            //     }
            //     AddCapability(mainTargetGuid,PBXCapabilityType.StringToPBXCapabilityType("com.apple." + capability));
            // }
        }
        public void AddAssociatedDomains(string[] domains)
        {
            if(!domains.IsSafe())return;
            var capManager = new ProjectCapabilityManager(pbxPath, "default.entitlements", "Unity-iPhone");
            capManager.AddAssociatedDomains(domains);
            capManager.WriteToFile();
        }
        public void AddFrameworks(string[] frameworks)
        {
            if (!frameworks.IsSafe()) return;
            foreach (var framework in Settings.Instance.frameworks)
            {
                AddFrameworkToProject(frameworkTargetGuid, framework +".framework", true);
            }
        }
        public void SetBuildProperty(string property, bool value, Target target) => SetBuildProperty(property, value ? "YES" : "NO",target);
        public void SetBuildProperty(string property, Target target) => SetBuildProperty(GetGUIDs(target), property, "YES");
        public void SetBuildProperty(string property, string value, Target target) => SetBuildProperty(GetGUIDs(target), property, value);
        public string[] GetGUIDs(Target target)
        {
            var guids = new List<string>(4);
            if ((target & Target.Main) != 0) guids.Add(mainTargetGuid);
            if ((target & Target.Framework) != 0) guids.Add(frameworkTargetGuid);
            if ((target & Target.Test) != 0) guids.Add(testTargetGuid);
            if ((target & Target.Project) != 0) guids.Add(projectGuid);
            return guids.ToArray();
        }
        public void CopyXcodeAssets()
        {
            var xcodeAssetsDir = Path.Join(Application.dataPath, "Editor", "XCode");
            if (!Directory.Exists(xcodeAssetsDir)) return;
            foreach (var file in Directory.GetFiles(xcodeAssetsDir))
            {
                var fileName = Path.GetFileName(file);
                if(fileName.EndsWith(".meta"))continue;
                File.Copy(file,Path.Join(buildPath,fileName),true);
                AddFileToBuild(mainTargetGuid,AddFile(fileName,fileName));
            }
            var embedFrameworksDir = Path.Combine(xcodeAssetsDir, "EmbeddedFrameworks");
            if (!Directory.Exists(embedFrameworksDir)) return;
            var files = Directory.GetFiles(embedFrameworksDir, "*.zip");
            if (!files.IsSafe()) return;
            var extractionPath = Path.Combine(buildPath, "Frameworks");
            if (!Directory.Exists(extractionPath)) Directory.CreateDirectory(extractionPath);
            var mainLinkPhaseGuid = GetFrameworksBuildPhaseByTarget(mainTargetGuid);
            var unityFrameworkLinkPhaseGuid = GetFrameworksBuildPhaseByTarget(frameworkTargetGuid);
            foreach (var file in Directory.GetFiles(embedFrameworksDir,"*.zip"))
            {
                if(!file.EndsWith("xcframework.zip"))continue;
                var fileName = Path.GetFileName(file);
                Debug.Log(fileName[..^4]);
                ZipFile.ExtractToDirectory(file, extractionPath);
                var frameworkPath = Path.Combine(extractionPath, fileName[..^4]);
                string fileGuid = AddFile(frameworkPath, Path.Combine("Frameworks", fileName[..^4]));
                this.AddFileToEmbedFrameworks(mainTargetGuid, fileGuid);
                AddFileToBuildSection(mainTargetGuid, mainLinkPhaseGuid, fileGuid);
                AddFileToBuildSection(frameworkTargetGuid, unityFrameworkLinkPhaseGuid, fileGuid);
            }
        }

        public void RenameMainTarget(string newName)
        {

        }
        public void Save()
        {
            WriteToFile(pbxPath);
        }
    }
}
#endif