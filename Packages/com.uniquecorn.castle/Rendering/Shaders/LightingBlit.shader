Shader "Castle/LightingBlit"
{
    HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        // The Blit.hlsl file provides the vertex shader (Vert),
        // the input structure (Attributes), and the output structure (Varyings)
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
        float _AdditiveStrength;
        float4 _AmbientLight;
    ENDHLSL

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}

        Pass
		{ // 1
			Name "Multiply"
			Cull Off
	        Blend DstColor Zero
	        ZTest LEqual
	        ZWrite Off
	        Fog {Mode Off}

			HLSLPROGRAM

			#pragma vertex Vert
			#pragma fragment frag
			half4 frag(Varyings input) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord);
            	return lerp(_AmbientLight,float4(1,1,1,1),col);
                //return col;
            }
			ENDHLSL
		}
		Pass
		{
			Name "Additive"
			Cull Off
		    Blend One One
		    ZTest LEqual
		    ZWrite Off
		    Fog {Mode Off}

		    HLSLPROGRAM
			#pragma vertex Vert
			half4 frag(Varyings input) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord);
                return col * _AdditiveStrength;
            }
			#pragma fragment frag
			ENDHLSL
		}
    }
}