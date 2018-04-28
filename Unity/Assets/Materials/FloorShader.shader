Shader "Custom/FloorShader" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_GridColour("Grid Colour", Color) = (1,0,0,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_NoiseTex("Noise Tex", 2D) = "white"{}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_GridThickness("Grid Thickness", Float) = 1.0
		_GridScale("Grid Scale", Float) = 1
		[MaterialToggle]
		_EnableMainGrid("Use Main Grid", Float) = 1
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha:fade

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _NoiseTex;

		struct Input {
			float2 uv_MainTex;
			float2 uv_NoiseTex;
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		float _GridThickness;
		float _GridScale;
		float _EnableMainGrid;
		fixed4 _Color;
		fixed4 _GridColour;

		float _using[100];
		float4 _positions[100];
		float4 _colours[100];

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {

			// Pick a coordinate to visualize in a grid
			float2 coord = IN.worldPos.xy/ _GridScale;

			// Compute anti-aliased world-space grid lines
			float2 grid = abs(frac(coord - 0.5) - 0.5) / fwidth(coord*_GridThickness);
			float gridLine = min(grid.x, grid.y);

			float2 coord2 = IN.worldPos.xy / (_GridScale/4);

			float2 grid2 = abs(frac(coord2 - 0.5) - 0.5) / fwidth(coord2*_GridThickness);
			float gridLine2 = min(grid2.x, grid2.y);

			// Just visualize the grid lines directly
			float gridAlpha = (((1 - min(gridLine, 1.0))*_EnableMainGrid) + (1 - min(gridLine2, 1.0)))*0.5;

			float4 _addedColour = float4(0,0,0,1);

			for (int j = 0; j < 100; j++)
			{
				if (_using[j] == 1) {
					float dist = distance(IN.worldPos.xy, _positions[j]);
					_addedColour += _colours[j] * -min(0, (dist-1)/1) * 2 * _colours[j].a;
				}
			}

			float2 uv = IN.uv_MainTex + float2(_Time.x, _Time.x);
			float noise = tex2D(_NoiseTex, uv).r;
			float4 _colourNoise = float4(1,1,1,1) * 1.0f * (noise * noise * noise);

			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * lerp(_Color, _GridColour + _addedColour + _colourNoise, gridAlpha);
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = 1;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
