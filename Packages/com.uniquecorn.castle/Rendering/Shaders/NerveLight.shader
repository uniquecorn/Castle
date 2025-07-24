Shader "Nerve/Nerve-Light"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        // Legacy properties. They're here so that materials using this shader can gracefully fallback to the legacy sprite shader.
        [HideInInspector] _Color ("Tint", Color) = (1,1,1,1)
        [HideInInspector] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [HideInInspector] _AlphaTex ("External Alpha", 2D) = "white" {}
        [HideInInspector] _EnableExternalAlpha ("Enable External Alpha", Float) = 0

        [PerRendererData] _ColorMask ("Color Mask", Float) = 15
    }

    HLSLINCLUDE
    #include "Nerve.hlsl"
    ENDHLSL

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        //BlendOp Add
        Blend SrcAlpha One
        ColorMask [_ColorMask]
        Cull Off
        ZWrite Off

        Pass
        {
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #pragma vertex DimVertex
            #pragma fragment UnlitFragment
            ENDHLSL
        }
        Pass
        {
            Tags { "LightMode" = "NervePass" }

            HLSLPROGRAM
            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment
            ENDHLSL
        }
    }



    Fallback "Sprites/Default"
}