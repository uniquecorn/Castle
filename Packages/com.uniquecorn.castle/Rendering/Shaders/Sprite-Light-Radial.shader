Shader "Castle/Sprite-Light-Radial"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        // Legacy properties. They're here so that materials using this shader can gracefully fallback to the legacy sprite shader.
        [HideInInspector] _Color ("Tint", Color) = (1,1,1,1)
        [HideInInspector] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _AlphaTex ("External Alpha", 2D) = "white" {}
        [HideInInspector] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    HLSLINCLUDE
    #include "CastleLighting.hlsl"
    ENDHLSL

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        BlendOp Add
        Blend SrcAlpha One
        Cull Off
        ZWrite Off

        Pass
        {
            Name "LightSource"
            Tags { "LightMode" = "CastleLighting" }

            HLSLPROGRAM
            #pragma vertex UnlitVertex
            #pragma fragment RadialFragment
            ENDHLSL
        }
    }
}