Shader "Custom/PitShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_MoveX("Movement on X", Float) = 0.0
		_MoveY("Movement on Y", Float) = 0.0
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

		half _Glossiness;
		half _Metallic;
		float _MoveX;
		float _MoveY;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

			float2 mirrorTile(float2 _st, float _zoom) {
			_st *= _zoom;
			if (frac(_st.y * 0.5) > 0.5) {
				_st.x = _st.x + 0.5;
				_st.y = 1.0 - _st.y;
			}
			return frac(_st);
		}

		float fillY(float2 _st, float _pct, float _antia) {
			return  smoothstep(_pct - _antia, _pct, _st.y);
		}
		void surf (Input IN, inout SurfaceOutputStandard o) {

			float2 st = IN.worldPos.xy;
			st.y += _Time.y*_MoveY;
			st.x += _Time.x*_MoveX;
			st = mirrorTile(st*float2(2*0.1, 4.*0.1), 5.);
			float x = st.x*2.;
			float a = floor(1. + sin(x*3.14));
			float b = floor(1. + sin((x + 1.)*3.14));
			float f = frac(x);

			float result = fillY(st, lerp(a, b, f), 0.01);

			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb * result;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
