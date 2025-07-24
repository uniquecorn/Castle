Shader "Castle/Sprite-Lit-Fallback"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}

        // Legacy properties. They're here so that materials using this shader can gracefully fallback to the legacy sprite shader.
        [HideInInspector] _Color ("Tint", Color) = (1,1,1,1)
        [HideInInspector] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _AlphaTex ("External Alpha", 2D) = "white" {}
        [HideInInspector] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Name "LightOcclusion"
            Tags { "LightMode" = "CastleLighting" "Queue"="Transparent" "RenderType"="Transparent" }
            HLSLPROGRAM
            #include "CastleLighting.hlsl"

            #pragma vertex OccluderVertex
            #pragma fragment OccluderFragment

            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ SKINNED_SPRITE

            ENDHLSL
        }
    }
}