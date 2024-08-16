using System.Collections.Generic;

namespace Castle.Core.Scene
{
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
}