Shader "PhysicsDebugger/Common"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent"}
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"
            
            sampler2D _CameraDepthTexture;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            float4 _Color;
            float _Extend;
            
            v2f vert (appdata v)
            {
                UNITY_SETUP_INSTANCE_ID(v);

                v2f o;
                v.vertex.xyz += v.normal * _Extend;
                o.vertex = UnityObjectToClipPos(v.vertex);

                UNITY_TRANSFER_INSTANCE_ID(v, o);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            { 
                UNITY_SETUP_INSTANCE_ID(i);
                return _Color;
            }
            ENDCG
        }
    }
}
