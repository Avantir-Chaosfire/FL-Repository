using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataBase {

	//a list of the colliders of solid wall objects that is currently visible on camera
	public static List<Vector2[]> corners = new List<Vector2[]>();

	public static float cameraZ = -10f;
	public static float fogZ = 2f;
	public static float lightZ = 1f;//lightZ must be smaller then fogZ AND bigger then objectZ(duno why for second one)
	public static float objectZ = 0f;

	//currently, the only value that matters in lightColor is its alpha value
	public static Color fogColor = Color.black;
	public static Color getLightColor(int numOfLights)
	{
		float alpha = 1f / (numOfLights - 0.9f);
		return new Color(1f,1f,1f,alpha);
	}
}
