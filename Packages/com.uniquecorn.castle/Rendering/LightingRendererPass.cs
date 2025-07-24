using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace Castle.Rendering
{
    public class LightingRendererPass : ScriptableRenderPass
    {
        private static readonly int AmbientLight = Shader.PropertyToID("_AmbientLight");
        private static readonly int BlurOffset = Shader.PropertyToID("_BlurOffset");
        private static readonly int AdditiveStrength = Shader.PropertyToID("_AdditiveStrength");
        LightingRendererFeature.LightingSettings lightingSettings;
        ShaderTagId m_ShaderTagId;
        ShaderTagId[] m_ShaderTagIdList;
        RenderStateBlock m_RenderStateBlock;
        FilteringSettings m_FilteringSettings;
        private float bloom;
        private Material multiplyMaterial,blurMaterial;
        private TextureDesc halfTextureDescriptor;
        private TextureDesc quarterTextureDescriptor;
        private TextureDesc eighthTextureDescriptor;
        public LightingRendererPass(Material multiplyMaterial,Material blurMaterial, LightingRendererFeature.LightingSettings lightingSettings)
        {
            this.lightingSettings = lightingSettings;
            this.multiplyMaterial = multiplyMaterial;
            this.blurMaterial = blurMaterial;
            m_ShaderTagId = new ShaderTagId("CastleLighting");
            m_ShaderTagIdList = new[] {m_ShaderTagId};
            m_RenderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
            m_FilteringSettings = new FilteringSettings(RenderQueueRange.all, lightingSettings.layerMask);

            //m_FilteringSettings.layerMask = LayerMask.GetMask("Default");
        }
        private void InitPassData(UniversalCameraData cameraData, ref PassData passData)
        {
            passData.renderPassEvent = renderPassEvent;
            passData.cameraData = cameraData;
        }
        private void InitRendererLists(UniversalRenderingData renderingData, UniversalLightData lightData,
            ref PassData passData, ScriptableRenderContext context, RenderGraph renderGraph, bool useRenderGraph)
        {
            var drawingSettings = RenderingUtils.CreateDrawingSettings(m_ShaderTagId, renderingData,
                passData.cameraData, lightData, SortingCriteria.CommonTransparent);
            var param = new RendererListParams(renderingData.cullResults, drawingSettings, m_FilteringSettings)
            {
                tagName = m_ShaderTagId,
                isPassTagName = true
            };
            passData.rendererListHdl = renderGraph.CreateRendererList(param);
        }
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            multiplyMaterial.SetColor(AmbientLight,lightingSettings.ambientLighting);
            multiplyMaterial.SetFloat(AdditiveStrength,lightingSettings.additiveBloom);
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
            UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
            UniversalLightData lightData = frameData.Get<UniversalLightData>();
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            TextureHandle srcCamColor = resourceData.activeColorTexture;
            if (!srcCamColor.IsValid()) return;
            var baseTextureDescriptor = srcCamColor.GetDescriptor(renderGraph);
            baseTextureDescriptor.depthBufferBits = 0;
            var lightingTextureDescriptor = baseTextureDescriptor;
            lightingTextureDescriptor.clearColor = Color.black;
            lightingTextureDescriptor.clearBuffer = true;
            var lightingBuffer = renderGraph.CreateTexture(lightingTextureDescriptor);
            using (var builder =
                   renderGraph.AddRasterRenderPass<PassData>(passName, out var passData, profilingSampler))
            {
                InitPassData(cameraData, ref passData);
                passData.color = lightingBuffer;
                builder.SetRenderAttachment(lightingBuffer, 0, AccessFlags.ReadWrite);
                InitRendererLists(renderingData, lightData, ref passData, default(ScriptableRenderContext), renderGraph, true);
                builder.AllowPassCulling(false);
                builder.AllowGlobalStateModification(true);
                builder.UseRendererList(passData.rendererListHdl);
                builder.SetRenderFunc((PassData data, RasterGraphContext rgContext) =>
                {
                    rgContext.cmd.DrawRendererList(data.rendererListHdl);
                });
            }
            if(lightingSettings.bloom > 0.001f) BlurPass(renderGraph, lightingBuffer, baseTextureDescriptor);
            var blitParam = new RenderGraphUtils.BlitMaterialParameters(lightingBuffer, srcCamColor, multiplyMaterial, 0);
            renderGraph.AddBlitPass(blitParam);
            if (lightingSettings.additiveBloom > 0.001f)
            {
                blitParam = new RenderGraphUtils.BlitMaterialParameters(lightingBuffer, srcCamColor, multiplyMaterial, 1);
                renderGraph.AddBlitPass(blitParam);
            }
        }

        public void BlurPass(RenderGraph renderGraph, TextureHandle source,TextureDesc baseTextureDescriptor)
        {
            blurMaterial.SetFloat(BlurOffset,lightingSettings.bloom);
            halfTextureDescriptor = baseTextureDescriptor;
            halfTextureDescriptor.width /= 2;
            halfTextureDescriptor.height /= 2;
            var halfBuffer = renderGraph.CreateTexture(halfTextureDescriptor);
            if (!halfBuffer.IsValid()) return;
            quarterTextureDescriptor = halfTextureDescriptor;
            quarterTextureDescriptor.width /= 2;
            quarterTextureDescriptor.height /= 2;
            var quarterBuffer = renderGraph.CreateTexture(quarterTextureDescriptor);
            RenderGraphUtils.BlitMaterialParameters downSample0 = new(source, halfBuffer, blurMaterial, 0);
            renderGraph.AddBlitPass(downSample0,"down1");
            RenderGraphUtils.BlitMaterialParameters downSample1 = new(halfBuffer, quarterBuffer, blurMaterial, 0);
            renderGraph.AddBlitPass(downSample1, "down2");
            RenderGraphUtils.BlitMaterialParameters upSample1 = new(quarterBuffer, halfBuffer, blurMaterial, 1);
            renderGraph.AddBlitPass(upSample1, "up2");
            RenderGraphUtils.BlitMaterialParameters upSample2 = new(halfBuffer, source, blurMaterial, 1);
            renderGraph.AddBlitPass(upSample2, "up3");
        }
        private class PassData
        {
            internal RenderObjects.CustomCameraSettings cameraSettings;
            internal RenderPassEvent renderPassEvent;

            internal TextureHandle color;
            internal RendererListHandle rendererListHdl;

            internal UniversalCameraData cameraData;

            // Required for code sharing purpose between RG and non-RG.
            internal RendererList rendererList;
        }
    }
}