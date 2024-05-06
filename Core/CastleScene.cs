using System.Collections.Generic;
using UnityEngine;

namespace Castle.Core
{
    public abstract class CastleBaseScene : MonoBehaviour
    {
        public string SceneName => gameObject.scene.name;
        [SerializeField]
        public List<ISceneObject> sceneObjects;
    }
    public abstract class CastleScene : CastleBaseScene
    {
        public static CastleScene currentScene;
        public static List<CastleScene> loadedScenes;

        public virtual void SetCurrentScene()
        {
            currentScene = this;
            loadedScenes ??= new List<CastleScene>(UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings);
            var sceneAlreadyLoaded = false;
            for (var i = 0; i < loadedScenes.Count; i++)
            {
                if (loadedScenes[i] != this) continue;
                loadedScenes.Move(i, loadedScenes.Count - 1);
                sceneAlreadyLoaded = true;
                break;
            }
            if (!sceneAlreadyLoaded) loadedScenes.Add(this);
        }
    }
    public abstract class CastleSubscene : CastleBaseScene
    {
        
    }
}
