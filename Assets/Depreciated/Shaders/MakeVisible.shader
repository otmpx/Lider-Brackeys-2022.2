Shader "Custom/MakeVisible"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,0)
        _ColorMask ("Color Mask(15==RGBA)(0==NONE)", Float) = 0
        //        _MainTex ("Texture", 2D) = "white" {}
        _Stencil("Stencil mask", Int) = 5
    }
    SubShader
    {
        //Transparent queue important otherwise defects detected
        Tags
        {
            "RenderType"="Opaque" "Queue"= "Transparent+1"
        }
        LOD 100
        ColorMask [_ColorMask]
        Blend SrcAlpha OneMinusSrcAlpha
        //        ZTest Greater
        //        Cull Back
        //        ZWrite On
        //        ZTest Off
        //        Cull Front //This makes it so that only back collisions show up 
        //        Cull Back

        ZWrite Off
//        Ztest Off

        //        https://docs.unity3d.com/2019.3/Documentation/Manual/SL-Stencil.html
        Stencil
        {
            Ref [_Stencil]
            Comp Always
            Pass Replace
            //                ZFail ReplacZTest Alwayse
        }
        //        Pass
        //        {
        //            Cull Front
        //            //            ZWrite On
        //            //            ZTest Greater
        //            ZTest Less
        //
        //            //                    Stencil
        //            //                    {
        //            //                        Ref 5
        //            //                        Comp Always
        //            //                        Pass Replace
        //            //                        //                ZFail Replace
        //            //                    }
        //        }
        //        Pass
        //        {
        //            Cull Back
        //            ZTest Greater
        //            //            ZTest GEqual
        //            //            ZWrite Off
        //            //                    Stencil
        //            //                    {
        //            //                        Ref [_StencilMask]
        //            //                        Comp Always
        //            //                        Pass IncrWrap
        //            //                    }
        //        }
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

            fixed4 _Color;
            
            fixed4 frag(v2f i) : SV_Target
            {
                return _Color;
            }
            ENDCG
        }


    }
}