Shader "Unlit/MakeVisibleTest"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,0)
        _ColorMask ("Color Mask(15==RGBA)(0==NONE)", Float) = 0
        _Stencil("Stencil mask", Int) = 7
        //        _StencilMinus("Stencil mins1", Int) = 6
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque" "Queue"= "Transparent+1"
        }
        LOD 100
        ColorMask [_ColorMask]
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            Cull Back
            ColorMask 0
//            ZWrite Off
            Stencil
            {
                Ref [_Stencil]
                Comp Always
                Pass Replace
            }

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

            fixed4 _Color;

            fixed4 frag(v2f i) : SV_Target
            {
                return _Color;
            }
            ENDCG


        }
        Pass
        {
            Cull Front
            ZWrite Off
            ColorMask 0
            Lighting Off

            Stencil
            {
                Ref [_Stencil]
                Comp Always
                Pass Zero
                ZFail IncrWrap
            }


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

            fixed4 _Color;

            fixed4 frag(v2f i) : SV_Target
            {
                return _Color;
            }
            ENDCG

        }
        //        Pass
        //        {
        //
        //            CGPROGRAM
        //            #pragma vertex vert
        //            #pragma fragment frag
        //            #include "UnityCG.cginc"
        //
        //            struct appdata
        //            {
        //                float4 vertex : POSITION;
        //                float2 uv : TEXCOORD0;
        //            };
        //
        //            struct v2f
        //            {
        //                float2 uv : TEXCOORD0;
        //                float4 vertex : SV_POSITION;
        //            };
        //
        //            v2f vert(appdata v)
        //            {
        //                v2f o;
        //                o.vertex = UnityObjectToClipPos(v.vertex);
        //                return o;
        //            }
        //
        //            fixed4 _Color;
        //
        //            fixed4 frag(v2f i) : SV_Target
        //            {
        //                return _Color;
        //            }
        //            ENDCG
        //        }


    }
}