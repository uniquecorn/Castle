using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine.UI;

namespace Castle.Editor
{
    public static class EditorExtensions
    {
        [MenuItem("CONTEXT/LayoutGroup/Set Size", false, 1)]
        public static void SetSize(MenuCommand command)
        {
            if(command.context is LayoutGroup layoutGroup) layoutGroup.SetSize();
        }
        [MenuItem("CONTEXT/RectTransform/Fill Rect", false, 1)]
        public static void FillRect(MenuCommand command)
        {
            if(command.context is RectTransform rectTransform) rectTransform.FillRect();
        }
        [MenuItem("CONTEXT/AnimatorController/Merge Controller")]
        public static void MergeAnimatorController(MenuCommand command)
        {
            if (command.context is not AnimatorController controller) return;
            var path = AssetDatabase.GetAssetPath(controller);
            var states = new Dictionary<AnimatorState, string>();
            foreach (var layer in controller.layers)
            {
                foreach (var state in layer.stateMachine.states)
                {
                    states.Add(state.state, state.state.motion.name);
                }
            }
            var clips = new HashSet<AnimationClip>();
            foreach (var clip in controller.animationClips)
            {
                if (AssetDatabase.GetAssetPath(clip) == path) continue;
                clips.Add(clip);
            }
            foreach (var clip in clips)
            {
                var newClip = Object.Instantiate(clip);
                newClip.name = clip.name;
                AssetDatabase.AddObjectToAsset(newClip, path);
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newClip));
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(clip));
                foreach (var state in states)
                {
                    if (state.Value != newClip.name) continue;
                    state.Key.motion = newClip;
                }
            }
            AssetDatabase.SaveAssets();
        }
    }
}