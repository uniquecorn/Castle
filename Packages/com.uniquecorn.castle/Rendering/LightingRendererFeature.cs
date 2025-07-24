using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Castle.Rendering
{
    public class LightingRendererFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class LightingSettings
        {
            public Color ambientLighting;
            [Range(0,10)]
            public float bloom;
            [Range(0,1)]
            public float additiveBloom;
            public LayerMask layerMask;
        }
        public LightingSettings settings = new LightingSettings();
        public Shader multiplyShader,blurShader;
        private Material multiplyMaterial,blurMaterial;
        private LightingRendererPass lightingRendererPass;
        public override void Create()
        {
            blurShader = Shader.Find("Castle/BlurBlit");
            multiplyShader = Shader.Find("Castle/LightingBlit");
            if (multiplyShader == null || blurShader == null) return;
            multiplyMaterial = new Material(multiplyShader);
            blurMaterial = new Material(blurShader);
            lightingRendererPass = new LightingRendererPass(multiplyMaterial,blurMaterial,settings)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingTransparents
            };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType == CameraType.Preview
                || UniversalRenderer.IsOffscreenDepthTexture(ref renderingData.cameraData))
                return;
            if(renderingData.cameraData.cameraType == CameraType.SceneView && !renderingData.cameraData.postProcessEnabled)return;
            renderer.EnqueuePass(lightingRendererPass);
        }
    }
}