#ifndef CastleLighting_HLSL
#define CastleLighting_HLSL
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"

TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);
CBUFFER_START(UnityPerMaterial)
    float4 _Color;
CBUFFER_END

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
    float4   color       : COLOR;
    float2  uv          : TEXCOORD0;
    UNITY_VERTEX_OUTPUT_STEREO
};

struct StrippedAttributes
{
    float3 positionOS   : POSITION;
    float2 uv           : TEXCOORD0;
    UNITY_SKINNED_VERTEX_INPUTS
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct StrippedVaryings
{
    float4  positionCS      : SV_POSITION;
    float2  uv              : TEXCOORD0;
    UNITY_VERTEX_OUTPUT_STEREO
};
StrippedVaryings OccluderVertex(StrippedAttributes attributes)
{
    StrippedVaryings o = (StrippedVaryings)0;
    UNITY_SETUP_INSTANCE_ID(attributes);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    UNITY_SKINNED_VERTEX_COMPUTE(attributes);

    SetUpSpriteInstanceProperties();
    attributes.positionOS = UnityFlipSprite(attributes.positionOS, unity_SpriteProps.xy);
    o.positionCS = TransformObjectToHClip(attributes.positionOS);
    o.uv = attributes.uv;
    return o;
}
float4 OccluderFragment(StrippedVaryings i) : SV_Target
{
    return float4(0,0,0,SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv).a);
}
Varyings UnlitVertex(Attributes v)
{
    Varyings o = (Varyings)0;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    UNITY_SKINNED_VERTEX_COMPUTE(attributes);

    o.positionCS = TransformObjectToHClip(v.positionOS);
    o.uv = v.uv;
    o.color = v.color * _Color;
    return o;
}
float4 RadialFragment(Varyings i) : SV_Target
{
    //return half4(1,1,1,1);
    //return i.color;
    float t = length(i.uv - float2(0.5, 0.5)); // 1.141... = sqrt(2)
    float4 col = i.color;
    col.a = clamp((1-t) * i.color.a,0,1);
    //col.a = saturate(clamp(lerp(i.color.a,0,t/0.5),0,i.color.a));
    return col;
}
#endif