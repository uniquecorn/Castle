using Sirenix.OdinInspector;
using UnityEngine;

namespace Castle.Rendering.Lights
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer)),ExecuteInEditMode]
    public abstract class BaseLight : MonoBehaviour
    {
        [SerializeField,HideInInspector]
        protected Material lightMaterial;
        [OnValueChanged("UpdateMesh")]
        public Color color;
        [SerializeField,HideInInspector]
        protected Mesh mesh;
        [Autohook]
        public MeshFilter meshFilter;
        [Autohook]
        public MeshRenderer meshRenderer;
        protected Vector3[] vertices;
        protected int[] triangles;
        protected Color[] colors;
        protected Vector2[] uvs;
        [Button]
        public abstract void UpdateMesh();
        // private void OnValidate()
        // {
        //     if (mesh == null) mesh = new Mesh();
        //     UpdateMesh();
        // }
        private void Reset()
        {
            mesh = new Mesh();
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.material = lightMaterial;
            UpdateMesh();
        }
        private void Update()
        {
            if(transform.hasChanged)
            {
                UpdateMesh();
            }
        }
    }
}