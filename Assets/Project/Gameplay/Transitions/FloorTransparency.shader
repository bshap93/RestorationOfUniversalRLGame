// Optional: Shader-based visibility system
Shader "Transitions/FloorTransparency"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _PlayerPos ("Player Position", Vector) = (0,0,0,0)
        _VisibilityRadius ("Visibility Radius", Float) = 5.0
        _FadeStrength ("Fade Strength", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent" "RenderType"="Transparent"
        }
        LOD 100

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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _PlayerPos;
            float _VisibilityRadius;
            float _FadeStrength;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;

                float dist = distance(_PlayerPos.xz, i.worldPos.xz);
                float fade = saturate(dist / _VisibilityRadius);

                col.a *= lerp(_FadeStrength, 1, fade);

                return col;
            }
            ENDCG
        }
    }
}