//(make sure this shader's z is > then that of fogShader so it stays infront of fogShader)
//this shader uses alpha blending
Shader "Custom/LightShader" {
	Properties {
		_Alpha ("Alpha", float) = 0//invis
		_Color ("Color", Color) = (1,1,1,1)//white
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
      	 	 //Blend SrcAlpha OneMinusSrcAlpha
      	 	 Blend Zero One, SrcAlpha DstAlpha
      	 	 BlendOp RevSub
      	 	 //Blend OneMinusSrcAlpha SrcAlpha
      	 	 //Blend SrcAlpha DstAlpha
      	 	 //BlendOp RevSub
      	 	 //BlendOp RevSub
      	 	 //Blend SrcAlpha One
      	 }  	 
     } 
}
