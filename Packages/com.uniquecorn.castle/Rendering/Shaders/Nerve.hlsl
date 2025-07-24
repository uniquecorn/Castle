#ifndef NERVE_HLSL
#define NERVE_HLSL
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);
half4 _MainTex_ST;
float4 _Color;
half4 _RendererColor;

struct Attributes
{
    float3 positionOS   : POSITION;
    float4 color        : COLOR;
    float2 uv           : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float4  positionCS  : SV_POSITION;
    half4   color       : COLOR;
    float2  uv          : TEXCOORD0;
    UNITY_VERTEX_OUTPUT_STEREO
};

Varyings BlackVertex(Attributes v)
{
    Varyings o = (Varyings)0;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

    o.positionCS = TransformObjectToHClip(v.positionOS);
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    o.color = float4(0,0,0,v.color.a) * _Color * _RendererColor;
    return o;
}

Varyings DimVertex(Attributes v)
{
    Varyings o = (Varyings)0;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

    o.positionCS = TransformObjectToHClip(v.positionOS);
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    o.color = float4(1,1,1,0.1) * v.color * _Color * _RendererColor;
    return o;
}

Varyings UnlitVertex(Attributes v)
{
    Varyings o = (Varyings)0;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

    o.positionCS = TransformObjectToHClip(v.positionOS);
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    o.color = v.color * _Color * _RendererColor;
    return o;
}

half4 UnlitFragment(Varyings i) : SV_Target
{
    return i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
}


#endif