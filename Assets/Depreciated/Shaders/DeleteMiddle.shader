Shader "Custom/DeleteMiddle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        blendTex("Background Texture", 2D) = "white" {}
        radius ("Radius", float) = 0.5
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent" "RenderType" = "Transparent"
        }
        LOD 100
        //        Blend One OneMinusSrcAlpha
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        Lighting Off

        //        CGPROGRAM
        //        float IsInsideCircle(float2 location, float rad)
        //        {
        //        }
        //        ENDCG

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
            sampler2D blendTex;
            float4 _MainTex_ST;
            float radius;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                if (distance(i.uv, float2(0.5, 0.5)) < radius)
                {
                    col = tex2D(blendTex, i.uv);
                }

                return col;
            }
            ENDCG
        }
    }
}