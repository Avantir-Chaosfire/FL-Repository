using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataBase {

	//a list of the colliders of solid wall objects that is currently visible on camera
	public static List<PolygonCollider2D> corners = new List<PolygonCollider2D>();

	public static float cameraZ = -10f;
	public static float fogZ = 0f;
	public static float lightZ = -1f;//lightZ must be smaller then fogZ
	public static float objectZ = 0f;
	public static float solidWallZ = -1;//will appear on top of fog. (when you want it to be hidden by fog, move it down to objectZ)
}
