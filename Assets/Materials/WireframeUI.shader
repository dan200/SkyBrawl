Shader "WireframeUI"
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
        Cull Off 
        ZWrite Off 
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

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
            fixed4 _BrightColour;
            fixed4 _DarkColour;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColour = tex2D(_MainTex, i.uv);
                if(texColour.a < 0.5)
                {
                    return fixed4(_DarkColour.rgb, 0.0);   
				}
                else if(texColour.r > 0.5)
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
