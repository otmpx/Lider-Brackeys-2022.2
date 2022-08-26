Shader "Custom/InstancedSpec"
{
	Properties
	{
		//_MainTex("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType" = "Transparent" }
		Lighting Off
		Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma instancing_options maxcount:1024
			//This may not be allowed if the spare buffers were too big, if that was the case, use DrawMeshInstancedIndirect instead
			#pragma multi_compile_instancing 
			//https://forum.unity.com/threads/understanding-instancing-and-drawmeshinstanced.445995/

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
				fixed4 colour : COLOR;
			};

			float4 _Colours[1023];
			float _Times[1023];
			sampler2D _MainTex;

//https://github.com/AlfonsoLRz/PointCloudRendering/blob/main/PointCloudRendering/Assets/Shaders/Points/pointCloud-frag.glsl
			//Need the uv coord for rounding on sphere

			v2f vert(appdata v,uint instanceID: SV_InstanceID)
			{
				UNITY_SETUP_INSTANCE_ID(v);

				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
                //Can just pass the UV directly wihthout Maintex
                o.uv = v.uv;

#ifdef UNITY_INSTANCING_ENABLED
				o.colour = _Colours[instanceID];
#endif
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return i.colour;
			}
			ENDCG
		}
	}
}
