Shader "Custom/FinalPass" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
         Tags { 
         	//draw this shader right before drawing fogShader
         	"Queue" = "Overlay"}
         Zwrite On
         //draw over everything
         Ztest Always  
      	 Pass {
      	 	 Color[_Color]//should be fog color
      	 	 //Blend SrcAlpha OneMinusSrcAlpha
      	 	 Blend DstAlpha OneMinusDstAlpha
      	 	 //Blend OneMinusSrcAlpha SrcAlpha
      	 	 //Blend SrcAlpha DstAlpha
      	 	 //BlendOp RevSub
      	 	 //BlendOp RevSub
      	 	 //Blend SrcAlpha One
      	 }  	 
     } 
}
