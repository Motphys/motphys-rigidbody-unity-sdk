Shader "PhysicsDebugger/AABB"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            StructuredBuffer<float3> _VertexBuffer;
            fixed4 _Color;

            v2f vert(uint vertex_id : SV_VERTEXID, uint instancec_id : SV_INSTANCEID)
            {
                v2f o;
                float3 realPos = _VertexBuffer[vertex_id];
                o.vertex = UnityObjectToClipPos(realPos);
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
