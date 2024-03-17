    Shader "Unlit/ShadowShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ShadowTex("Shadow texture", 2D) = "white" {}
        _ShadowColor("Shadow color", Color) = (0,0,0,0)
        _Angle("Angle", Float) = 0
        _Length("Length", Float) = 0
        _Strength("Strength", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
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
            };

            sampler2D _MainTex;
            sampler2D _ShadowTex;
            float4 _ShadowColor;
            float _Angle;
            float _Length;
            float _Strength;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 originalShadow = tex2D(_ShadowTex, i.uv);
                originalShadow *= originalShadow.a;
                fixed2 offset = (fixed2(sin(_Angle), cos(_Angle))) * _Length;

                fixed2 up = fixed2(0, -offset.x);
                fixed2 down = fixed2(0, offset.x);
                fixed2 right = fixed2(offset.y, 0);
                fixed2 left = fixed2(-offset.y, 0);

                fixed4 triangleTest = tex2D(_ShadowTex, i.uv + up + offset);
                fixed4 triangleTest2 = tex2D(_ShadowTex, i.uv + right);
                triangleTest *= triangleTest.a;
                triangleTest2 *= triangleTest2.a;
                if (triangleTest.a && triangleTest2.a) return fixed4(1, 1, 1, 1);

                fixed4 offsettedShadow = tex2D(_ShadowTex, i.uv + offset);
                offsettedShadow *= offsettedShadow.a;

                fixed4 mapColor = tex2D(_MainTex, i.uv);
                return max(originalShadow, max(offsettedShadow, triangleTest));
            }
            ENDCG
        }
    }
}
