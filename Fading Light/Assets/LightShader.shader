Shader "Custom/LightShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.0
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
         Tags { "Queue" = "Transparent" }
         Pass
          {
              Blend Zero One
              Lighting On
              ZWrite On
              Material
              {
                  Diffuse (0,0,0,0)
              }
          }
     } 
}
