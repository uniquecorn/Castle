using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace Castle.Rendering
{
    public class BlurRendererFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class BlurSettings
        {
            [Range(0,10f),HideIf("quality",Quality.None)] public float blurOffset;
            public Quality quality = Quality.Medium;
            public enum Quality
            {
                None,
                Low,
                Medium,
                High
            }
            public bool DoSecondPass => quality >= Quality.Medium;
            public bool DoThirdPass => quality == Quality.High;
        }
        [SerializeField] private BlurSettings settings;
        [SerializeField] private Shader shader;
        private Material material;
        private BlurRenderPass blurRenderPass;
        public override void Create()
        {
            if (shader == null) return;
            material = new Material(shader);
            blurRenderPass = new BlurRenderPass(material, settings);

            blurRenderPass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer,
            ref RenderingData renderingData)
        {
            if (blurRenderPass == null) return;
            if (renderingData.cameraData.cameraType == CameraType.Game)
            {
                renderer.EnqueuePass(blurRenderPass);
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (Application.isPlaying) Destroy(material);
            else DestroyImmediate(material);
        }
    }
}