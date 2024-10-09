Shader "Unlit/ConveyorBelt"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TinlingAndOffset ("Tiling And Offset", Vector) = (0,0,0,0)
        _uvSpeed("_uvSpeed", range(0,10)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"

            #pragma vertex Vertex
            #pragma fragment fragment
            
            TEXTURE2D(_MainTex); 
            SAMPLER(sampler_MainTex);
            float4 _TinlingAndOffset;
            float _uvSpeed;

            struct Attributes
            {
                float4 positionOS: POSITION;
                float2 uv: TEXCOORD0;
            };

            struct Varings
            {
                float4 positionCS: SV_POSITION;
                float2 uv: TEXCOORD0;
            };

            Varings Vertex(Attributes v)
            {
                Varings o;
                o.positionCS = TransformObjectToHClip(v.positionOS);
                v.uv.y += _Time.y * _uvSpeed;
                o.uv = v.uv * _TinlingAndOffset.xy;
                return o;
            }

            half4 fragment(Varings i): SV_TARGET
            {
                float2 uv = i.uv;
                float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                return half4(color.xyz, 1);
            }

            ENDHLSL
        }
    }
}
