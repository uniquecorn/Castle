using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace Castle.Rendering
{
    public class BlurRenderPass : ScriptableRenderPass
    {
        private BlurRendererFeature.BlurSettings defaultSettings;
        private Material material;
        private TextureDesc halfTextureDescriptor;
        private TextureDesc quarterTextureDescriptor;
        private TextureDesc eighthTextureDescriptor;
        private static readonly int offsetBlurId = Shader.PropertyToID("_BlurOffset");
        public BlurRenderPass(Material material, BlurRendererFeature.BlurSettings defaultSettings)
        {
            this.material = material;
            this.defaultSettings = defaultSettings;
        }
        public override void RecordRenderGraph(RenderGraph renderGraph,
            ContextContainer frameData)
        {
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            if (resourceData.isActiveTargetBackBuffer) return;
            UpdateBlurSettings();
            if(defaultSettings.quality == BlurRendererFeature.BlurSettings.Quality.None || defaultSettings.blurOffset < 0.001f) return;
            TextureHandle srcCamColor = resourceData.activeColorTexture;
            if (!srcCamColor.IsValid()) return;

            halfTextureDescriptor = srcCamColor.GetDescriptor(renderGraph);
            halfTextureDescriptor.depthBufferBits = 0;
            halfTextureDescriptor.width /= 2;
            halfTextureDescriptor.height /= 2;
            var halfBuffer = renderGraph.CreateTexture(halfTextureDescriptor);
            if (!halfBuffer.IsValid()) return;
            var quarterBuffer = halfBuffer;
            var eighthBuffer = quarterBuffer;

            // The AddBlitPass method adds a vertical blur render graph pass that blits from the source texture (camera color in this case) to the destination texture using the first shader pass (the shader pass is defined in the last parameter).
            RenderGraphUtils.BlitMaterialParameters downSample0 = new(srcCamColor, halfBuffer, material, 0);
            renderGraph.AddBlitPass(downSample0,"down1");
            if (defaultSettings.DoSecondPass)
            {
                quarterTextureDescriptor = halfTextureDescriptor;
                quarterTextureDescriptor.width /= 2;
                quarterTextureDescriptor.height /= 2;
                quarterBuffer = renderGraph.CreateTexture(quarterTextureDescriptor);
                if(!quarterBuffer.IsValid()) return;
                if (defaultSettings.DoThirdPass)
                {
                    eighthTextureDescriptor = quarterTextureDescriptor;
                    eighthTextureDescriptor.width /= 2;
                    eighthTextureDescriptor.height /= 2;
                    eighthBuffer = renderGraph.CreateTexture(eighthTextureDescriptor);
                    if(!eighthBuffer.IsValid()) return;
                }
                RenderGraphUtils.BlitMaterialParameters downSample1 = new(halfBuffer, quarterBuffer, material, 0);
                renderGraph.AddBlitPass(downSample1, "down2");
                if (defaultSettings.DoThirdPass)
                {
                    RenderGraphUtils.BlitMaterialParameters downSample2 = new(quarterBuffer, eighthBuffer, material, 0);
                    renderGraph.AddBlitPass(downSample2, "down3");
                    RenderGraphUtils.BlitMaterialParameters upSample0 = new(eighthBuffer, quarterBuffer, material, 1);
                    renderGraph.AddBlitPass(upSample0,"up1");
                }
                RenderGraphUtils.BlitMaterialParameters upSample1 = new(quarterBuffer, halfBuffer, material, 1);
                renderGraph.AddBlitPass(upSample1, "up2");
            }
            RenderGraphUtils.BlitMaterialParameters upSample2 = new(halfBuffer, srcCamColor, material, 1);
            renderGraph.AddBlitPass(upSample2, "up3");
        }
        private void UpdateBlurSettings()
        {
            if (material == null) return;
            material.SetFloat(offsetBlurId, defaultSettings.blurOffset);
        }
    }
}