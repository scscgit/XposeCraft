Shader "Fog Of War/Model Object" {
	Properties {
		_FOWTex ("Detail", 2D) = "gray" {}
		_MainTex ("BaseMap (RGB)", 2D) = "white" {}
				
	}
	SubShader {
		Tags { "RenderType"="Opaque" 
				"SplatCount" = "4"
			}
		CGPROGRAM
		#pragma surface surf Lambert nolightmap
		
		struct Input {
			float2 uv_FOWTex;
			float2 uv_MainTex;
		};
		
		sampler2D _MainTex, _FOWTex;
		
		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
			fixed3 c = tex2D (_FOWTex, IN.uv_FOWTex).aaa;
			c = 1-c;
			o.Albedo *= c;
		}
		ENDCG
	}
	FallBack off
}
