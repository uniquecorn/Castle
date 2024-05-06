using System;
using System.IO;
using UnityEditor;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Castle.Core.Audio
{
    [Serializable]
    public class CastleBGM
    {
#if ODIN_INSPECTOR
        [ValueDropdown("GetClips")]
#endif
        public AudioClip clip;
        public bool introLoop;
#if ODIN_INSPECTOR
        [ShowIf("introLoop")]
#endif
        public int introSamplePoint;
#if ODIN_INSPECTOR && UNITY_EDITOR
        public ValueDropdownList<AudioClip> GetClips()
        {
            var dropdown = new ValueDropdownList<AudioClip>();
            var dir = new DirectoryInfo(Application.dataPath + "/Audio/BGM");
            if (!dir.Exists) return dropdown;
            var files = dir.GetFiles("*.wav");
            foreach (var f in files)
            {
                var c = f.NameNoExtension();
                dropdown.Add(c,AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/BGM/"+f.Name));
            }
            return dropdown;
        }
        #endif
    }
}
