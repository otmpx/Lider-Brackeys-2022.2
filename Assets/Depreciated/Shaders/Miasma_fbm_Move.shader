Shader "Custom/Miasma_fbmMove"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex("Noise Texture", 2D) = "white" {}
        _NumOctaves("Octaves of FBM", int) = 6
        _Lacunarity("Lacunarity", float) = 2
        //        https://lospec.com/palette-list/ink-crimson
        //        https://corecoding.com/utilities/rgb-or-hex-to-float.php
        _col1("Colour 1", Color) = (1, 0.02, 0.275)
        _col2("Colour 2", Color) = (0.612, 0.09, 0.231)
        _coordZoom("Zooming out of coord", float) = 10
        _timeZoom("Zooming of how fast time is affected", Vector) = (0.5, 0.17,0,0)

    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100



        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"


            #define VALUE_NOISE

            #ifdef VALUE_NOISE
            // #define LERPING_VALUE_NOISE

            //hash gives a diagonal like pattern (no sine involved)
            float hash(float2 p)
            {
                float3 p3 = frac(float3(p.xyx) * 0.13);
                p3 += dot(p3, p3.yzx + 3.333);
                return frac((p3.x + p3.y) * p3.z);
            }

            float random(float2 st)
            {
                //Big sin wave combined with only getting the frac makes this really random
                return frac(sin(dot(st.xy,
                                    float2(12.9898, 78.233))) *
                    43758.5453123);
            }

            float noise(float2 input)
            {
                //Gives whole numbers
                float2 integer = floor(input);
                float2 fractional = frac(input);

                //corners
                float tl = hash(integer);
                float bl = hash(integer + float2(0, 1));
                float tr = hash(integer + float2(1, 0));
                float br = hash(integer + float2(1, 1));
                #ifndef LERPING_VALUE_NOISE

                // https://www.desmos.com/calculator/bu60hn25dm
                //           normal quadratic       (becomes cubic at x=1)
                fractional = pow(fractional, 2.0) * (3.0 - 2.0 * fractional);

                #endif
                //This is lerping so it might look rough 
                float topMix = lerp(tl, tr, fractional.x);
                float botMix = lerp(bl, br, fractional.x);
                float wholeMix = lerp(topMix, botMix, fractional.y);

                return wholeMix;
            }

            #else
        float noise(float input)
        {
            
        }
            #endif

            int _NumOctaves;
            float _Lacunarity;
            fixed4 _col1;
            fixed4 _col2;
            float _coordZoom;
            float2 _timeZoom;

            //Functions with especially for loops are more optimized in shader code when the loops are hard baked
            //TODO: need normalization?
            float fbm(float2 input)
            {
                float value = 0;
                float scale = .5f;
                // float normalize = 0;
                for (int i = 0; i < _NumOctaves; i++)
                {
                    value += noise(input) * scale;
                    // normalize += scale;
                    //Zoom out of noise every so often
                    //Bottom left seems blocky with lower values`
                    input *= _Lacunarity;
                    scale *= 0.5f;
                }
                // return value / normalize;
                return value;
            }


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
            float4 _MainTex_ST;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = (0, 0, 0, 0);
                //Zoomed out
                float2 coord = i.uv * _coordZoom;
                //Zoomed in fbm looks like a lot of oscillation
                float2 movement = float2(noise(coord + _Time.y * _timeZoom.x), noise(coord + _Time.y * _timeZoom.y));
                // float2 movement = float2(noise(coord + _Time.y ), noise(coord + _Time.y )) * _timeZoom;
                movement = clamp(movement, (0,0), (1,1));
                col = lerp(_col1, _col2, clamp(fbm( coord+ movement), 0, 1));

                // col = lerp(_col1, _col2,  clamp(movement, 0, 1));
                return col;
            }
            ENDCG
        }
    }
}