using System.IO;
using System.Threading.Tasks;
using Castle.Core.TimeTools;
using Castle.Tools;
using Sirenix.Serialization;
using UnityEngine;
#if ODIN_INSPECTOR
#endif

namespace Castle.Core.Save
{
    public static class CastleSave
    {
        private const string TempSaveName = "temp.csave";
        private const string SaveName = "save.csave";
        private const string BackupSaveName = "save.csave.bak";
        public static readonly string TempSavePath = Path.Join(Application.persistentDataPath, TempSaveName);
        public static readonly string BackupSavePath = Path.Join(Application.persistentDataPath, BackupSaveName);
        public static readonly string SavePath = Path.Join(Application.persistentDataPath, SaveName);
        private enum SaveState
        {
            Idle,
            Delay,
            Saving,
            Force
        }
        public static bool SavingInProgress => SaveTask is {IsCompleted: false};
        private static SaveState saveState;
        private const int SaveDelay = 3;
        private static int SaveOffset;
        private static Task<bool> SaveTask;
        [System.Serializable]
        public abstract class Save<T> : Save where T : Save<T>, new()
        {
            static T save;
            public static T SaveInstance
            {
                get => save;
                set => save = value;
            }
            public static T New()
            {
                SaveInstance = new T();
                SaveInstance.InitializeNewSave();
                return SaveInstance;
            }
        }
        [System.Serializable]
        public abstract class Save
        {
            public int cloudSaveID;
            public bool cloudDisabled;
            public bool cloudSavedBefore;
            public double lastCloudSaveOA;
            public double firstSaved;
            public double lastSaved;
            public float musicVolume;
            public float sfxVolume;
            public int lastSavedVersion;
            public long lastCloudSavedProgress;
            public virtual long Progress => 1;
            public virtual int PlayTime
            {
                get
                {
                    var s = LastSaved.Subtract(FirstSaved).Milliseconds;
                    return s <= 0 ? 1000 : s;
                }
            }

            public virtual bool DisableCloudSave => false;
            public Save()
            {
                cloudSaveID = System.Guid.NewGuid().GetHashCode();
                cloudDisabled = false;
                musicVolume = sfxVolume = 1;
                FirstSaved = CastleTime.Now;
            }

            public virtual void InitializeNewSave() { }
            public System.DateTime FirstSaved
            {
                get => System.DateTime.FromOADate(firstSaved);
                set => firstSaved = value.ToOADate();
            }
            public System.DateTime LastCloudSave
            {
                get => System.DateTime.FromOADate(lastCloudSaveOA);
                set => lastCloudSaveOA = value.ToOADate();
            }
            public System.DateTime LastSaved
            {
                get => System.DateTime.FromOADate(lastSaved);
                set => lastSaved = value.ToOADate();
            }
            public virtual void PreSaveActions() => lastSavedVersion = CastleTools.VersionNum;
            public virtual void LoadActions()
            {
                if (cloudSaveID == 0)
                {
                    cloudSaveID = System.Guid.NewGuid().GetHashCode();
                }
                UpgradeSave(lastSavedVersion,CastleTools.VersionNum);
            }
            public virtual void UpgradeSave(int oldVersion, int newVersion) => lastSavedVersion = newVersion;
        }
        public static string SaveRawJSON<T>() where T : Save<T>, new() => JsonUtility.ToJson(Save<T>.SaveInstance);
        public static T LoadRawJSON<T>(string json) where T : Save<T>, new() => JsonUtility.FromJson<T>(json);
        public static bool SaveExists
        {
#if ODIN_INSPECTOR
            get => File.Exists(SavePath);
#else
        get => PlayerPrefs.HasKey("save");
#endif
        }
#if ODIN_INSPECTOR
        public static bool BackupSaveExists => File.Exists(BackupSavePath);
        public static byte[] SaveRawBytes<T>() where T : Save<T>, new() => SerializationUtility.SerializeValue(Save<T>.SaveInstance, DataFormat.Binary);
        public static T RawBytesToSave<T>(byte[] bytes) where T : Save<T>, new() => SerializationUtility.DeserializeValue<T>(bytes, DataFormat.Binary);
        public static T RawStreamToSave<T>(Stream stream) where T : Save<T>, new() => SerializationUtility.DeserializeValue<T>(stream, DataFormat.Binary);
#endif
        private static async Task<bool> SaveGameTask<T>() where T : Save<T>, new()
        {
            saveState = saveState != SaveState.Force ? SaveState.Delay : saveState;
            while (SaveOffset > 0 && saveState != SaveState.Force)
            {
                Debug.Log(SaveOffset);
                SaveOffset--;
                await Task.Yield();
            }
            saveState = SaveState.Saving;
            var result =  await SaveGameAsync<T>();
            saveState = SaveState.Idle;
            return result;
        }
        public static async Task<bool> SaveGameAsync<T>() where T : Save<T>, new()
        {
            Save<T>.SaveInstance.PreSaveActions();
#if ODIN_INSPECTOR
            var hasExistingSave = SaveExists;
            var path = hasExistingSave ? TempSavePath : SavePath;
            var bytes = SaveRawBytes<T>();
            await using var sourceStream =
                new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Delete | FileShare.ReadWrite, 4096, true);
            await sourceStream.WriteAsync(bytes, 0, bytes.Length);
            await sourceStream.FlushAsync();
            if (hasExistingSave)
            {
                try
                {
                    File.Replace(TempSavePath,SavePath,BackupSavePath);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e);
                    return false;
                }
            }
#else
        PlayerPrefs.SetString("save",SaveRawJSON<T>());
        await Task.Yield();
#endif
            return true;
        }

        public static async Task<bool> LoadGameAsync<T>(bool loadBackup = false) where T : Save<T>, new()
        {
#if ODIN_INSPECTOR
            await using var sourceStream =
                new FileStream(loadBackup? BackupSavePath : SavePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite, 4096, true);
            try
            {
                Save<T>.SaveInstance = RawStreamToSave<T>(sourceStream);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
#else
        Save<T>.SaveInstance = LoadRawJSON<T>(PlayerPrefs.GetString("save"));
        await Task.Yield();
#endif
            if (Save<T>.SaveInstance == null) return false;
            Save<T>.SaveInstance.LoadActions();
            return true;

        }
        public static async Task<bool> LoadGame<T>() where T : Save<T>, new ()
        {
            if (SaveExists)
            {
                var result = await LoadGameAsync<T>();
#if ODIN_INSPECTOR
                if (!result && BackupSaveExists)
                {
                    return await LoadGameAsync<T>(true);
                }
#endif
                return result;
            }
#if ODIN_INSPECTOR
            if (BackupSaveExists)
            {
                return await LoadGameAsync<T>(true);
            }
#endif
            return false;
        }
        public static async Task<bool> SaveGame<T>(bool immediate = false) where T : Save<T>, new()
        {
            if (saveState == SaveState.Saving)
            {
                await SaveTask;
            }
            if (immediate)
            {
                saveState = SaveState.Force;
                SaveOffset = 0;
            }
            else if(saveState == SaveState.Force)
            {
                return await SaveTask;
            }
            else
            {
                SaveOffset = SaveDelay;
            }
            if (SavingInProgress)
            {
                return await SaveTask;
            }
            SaveTask?.Dispose();
            SaveTask = SaveGameTask<T>();
            SaveTask.Start();
            //Cloud Save HERE
            return await SaveTask;
        }
    }
}
