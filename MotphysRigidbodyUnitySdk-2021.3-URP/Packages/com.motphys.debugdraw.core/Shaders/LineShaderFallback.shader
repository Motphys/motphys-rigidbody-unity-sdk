Shader "LineShaderFallback"
{
    Properties
    {

    }
    SubShader
    {
        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct Attributes {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };


            struct Varings {
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
                UNITY_DEFINE_INSTANCED_PROP(float4, _End)
                UNITY_DEFINE_INSTANCED_PROP(half4, _Color)
            UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)

            #define _End UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _End)
            #define _Color UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _Color)

            Varings vert(Attributes input, uint vertexId : SV_VertexID){
                Varings o;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input,o);

                float3 vertex = lerp(input.positionOS, _End, vertexId % 2);
                o.positionCS = UnityObjectToClipPos(float4(vertex, 1)); 
                return o;
            }

            half4 frag(Varings input) : SV_TARGET{
                UNITY_SETUP_INSTANCE_ID(input);
                return half4(_Color.rgb, 1);
            }

            ENDCG
        }
    }
}
