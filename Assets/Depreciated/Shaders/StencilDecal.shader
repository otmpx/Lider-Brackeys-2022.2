Shader "Custom/SencilDecal"
{
    Properties
    {
        //        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        //Transparent queue important otherwise defects detected
        Tags
        {
            "RenderType"="Opaque" "Queue"= "Transparent"
        }
        LOD 100
        ColorMask 0
        //        ZTest Greater
        //        Cull Back
        //        ZWrite On
        //        ZTest Off
        //        Cull Front //This makes it so that only back collisions show up 
        //        Cull Back

        //        Stencil
        //            {
        //                Ref 5
        //                Comp Always
        //                Pass Replace
        ////                ZFail Replace
        //            }
        Pass
        {
            Cull Front
            
//            ZTest Greater
            Stencil
            {
                Ref 5
                Comp Always
                Pass Replace
                //                ZFail Replace
            }
        }
        Pass
        {
            Cull Back
            ZWrite Off
            ZTest Always
            Stencil
            {
                Ref 5
                Comp Always
                Pass DecrWrap
            }
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