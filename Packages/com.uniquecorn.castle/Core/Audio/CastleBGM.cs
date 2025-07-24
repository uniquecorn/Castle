using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Castle.Core.Audio
{
    [Serializable]
    public class CastleBGM
    {
        [ValueDropdown("GetClips")]
        public AudioClip clip;
        public bool introLoop;
        [ShowIf("introLoop")]
        public int introSamplePoint;
#if UNITY_EDITOR
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