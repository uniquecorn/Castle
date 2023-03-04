using System;
using System.IO;
using System.Linq;
using UnityEditor.iOS.Xcode;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Castle.Editor
{
    [GlobalConfig("Assets/Castle/")]
    public class SKAdData : GlobalConfig<SKAdData>, IPostprocessBuildWithReport
    {
        private const string SKANItems = "SKAdNetworkItems";
        private const string SKANIdentifier = "SKAdNetworkIdentifier";
        public string[] networks;

        public class SKAdHelper : OdinEditorWindow
        {
            public static readonly string[] Separators = {"<string>", "</string>"};
            private const string SKAN = "skadnetwork";
            [ShowInInspector, MultiLineProperty(10), SuffixLabel("Paste text here...", true)]
            public string Networks
            {
                get => "";
                set => ParseText(value);
            }
            [ShowInInspector] public string[] adNetworks => Instance.networks;
            [MenuItem("Window/SKAd Helper")]
            static void Init() => GetWindow<SKAdHelper>().Show();
            public void ParseText(string text)
            {
                var x = text.Split(Separators, StringSplitOptions.RemoveEmptyEntries).ToList();
                for (var i = x.Count - 1; i >= 0; i--)
                {
                    if (x[i].EndsWith(SKAN)) continue;
                    x.RemoveAt(i);
                }

                Instance.networks = x.ToArray();
                EditorUtility.SetDirty(Instance);
                AssetDatabase.SaveAssetIfDirty(Instance);
            }
            [Button,ShowIf("ExistingBuildExists")]
            void AddToExistingBuild() => Instance.AddSKAN(BuildTool.BuildPath(BuildTarget.iOS)+Application.productName);
            private bool ExistingBuildExists =>
                Directory.Exists(BuildTool.BuildPath(BuildTarget.iOS) + Application.productName);
        }
        public int callbackOrder => 101;
        public void OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.iOS) return;
            if (Instance.networks == null) return;
            AddSKAN(report.summary.outputPath);
        }
        void AddSKAN(string buildPath)
        {
            var plistPath = Path.Combine(buildPath, "Info.plist");
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);
            var rootDict = plist.root;
            PlistElementArray SKAdNetworkItems = null;
            if (rootDict.values.ContainsKey(SKANItems))
            {
                try
                {
                    SKAdNetworkItems = rootDict.values[SKANItems] as PlistElementArray;
                }
                catch (Exception e)
                {
                    Debug.LogWarning(string.Format("Could not obtain SKAdNetworkItems PlistElementArray: {0}",
                        e.Message));
                }
            }
            SKAdNetworkItems ??= rootDict.CreateArray(SKANItems);
            foreach (var network in Instance.networks)
            {
                SKAdNetworkItems.AddDict().SetString(SKANIdentifier, network);
            }

            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }
}