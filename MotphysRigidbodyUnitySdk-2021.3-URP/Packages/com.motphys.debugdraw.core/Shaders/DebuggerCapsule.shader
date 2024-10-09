Shader "PhysicsDebugger/DebugCapsule"
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

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            float _Radius;
            float _Height; // includes hemisphere
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;

                // standard mesh: (r = 0.5, h = 2)
                // for any R,H, scale first, we have:
                float scale = max(0, _Radius / 0.5f);
                v.vertex.xyz *= scale;
                // now, bottom y of the hemisphere is R, top y is 2R;
                // whole height is 4R;
                // the rest height is H - 4R;
                float h = max(0, _Height);
                float rest = _Height - (_Radius * 4);
                // move the hemisphere.y with +-rest / 2;
                // of course, if rest height is negative, the maximum move length is r. the capsule degenerate into a ball.
                float offsetY = max(-_Radius, (rest / 2));

                if (v.vertex.y > 0)
                {
                    v.vertex.y += offsetY;
                }
                else
                {
                    v.vertex.y -= offsetY;
                }

                o.vertex = UnityObjectToClipPos(v.vertex);
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
