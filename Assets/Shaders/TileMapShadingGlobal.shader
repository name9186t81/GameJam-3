Shader "Unlit/TileMapShadingGlobal"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _MapTexture("Map texture", 2D) = "white" {}
        _ShadowTex("Shadow texture", 2D) = "white" {}
        _ShadowColor("Shadow color", Color) = (0,0,0,0)
        _MapColor("Map color", Color) = (0,0,0,0)
        _Angle("Angle", Float) = 0
        _Length("Length", Float) = 0
        _Affinity("Map affinity strength", Float) = 0
        _Iterations("Iterations", Int) = 0
        _Strength("Strength", Float) = 0
    }
        SubShader
        {
            Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull back
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
                sampler2D _MapTexture;
                sampler2D _ShadowTex;
                float4 _ShadowColor;
                float4 _MapColor;
                float _Angle;
                float _Length;
                float _Affinity;
                float _Strength;
                float4 _MainTex_ST;
                int _Iterations;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed shadow = 0;
                    fixed4 sampled = tex2D(_ShadowTex, i.uv);

                    fixed2 direction = (fixed2(cos(_Angle), sin(_Angle)));
                    fixed strengthDelta = _Length / _Iterations;

                    for (int j = 0; j < _Iterations; ++j) {
                        fixed4 offsettedShadow = tex2D(_ShadowTex, i.uv + direction * strengthDelta * (j + 1));
                        shadow = max(shadow, offsettedShadow.r * offsettedShadow.a);
                    }

                    shadow = max(0, shadow - sampled.r * sampled.a);
                    fixed4 mapColor = tex2D(_MapTexture, i.uv);
                    fixed4 color = lerp(lerp(mapColor, _ShadowColor, shadow * _Strength), _MapColor, (1 - shadow) * _Affinity);
                    color.a = lerp(_Affinity, _Strength, shadow);
                    return color;
                }
                ENDCG
            }
        }
}
