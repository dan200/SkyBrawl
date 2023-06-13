Shader "Wireframe"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BrightColour ("Bright Colour", Color) = (1, 1, 1, 1)
        _DarkColour ("Dark Colour", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.position = UnityObjectToClipPos(v.position);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            fixed4 _BrightColour;
            fixed4 _DarkColour;

            bool isDifferent(fixed3 a, fixed3 b)
            {
                float threshold = 1.0 / 64.0;
                return distance(a, b) > threshold;            
			}

            bool isWhite(fixed3 colour)
            {
                return !isDifferent(colour, fixed3(1.0, 1.0, 1.0));
			}

            fixed4 frag (v2f i) : SV_Target
            {
                float2 p = _MainTex_TexelSize.xy;
                fixed3 c = tex2D(_MainTex, i.uv).rgb;
                fixed3 n = tex2D(_MainTex, i.uv + float2(0.0, -p.y)).rgb;
                fixed3 e = tex2D(_MainTex, i.uv + float2(-p.x, 0.0)).rgb;
                fixed3 ne = tex2D(_MainTex, i.uv + float2(-p.x, -p.y)).rgb;

                bool nDiff = isDifferent(c, n) && !isWhite(n);
                bool eDiff = isDifferent(c, e) && !isWhite(e);
                bool neDiff = isDifferent(c, ne) && !isWhite(ne);
                bool isOutline = nDiff || (eDiff && neDiff);

                if( isOutline || isWhite(c) )
                {
                    return _BrightColour;        
				}
                else
                {
                    return _DarkColour;        
				}
            }
            ENDCG
        }
    }
}
