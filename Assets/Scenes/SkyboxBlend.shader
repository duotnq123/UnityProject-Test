Shader "Custom/SkyboxBlend"
{
    Properties
    {
        _Tex1 ("Cubemap 1", CUBE) = "" {}
        _Tex2 ("Cubemap 2", CUBE) = "" {}
        _Blend ("Blend", Range(0,1)) = 0
        _Exposure ("Exposure", Range(0,8)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "IgnoreProjector"="True" }
        Cull Off
        ZWrite Off
        Fog { Mode Off }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            samplerCUBE _Tex1;
            samplerCUBE _Tex2;
            float _Blend;
            float _Exposure;

            struct appdata { float4 vertex : POSITION; };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 dir : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                // lấy direction từ camera -> world position của vertex (đảm bảo skybox hướng đúng)
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.dir = normalize(worldPos - _WorldSpaceCameraPos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c1 = texCUBE(_Tex1, i.dir);
                fixed4 c2 = texCUBE(_Tex2, i.dir);
                fixed4 col = lerp(c1, c2, saturate(_Blend)) * _Exposure;
                return col;
            }
            ENDCG
        }
    }

    Fallback "RenderFX/Skybox"
}
