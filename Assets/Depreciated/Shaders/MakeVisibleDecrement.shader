Shader "Custom/MakeVisibleDecrement"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,0)
        _ColorMask ("Color Mask(15==RGBA)(0==NONE)", Float) = 0
        //        _MainTex ("Texture", 2D) = "white" {}
        _Stencil("Stencil mask", Int) = 4
        _OtherStencil("Other Stencil mask", Int) = 5

    }
    SubShader
    {
        //Transparent queue important otherwise defects detected with opaque
        Tags
        {
            "Queue"= "Transparent+6"
        }
        //All blood stencil effects will be secondary and are dependant on the value after skeleton unvisible
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            //if behind skeleton value is 3
            //this pass changes value to 1 only if value is two (behind viewCylinder but not behind skeleton)
            ColorMask 0
            Stencil
            {
                Ref 2
                Comp Equal
                Pass DecrWrap
            }
        }

        Pass
        {
            ColorMask 0
            Stencil
            {
                Ref 0
                Comp Equal
                Pass IncrWrap
            }
        }

        Pass
        {
            ColorMask [_ColorMask]
            Blend SrcAlpha OneMinusSrcAlpha
            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            // UNITY_INSTANCING_BUFFER_START(Props)
            fixed4 _Color;
            // UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            // UNITY_INSTANCING_BUFFER_END(Props)

            v2f vert(appdata v)
            {
                v2f o;
                 UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }


            fixed4 frag(v2f i) : SV_Target
            {
                // return fixed4(1,0,1,1);
                return _Color;
            }
            ENDCG
        }


    }
}