Shader "Custom/WallShader" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_BounceColour("Bounce Colour", Color) = (1,1,1,1)
		_RingThickness("Ring Thickness", Float) = 0.2
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		float _maxDistance = 5.f;
		float _RingThickness;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed4 _BounceColour;
		float4 _HitPoints[100];

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;

			for (int j = 0; j < 100; j++) {

				float dist = distance(_HitPoints[j].xy, IN.worldPos.xy)/5;
				if (dist < _HitPoints[j].w && dist > _HitPoints[j].w - _RingThickness) {
					c += _BounceColour * (0.5f * -(dist-5)/5);
				}
			}

			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
