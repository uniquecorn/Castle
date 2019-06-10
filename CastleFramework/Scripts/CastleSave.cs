using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.NativePlugins;

public static class CastleSave
{
    public static Save save;
    public static bool savingDisabled;
    public enum CloudSaveViability
    {
        VIABLE,
        DISABLED,
        OLDERSAVEEXISTS,
        TOOSOON
    }
    public enum CloudStatus
    {
        NONE,
        PENDING,
        INITIALISED,
        FAILED
    }
    public static CloudStatus cloudStatus;
    public interface ISaveData<T>
    {
        void Init();
    }
    [System.Serializable]
    public abstract class Save : ISaveData<Save>
    {
        public LocalisationHelper.Language language;
        public bool cloudDisabled;
        public float lastCloudSaveOA;
        public float firstSaved;
        public float musicVolume;
        public float sfxVolume;
        public bool notificationsDisabled;
        public virtual void Init()
        {
            cloudDisabled = false;
            FirstSaved = CastleTools.CastleTime;
        }
        public System.DateTime FirstSaved
        {
            get
            {
                return System.DateTime.FromOADate(save.firstSaved);
            }
            set
            {
                save.firstSaved = (float)value.ToOADate();
            }
        }
        public System.DateTime LastCloudSave
        {
            get
            {
                return System.DateTime.FromOADate(lastCloudSaveOA);
            }
            set
            {
                lastCloudSaveOA = (float)value.ToOADate();
            }
        }
    }
    public static void Init()
    {
        Debug.Log("Save system initializing...");
        cloudStatus = CloudStatus.PENDING;
        CloudServices.KeyValueStoreDidInitialiseEvent += Initialised;
        if (Application.platform != RuntimePlatform.IPhonePlayer)
        {
            NPBinding.CloudServices.Initialise();
        }  
        else
        {
            cloudStatus = CloudStatus.INITIALISED;
        }
        NPBinding.CloudServices.Synchronise();
    }
    private static void Initialised(bool _success)
    {
        if (_success)
        {
            cloudStatus = CloudStatus.INITIALISED;
            Debug.Log("Initialized cloud save!");
        }
        else
        {
            cloudStatus = CloudStatus.FAILED;
            Debug.LogError("Cloud save initialization failed!");
        }
    }
    public static CloudSaveViability IsCloudSaveViable()
    {
        if (save.cloudDisabled)
        {
            Debug.Log("Cloud save is disabled!");
            return CloudSaveViability.DISABLED;
        }
        if (save.lastCloudSaveOA != 0)
        {
            System.TimeSpan span = CastleTools.CastleTime.Subtract(save.LastCloudSave);
            if (span.TotalHours < 3)
            {
                Debug.Log("Last cloud save was too soon!");
                return CloudSaveViability.TOOSOON;
            }
        }
        if(CloudSaveExists())
        {
            Save tempSave = JsonUtility.FromJson<Save>(NPBinding.CloudServices.GetString("save"));
            if (tempSave.FirstSaved.Subtract(save.FirstSaved).TotalMinutes < 0)
            {
                Debug.LogWarning("Older cloud save exists!");
                return CloudSaveViability.OLDERSAVEEXISTS;
            }
        }
        return CloudSaveViability.VIABLE;
    }
    public static bool IsCloudSaveInitialised()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return true;
        }
        else return NPBinding.CloudServices.IsInitialised();
    }
    public static bool SaveExists()
    {
        bool saveExists = false;
        if (JSONSaveExists())
        {
            if(PlayerPrefs.GetString("save") == "newSave")
            {
                return false;
            }
            saveExists = true;
        }
        if (CloudSaveExists())
        {
            saveExists = true;
        }
        return saveExists;
    }
    public static bool CloudSaveExists()
    {
        if (IsCloudSaveInitialised())
        {
            if (!string.IsNullOrEmpty(NPBinding.CloudServices.GetString("save")))
            {
                if(NPBinding.CloudServices.GetString("save") == "newSave")
                {
                    Debug.LogWarning("Cloud save hard reset!");
                    return false;
                }
                Debug.Log("Cloud save exists!");
                return true;
            }
            else
            {
                Debug.LogWarning("Cloud save doesn't exist!");
                return false;
            }
        }
        else
        {
            Debug.LogWarning("Cloud save not initialized!");
            return false;
        }
    }
    public static bool JSONSaveExists()
    {
        if (PlayerPrefs.HasKey("save"))
        {
            if (PlayerPrefs.GetString("save") == "newSave")
            {
                Debug.LogWarning("JSON save hard reset!");
                return false;
            }
            Debug.Log("JSON save exists!");
            return true;
        }
        return false;
    }
    public static void LoadGame<T>() where T : Save
    {
        if (!LoadFromJSON<T>())
        {
            LoadFromCloud<T>();
        }
        else
        {
            Debug.Log("Loaded from JSON.");
        }
    }
    private static bool LoadFromJSON<T>() where T : Save
    {
        if (JSONSaveExists())
        {
            save = JsonUtility.FromJson<T>(PlayerPrefs.GetString("save"));
            return true;
        }
        return false;
    }
    public static bool LoadFromCloud<T>() where T : Save
    {
        if(CloudSaveExists())
        {
            string cloudSave = NPBinding.CloudServices.GetString("save");
            save = JsonUtility.FromJson<T>(cloudSave);
            return true;
        }
        else
        {
            return false;
        }
    }
    public static void SaveGame<T>() where T : Save
    {
        if(savingDisabled)
        {
            return;
        }
        if(save == null)
        {
            NewSave<T>();
        }
        else
        {
            SaveToJSON<T>();
            CloudSaveViability viable = IsCloudSaveViable();
            if (viable == CloudSaveViability.VIABLE)
            {
                SaveToCloud<T>();
            }
            else if (viable == CloudSaveViability.OLDERSAVEEXISTS)
            {
                //DO SOMETHING
            }
        }
    }
    public static void SaveToJSON<T>() where T : Save
    {
        string jsonSave = JsonUtility.ToJson(save);
        PlayerPrefs.SetString("save", jsonSave);
    }
    public static void SaveToCloud<T>() where T : Save
    {
        string jsonSave = JsonUtility.ToJson(save);
        NPBinding.CloudServices.SetString("save", jsonSave);
    }
    public static void NewSave<T>() where T : Save
    {
        Debug.Log("Creating new save...");
        T _save = (T)System.Activator.CreateInstance(typeof(T), new object[] { });
        _save.Init();
        save = _save;
        SaveGame<T>();
    }
    public static T GetSave<T>() where T : Save
    {
        return (T)save;
    }
}
