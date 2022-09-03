Shader "Custom/IndirectInstancedSpec"
{
	Properties{
	_Color("Colour", Color) = (1,1,1,1)
	}

		SubShader
	{
		//Tags { "RenderType" = "Transparent" }
		Tags { "RenderType" = "Opaque" }
		Lighting Off
			Fog { Mode Off }

		//Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma enable_d3d11_debug_symbols
			#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
			//#pragma multi_compile


			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				//float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				//float2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
				//PSIZE only supported on opengl not dx11
				half psize : PSIZE;
			};

			StructuredBuffer<float4> points;

			//v2f vert(appdata v,uint instanceID: SV_InstanceID)
			v2f vert(uint instanceID: SV_VertexID)
			{
				v2f o;
				float4 posScale = points[instanceID];

				////Scale based on size
				//float3 localPosition = v.vertex.xyz * posScale.w;
				//float3 worldPosition = posScale.xyz + localPosition;
				//float3 worldPosition = float3(0, 0, 0);

				//https://forum.unity.com/threads/what-is-the-equivalent-of-unityobjecttoclippos-inside-shader-graph.809778/
				// First, the UnityObjectToClipPos() function.That transforms the mesh vertex position from local object space to clip space, as the function implies.It does that by transforming from object to world space, 
				// which is easy enough to understand, and then world to clip space using the view projection matrix.Clip space, or more accurately, homogeneous clip space, 
				// is a special 4 component space that describes the screen space position of a vertex.

				//It converts the vertex to world space first, then multiplies by View Project matrix (VP)
				//o.vertex = UnityObjectToClipPos(v.vertex);

				o.pos = mul(UNITY_MATRIX_VP, float4(posScale.xyz, 1.0f));
				//o.pos = UnityObjectToClipPos(float4(posScale.xyz, 1.0f));
				o.psize = 20;

				//o.uv = v.uv;

				return o;
			}
			float4 _Color;

			fixed4 frag(v2f i) : SV_Target
			{
				return _Color;
			}
			ENDCG
		}
	}
}
