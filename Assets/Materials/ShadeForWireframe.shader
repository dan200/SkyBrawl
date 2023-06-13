Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _Multiplier ("Multiplier", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            fixed4 _Multiplier;

            struct appdata
            {
                float4 position : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                fixed4 colour : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                float3 worldPos = mul(unity_ObjectToWorld, v.position);
                float3 worldNormal = mul(unity_ObjectToWorld, float4( v.normal, 0.0 )).xyz;

                float3 normalColour = float3(0.5, 0.5, 0.5) + 0.5 * worldNormal * _Multiplier;
                float distance = dot(_WorldSpaceCameraPos - worldPos, worldNormal);
                float distanceNormalised = 2.0 * abs(frac(distance*0.02) - 0.5);
                float3 finalColour = lerp(normalColour, float3(1.0, 1.0, 1.0) - normalColour, distanceNormalised);

                v2f o;
                o.position = UnityObjectToClipPos(v.position);
                o.colour = fixed4(finalColour, 1.0);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				return i.colour;
            }
            ENDCG
        }
    }
}
