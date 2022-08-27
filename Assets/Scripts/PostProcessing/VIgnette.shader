Shader "Custom/Vignette"
{
	Properties
	{
		_MainTex("Texture", 2D) = "" {}
		_Falloff("Falloff", float) = 0.5
		_Power("Power", float) = 0.25
	}
		CGINCLUDE

			//Putting functions here allows them to be used
#include "UnityCG.cginc"

			sampler2D _MainTex;
		float2 _Aspect;
		float _Falloff;
		float _Power;

		half4 frag(v2f_img i) : SV_Target
		{
			//float2 coord = (i.uv - 0.5) * _Aspect * 2;
			//float rf = sqrt(dot(coord, coord)) * _Falloff;
			//float rf2_1 = rf * rf + 1.0;
			//float e = 1.0 / (rf2_1 * rf2_1);

			////half4 src = tex2D(_MainTex, i.uv);

			////return half4(src.rgb * e, src.a);

			float2 uv = i.uv;
			uv *= float2(1, 1) - i.uv.yx;
		float vig = uv.x * uv.y * _Falloff;

		vig = pow(vig, _Power);
			half4 src = tex2D(_MainTex, i.uv);
			//half3 vegvec = half3(vig.x);

			//return half4(vig, vig, vig, src.a);
			return half4(src.rgb * vig, src.a);
		}

			ENDCG



			SubShader
		{
			Pass
			{
				ZTest Always Cull Off ZWrite Off
				//Post processing shaders cant be transparent otherwise it doesnt refresh screen ans tears
		//	Blend SrcAlpha OneMinusSrcAlpha
				CGPROGRAM
				//This is a default for vertex buffer I guess
				#pragma vertex vert_img
				#pragma fragment frag
			ENDCG
			}
		}
}
