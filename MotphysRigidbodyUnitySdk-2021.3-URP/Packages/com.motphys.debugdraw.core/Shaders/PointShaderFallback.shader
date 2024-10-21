Shader "PointShaderFallback"
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

            half4 _Color;

            Varings vert(Attributes input) {
                Varings o;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input,o);

                o.positionCS = UnityObjectToClipPos(input.positionOS); 
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
