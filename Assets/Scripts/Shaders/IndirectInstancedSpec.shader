Shader "Custom/IndirectInstancedSpec"
{
	SubShader
	{
		Tags { "RenderType" = "Transparent" }
		//Tags { "RenderType" = "Opaque" }
		Lighting Off
			Fog { Mode Off }

		Blend SrcAlpha OneMinusSrcAlpha
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
				float4 pos : SV_POSITION;
				float4 color : COLOR;
			};

			struct StaticPointDef
			{
				float4 posScale;
				float4 color;
			};

			StructuredBuffer<StaticPointDef> particles;

			v2f vert(appdata v,uint instanceID: SV_InstanceID)
			{
				v2f o;
				StaticPointDef data = particles[instanceID];

				//Scale based on size
				float3 localPosition = v.vertex.xyz * data.posScale.w;
				float3 worldPosition = data.posScale.xyz + localPosition;

				//https://forum.unity.com/threads/what-is-the-equivalent-of-unityobjecttoclippos-inside-shader-graph.809778/
				// First, the UnityObjectToClipPos() function.That transforms the mesh vertex position from local object space to clip space, as the function implies.It does that by transforming from object to world space, 
				// which is easy enough to understand, and then world to clip space using the view projection matrix.Clip space, or more accurately, homogeneous clip space, 
				// is a special 4 component space that describes the screen space position of a vertex.

				//It converts the vertex to world space first, then multiplies by View Project matrix (VP)
				//o.vertex = UnityObjectToClipPos(v.vertex);

				o.pos = mul(UNITY_MATRIX_VP, float4(worldPosition, 1.0f));

				o.color = data.color;
				o.uv = v.uv;

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return i.color;
			}
			ENDCG
		}
	}
}
