using System.Collections.Generic;
using UnityEngine;

namespace Castle.Core.Scene
{
    public abstract class CastleBaseScene : MonoBehaviour
    {
        public string SceneName => gameObject.scene.name;
        [SerializeField]
        public List<ISceneObject> sceneObjects;
    }
}