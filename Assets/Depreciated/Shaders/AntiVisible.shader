//Antivisible is cringe because it has same problem just at top of inside cyulinder where it gets zwritten
Shader "Unlit/AntiVisible"
{
    Properties
    {
        //        _MainTex ("Texture", 2D) = "white" {}
        _StencilMask("Stencil mask", Int) = 5
    }
    SubShader
    {
        //Transparent queue important otherwise defects detected
        Tags
        {
            "RenderType"="Opaque" "Queue"= "Transparent+1"
        }
        LOD 100
        ColorMask 0
        
//        ZWrite Off
        Stencil
        {
            Ref 5
            Comp Always
            Pass Replace
        }

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return fixed4(1, 1, 1, 1);
            }
            ENDCG
        }


    }
}