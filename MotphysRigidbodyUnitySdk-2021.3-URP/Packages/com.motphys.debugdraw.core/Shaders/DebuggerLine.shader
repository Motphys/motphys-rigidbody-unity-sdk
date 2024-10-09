Shader "PhysicsDebugger/Line"
{
    Properties
    {

    }
    SubShader
    {
        Pass
        {
            Tags{ "Queue" = "Opaque" "RenderType" = "Opaque" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed3 color : TEXCROOD0;
            };

            struct Line
            {
                float3 begin;
                float3 end;
                float3 color;
            };

            // per instance
            StructuredBuffer<Line> _LineBuffer;

            // vid = {0, 1}
            v2f vert(uint vid : SV_VertexID, uint instancec_id : SV_INSTANCEID)
            {
                v2f o;
                Line m_line = _LineBuffer[instancec_id];
                float3 pt = (vid & 1) * m_line.end + (!(vid & 1)) * m_line.begin;
                float4 worldPos = float4(pt, 1.0);
                o.vertex = mul(UNITY_MATRIX_VP, worldPos);
                o.color = m_line.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return fixed4(i.color, 1.0);
            }

            ENDCG
        }
    }
}
