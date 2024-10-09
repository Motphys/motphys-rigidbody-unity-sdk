Shader "PhysicsDebugger/Point"
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

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            fixed4 _Color;
            float _PointScale;

            StructuredBuffer<float3> _WorldPos;

            v2f vert(appdata v, uint instanceId : SV_InstanceID)
            {
                v2f o;
                v.vertex *= max(_PointScale, 0.001);
                v.vertex += float4(_WorldPos[instanceId], 0);
                float4 positionOS =  float4(v.vertex.xyz,1.0);
                o.vertex = mul(UNITY_MATRIX_VP, positionOS); 
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _Color;
            }

            ENDCG
        }
    }
}
