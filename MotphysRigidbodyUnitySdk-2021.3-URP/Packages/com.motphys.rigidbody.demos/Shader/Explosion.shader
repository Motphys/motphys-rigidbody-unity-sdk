Shader "CustomEffects/Explosion"
{
    Properties
    {
        _distortedInt("_distortedInt", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        
        Pass
        {
                ZTest Always
                ZWrite Off

                HLSLPROGRAM

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"

                #pragma vertex Vertex
                #pragma fragment fragment

                Texture2D _distortedTex;
                SamplerState sampler_distortedTex;

                TEXTURE2D(_CameraOpaqueTexture); 
                SAMPLER(sampler_CameraOpaqueTexture);

                half _distortedInt;

                struct Attributes
                {
                    float4 positionOS: POSITION;
                    float2 uv: TEXCOORD0;
                };

                struct Varings
                {
                    float4 positionCS: SV_POSITION;
                    float2 uv: TEXCOORD0;
                    float4 positionNDC : TEXCOORD1;
                };

                float hash2to1(float2 p)
                {
                    float3 p3  = frac(float3(p.xyx) * .1031);
                    p3 += dot(p3, p3.yzx + 33.33);
                    return frac((p3.x + p3.y) * p3.z);
                }

                float valueNoise(float2 uv)
                {
                    float2 intPos = floor(uv); 
                    float2 fracPos = frac(uv); 

                    float2 u = fracPos * fracPos * (3.0 - 2.0 * fracPos); 

                    float va = hash2to1( intPos + float2(0.0, 0.0) );  
                    float vb = hash2to1( intPos + float2(1.0, 0.0) );
                    float vc = hash2to1( intPos + float2(0.0, 1.0) );
                    float vd = hash2to1( intPos + float2(1.0, 1.0) );

                    float k0 = va;
                    float k1 = vb - va;
                    float k2 = vc - va;
                    float k4 = va - vb - vc + vd;
                    float value = k0 + k1 * u.x + k2 * u.y + k4 * u.x * u.y;

                    return value;
                }

                Varings Vertex(Attributes v){

                    Varings o;
                    o.positionCS = TransformObjectToHClip(v.positionOS);
                    o.positionNDC = TransformObjectToHClip(v.positionOS);

                    #if UNITY_REVERSED_Z
                    o.positionNDC.y *= -1; 
                    #endif

                    o.uv = v.uv;
                    return o;
                }

                half4 fragment(Varings i): SV_TARGET
                {
                    float2 screenUV = (i.positionNDC.xy / i.positionNDC.w) * 0.5 +0.5;
                    float noise = valueNoise(i.uv * 10 - 25);
                    float2 uv = noise * _distortedInt + screenUV;
                    return SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uv);
                }

                ENDHLSL
        }
    }
}
