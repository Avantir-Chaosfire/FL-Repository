//this shader just colors the entire thing black
Shader "Custom/FogShader" {
	SubShader {
		Tags { 
			//draw this shader last
			"Queue" = "Overlay" 
		}
		Zwrite On
		//if this shader is infront of other pixels, draw overthem
		Ztest LEqual
		Pass {
			Color (0.1,0.1,0.1,1)
		} 
	}
}
