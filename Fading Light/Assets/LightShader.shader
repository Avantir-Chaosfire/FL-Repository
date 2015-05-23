//(make sure this shader's z is > then that of fogShader so it stays infront of fogShader)
//this shader uses alpha blending
Shader "Custom/LightShader" {
	Properties {
		_Alpha ("Alpha", float) = 0//invis
		_Color ("Color", Color) = (1,1,1,1)//black
	}
	SubShader {
         Tags { 
         	//draw this shader right before drawing fogShader
         	"Queue" = "Overlay-1"}
         Zwrite On
         //draw over everything
         Ztest Always  
      	 Pass {
      	 	 Color[_Color]
      	 	 Blend SrcAlpha OneMinusSrcAlpha
      	 }
     } 
}
