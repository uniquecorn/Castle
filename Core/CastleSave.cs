using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Castle.Core
{
    public static class CastleSave
    {
        private const string TempSaveName = "temp.csave";
        private const string SaveName = "save.csave";
        private const string BackupSaveName = "save.csave.bak";
        private const string ColdStorageSaveName = "cold.csave.bak";
        public static readonly string TempSavePath = Path.Join(Application.persistentDataPath, TempSaveName);
        public static readonly string BackupSavePath = Path.Join(Application.persistentDataPath, BackupSaveName);
        public static readonly string SavePath = Path.Join(Application.persistentDataPath, SaveName);
        public static readonly string ColdStorageSavePath = Path.Join(Application.persistentDataPath, ColdStorageSaveName);
        private enum SaveState
        {
            Idle,
            Delay,
            Saving
        }
        public static bool SavingInProgress => saveState != SaveState.Idle;
        private static SaveState saveState;
        [System.Serializable]
        public abstract class Save<T> : Save where T : Save<T>, new()
        {
            static T save;
            public virtual bool SavingDisabled => false;
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
            public double lastCloudSaveOA;
            [SuffixLabel("@FirstSaved.ToString()")]
            public double firstSaved;
            [SuffixLabel("@LastSaved.ToString()")]
            public double lastSaved;
            public float musicVolume;
            public float sfxVolume;
            public int lastSavedVersion;
            public long lastCloudSavedProgress;
            public bool hasLastKnownLegitTime;
            public double lastLegitTimeOA;
            public virtual long Progress => 1;
            public virtual double PlayTime
            {
                get
                {
                    var s = LastSaved.Subtract(FirstSaved).TotalMilliseconds;
                    return s <= 0 ? 1000 : s;
                }
            }

            public virtual bool DisableCloudSave => false;
            public Save()
            {
                cloudSaveID = System.Guid.NewGuid().GetHashCode();
                musicVolume = sfxVolume = 1;
                firstSaved = System.DateTime.UtcNow.ToOADate();
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
            public System.DateTime LastKnownLegitTime
            {
                get => System.DateTime.FromOADate(lastLegitTimeOA);
                set => lastLegitTimeOA = value.ToOADate();
            }
            public virtual void PreSaveActions()
            {
                lastSavedVersion = Tools.VersionNum;
                lastSaved = System.DateTime.UtcNow.ToOADate();
                LastKnownLegitTime = CastleClock.UTC;
            }
            public virtual void LoadActions()
            {
                if (cloudSaveID == 0)
                {
                    cloudSaveID = System.Guid.NewGuid().GetHashCode();
                }
                UpgradeSave(lastSavedVersion,Tools.VersionNum);
            }
            public virtual void UpgradeSave(int oldVersion, int newVersion) => lastSavedVersion = newVersion;
        }
        public static bool SaveExists => File.Exists(SavePath);
        public static bool BackupSaveExists => File.Exists(BackupSavePath);
        public static byte[] SaveRawBytes<T>() where T : Save<T>, new() => SaveRawBytes(Save<T>.SaveInstance);
        public static byte[] SaveRawBytes<T>(T save) where T : Save<T>, new() => SerializationUtility.SerializeValue(save, DataFormat.Binary);
        public static T RawBytesToSave<T>(byte[] bytes) where T : Save<T>, new() => SerializationUtility.DeserializeValue<T>(bytes, DataFormat.Binary);
        public static T RawStreamToSave<T>(Stream stream) where T : Save<T>, new() => SerializationUtility.DeserializeValue<T>(stream, DataFormat.Binary);
        public static async UniTask<T> PathToSave<T>(string path, CancellationToken cts) where T : Save<T>, new()
        {
            await using var sourceStream =
                new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite, 4096, true);
            if (cts.IsCancellationRequested)
            {
                throw new System.OperationCanceledException();
            }
            return RawStreamToSave<T>(sourceStream);
        }
        
        public static async UniTask<bool> SaveGameAsync(byte[] bytes,CancellationToken cts)
        {
            var hasExistingSave = SaveExists;
            var path = hasExistingSave ? TempSavePath : SavePath;
            await using var sourceStream =
                new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Delete | FileShare.ReadWrite, 4096, true);
            if (cts.IsCancellationRequested)
            {
                throw new System.OperationCanceledException();
            }
            await sourceStream.WriteAsync(bytes, 0, bytes.Length,cts);
            await sourceStream.FlushAsync(cts);
            if (hasExistingSave)
            {
                try
                {
                    File.Replace(TempSavePath,SavePath,BackupSavePath);
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                    return false;
                }
            }
            return true;
        }

        public static bool SaveImmediate<T>(T save) where T : Save<T>, new()
        {
            save.PreSaveActions();
            var bytes = SaveRawBytes(save);
            var hasExistingSave = SaveExists;
            var path = hasExistingSave ? TempSavePath : SavePath;
            using var sourceStream =
                new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Delete | FileShare.ReadWrite, 4096);
            sourceStream.Write(bytes,0,bytes.Length);
            sourceStream.Flush();
            if (hasExistingSave)
            {
                try
                {
                    File.Replace(TempSavePath,SavePath,BackupSavePath);
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                    return false;
                }
            }
            return true;
        }
        public static async UniTask<bool> LoadGameAsync<T>(CancellationToken cts,bool loadBackup = false) where T : Save<T>, new()
        {
            await using var sourceStream =
                new FileStream(loadBackup? BackupSavePath : SavePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite, 4096, true);
            if (cts.IsCancellationRequested)
            {
                throw new System.OperationCanceledException();
            }
            try
            {
                Save<T>.SaveInstance = RawStreamToSave<T>(sourceStream);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
            if (Save<T>.SaveInstance == null) return false;
            Save<T>.SaveInstance.LoadActions();
            return true;
        }
        public static T LoadImmediate<T>() where T : Save<T>, new()
        {
            if (!SaveExists)
            {
                return null;
            }
            using var sourceStream =
                new FileStream(SavePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite, 4096);
            return RawStreamToSave<T>(sourceStream);
        }
        public static async UniTask<bool> LoadGame<T>(CancellationToken cts) where T : Save<T>, new ()
        {
            if (SaveExists)
            {
                var result = await LoadGameAsync<T>(cts);
                if (!result && BackupSaveExists)
                {
                    return await LoadGameAsync<T>(cts,true);
                }
                return result;
            }
            if (BackupSaveExists)
            {
                return await LoadGameAsync<T>(cts,true);
            }
            return false;
        }
        public static async UniTaskVoid SaveGame<T>(MonoBehaviour behaviour,CancellationToken cts, bool force = false) where T : Save<T>, new()
        {
            if (!force && Save<T>.SaveInstance.SavingDisabled)
            {
                return;
            }
            switch (saveState)
            {
                case SaveState.Idle:
                    saveState = SaveState.Delay;
                    break;
                case SaveState.Delay:
                case SaveState.Saving:
                    return;
            }

            await UniTask.WaitForEndOfFrame(behaviour, cts);
            Save<T>.SaveInstance.PreSaveActions();
            var bytesToSave = SaveRawBytes<T>();
            saveState = SaveState.Saving;
            var result = await SaveGameAsync(bytesToSave, cts);
            if (!result)
            {
                Debug.LogError("Failed to save!");
            }
            else
            {
                //Debug.Log("Logged in: "+CastleKit.IsAuthenticated);
                //Debug.Log("Saving to cloud: "+CastleKit.SavingToCloud);
                // Debug.Log("Cloud save disabled: "+Save<T>.SaveInstance.DisableCloudSave);
                // var progress = Save<T>.SaveInstance.Progress;
                // Debug.Log("Progress: "+progress + ", Cloud progress: "+Save<T>.SaveInstance.lastCloudSavedProgress);
                // if (CastleKit.IsAuthenticated && !CastleKit.SavingToCloud && !Save<T>.SaveInstance.DisableCloudSave && Save<T>.SaveInstance.Progress > Save<T>.SaveInstance.lastCloudSavedProgress)
                // {
                //     Debug.Log("Saving to cloud...");
                //     var cloudSaveResult = await CastleKit.SaveToCloud<T>(cts);
                //     Debug.Log("Cloud save:"+cloudSaveResult);
                // }
            }
            saveState = SaveState.Idle;
        }
    }
}