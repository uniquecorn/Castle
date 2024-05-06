using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Castle.Editor
{
    public static class KeystoreManager
    {
        [System.Serializable]
        public struct Key
        {
            public string Alias,StorePass,KeyPass,Company,CD,Region;
        }
        static string StorePath => Path.Combine(Settings.ProjectPath, "Store");
        static string KeystorePath => Path.GetFullPath(Path.Combine(Settings.ProjectPath, ShortKeystorePath));
        private static string KeyDetailsPath => Path.Combine(StorePath, "keyDetails.txt");
        static string ShortKeystorePath => Path.Combine("Store", Settings.ProductName + ".keystore");
        static string Alias => Settings.ProductName+"key";
        static KeystoreManager()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                if (File.Exists(KeystorePath))
                {
                    SetKey();
                }
            }
        }
        [MenuItem("Tools/CreateKey")]
        public static bool CreateKeystore()
        {
            if (PlayerSettings.companyName == "DefaultCompany")
            {
                EditorUtility.DisplayDialog("Can't create keystore!", "Company name is not set in PlayerSettings",
                    "Oops...");
                return false;
            }
            var path = Path.GetFullPath(StorePath);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            var shell = Path.Combine(Settings.CastlePath,"keycreator.sh");
            if (File.Exists(shell))
            {
                var key = new Key
                {
                    Alias = Alias,
                    StorePass = PlayerSettings.companyName.Slugged() + Tools.RandomString(8),
                    KeyPass = Tools.RandomString(8),
                    Company = PlayerSettings.companyName,
                    CD = System.Environment.UserName,
                    Region = RegionInfo.CurrentRegion.EnglishName + " (" + RegionInfo.CurrentRegion.Name + ")"
                };
                Tools.WriteTextFile(KeyDetailsPath,JsonUtility.ToJson(key));
                Tools.RunSh(shell, Alias, KeystorePath, key.StorePass, key.KeyPass);
                return true;
            }
            Debug.LogError("Keycreator not found!");
            return false;
        }
        [MenuItem("Tools/SetKey")]
        public static bool SetKey()
        {
            if (!File.Exists(KeystorePath))
            {
                if (!CreateKeystore())
                {
                    return false;
                }
            }
            if (!File.Exists(KeyDetailsPath)) return false;
            PlayerSettings.SetApplicationIdentifier(EditorUserBuildSettings.selectedBuildTargetGroup,Settings.Identifier);
            var details = JsonUtility.FromJson<Key>(Tools.ReadTextFile(KeyDetailsPath));
            PlayerSettings.Android.useCustomKeystore=true;
            PlayerSettings.Android.keystoreName = ShortKeystorePath;
            PlayerSettings.Android.keystorePass = details.StorePass;
            PlayerSettings.Android.keyaliasName = Alias;
            PlayerSettings.Android.keyaliasPass = details.KeyPass;
            return true;
        }
    }
}