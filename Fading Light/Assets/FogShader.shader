Shader "Custom/FogShader" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader {
        Tags { "Queue" = "Overlay" }
        Pass {
            SetTexture [_MainTex] { combine texture }
        }
    }
}
