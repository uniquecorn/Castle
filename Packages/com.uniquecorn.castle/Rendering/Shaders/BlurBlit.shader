Shader "Castle/BlurBlit"
{
    HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

        float _BlurOffset;

		struct BlurVaryings
		{
		    float4 positionCS : SV_POSITION;
		    float2 texcoord   : TEXCOORD0;
        	float4 pixelOffsets  : TEXCOORD1;
		    UNITY_VERTEX_OUTPUT_STEREO
		};
        BlurVaryings VertBlur(Attributes input)
        {
	        BlurVaryings output;
		    UNITY_SETUP_INSTANCE_ID(input);
		    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
			const float2 offset = float2(1.0 + _BlurOffset, 1.0 + _BlurOffset);
        	const float2 halfPixel = _BlitTexture_TexelSize * 0.5 * offset;
            const float2 halfPixelFlipped = float2(halfPixel.x, -halfPixel.y);
			output.pixelOffsets.xy = halfPixel;
        	output.pixelOffsets.zw = halfPixelFlipped;
		    output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
		    output.texcoord   = DYNAMIC_SCALING_APPLY_SCALEBIAS(GetFullScreenTriangleTexCoord(input.vertexID));

		    return output;
        }
        float4 FragDownSample (BlurVaryings input) : SV_Target
        {
            float4 color = SAMPLE_TEXTURE2D(_BlitTexture,sampler_LinearClamp,input.texcoord) * 4;
            color += SAMPLE_TEXTURE2D(_BlitTexture,sampler_LinearClamp,input.texcoord - input.pixelOffsets.xy);
            color += SAMPLE_TEXTURE2D(_BlitTexture,sampler_LinearClamp,input.texcoord + input.pixelOffsets.xy);
            color += SAMPLE_TEXTURE2D(_BlitTexture,sampler_LinearClamp,input.texcoord - input.pixelOffsets.zw);
            color += SAMPLE_TEXTURE2D(_BlitTexture,sampler_LinearClamp,input.texcoord + input.pixelOffsets.zw);
            return color * 0.125;
        }
        float4 FragUpSample(BlurVaryings input) : SV_Target
        {
            float4 color = SAMPLE_TEXTURE2D(_BlitTexture,sampler_LinearClamp,input.texcoord + float2(-input.pixelOffsets.x * 2.0,0.0));
            color += SAMPLE_TEXTURE2D(_BlitTexture,sampler_LinearClamp,input.texcoord - input.pixelOffsets.zw) * 2.0;
            color += SAMPLE_TEXTURE2D(_BlitTexture,sampler_LinearClamp,input.texcoord + float2(0.0, input.pixelOffsets.y * 2.0));
            color += SAMPLE_TEXTURE2D(_BlitTexture,sampler_LinearClamp,input.texcoord + input.pixelOffsets.xy) * 2.0;
            color += SAMPLE_TEXTURE2D(_BlitTexture,sampler_LinearClamp,input.texcoord + float2(input.pixelOffsets.x * 2.0, 0.0));
            color += SAMPLE_TEXTURE2D(_BlitTexture,sampler_LinearClamp,input.texcoord + input.pixelOffsets.zw) * 2.0;
            color += SAMPLE_TEXTURE2D(_BlitTexture,sampler_LinearClamp,input.texcoord + float2(0.0, -input.pixelOffsets.y * 2.0));
            color += SAMPLE_TEXTURE2D(_BlitTexture,sampler_LinearClamp,input.texcoord - input.pixelOffsets.xy) * 2.0;
            return color * 0.0833;
        }

    ENDHLSL

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        Fog {Mode Off}
        Pass
		{ // 1
			Name "DownSample"

			HLSLPROGRAM
			#pragma vertex VertBlur
			#pragma fragment FragDownSample
			ENDHLSL
		}

		Pass
		{ // 2
			Name "UpSample"

			HLSLPROGRAM
			#pragma vertex VertBlur
			#pragma fragment FragUpSample
			ENDHLSL
		}
    }
}