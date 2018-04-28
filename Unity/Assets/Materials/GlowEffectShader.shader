Shader "Hidden/GlowEffectShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
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

			struct VertIn
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 ray : TEXCOORD1;
			};

			struct VertOut
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 uv_depth : TEXCOORD1;
				float3 interpolatedRay : TEXCOORD2;
			};

			VertOut vert (VertIn v)
			{
				VertOut o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.uv_depth = v.uv.xy;

				float4 clip = float4(o.vertex.xy, 0.0, 1.0);
				o.interpolatedRay =  clip - _WorldSpaceCameraPos;

				return o;
			}
			
			sampler2D _MainTex;
			sampler2D_float _CameraDepthTexture;
			float4 _CameraPosition;
			float4 _HitPoints[100];

			fixed4 frag (VertOut i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				float4 resultColour = 0;
				float rawDepth = DecodeFloatRG(tex2D(_CameraDepthTexture, i.uv_depth));
				float linearDepth = Linear01Depth(rawDepth);
				float2 worldPos = (i.interpolatedRay.xy);

				if (linearDepth < 1) {
					for (int j = 0; j < 100; j++) {
						
						float dist = distance(_HitPoints[j].xy, worldPos);
						if (dist < 0.2) {
							resultColour = 1;
						}
					}
				}

				return col + resultColour;
			}
			ENDCG
		}
	}
}
