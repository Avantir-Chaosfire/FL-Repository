﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LightScript : MonoBehaviour {


	private List<RaycastHit2D> rayHits;
	private RaycastHit2D[] rayHitsArray;

	public float coneWidth;
	public float coneDistance;
	public float angle;

	private MeshRenderer renderer;
	private MeshFilter filter;

	// Use this for initialization
	void Start () {
		transform.position = new Vector3 (transform.position.x,transform.position.y,-1f);
		rayHits = new List<RaycastHit2D> ();

		renderer = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
		filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
		renderer.material.shader = Shader.Find ("Custom/LightShader");
		renderer.material.color = new Color(1f,1f,1f,0.25f);//or whatever color the light is

		coneWidth = 359f;
		coneDistance = 4f;
		angle = 0f;

		renderer.sortingLayerName = "Light";// and .sortingOrder = blah;
	}
	
	// Update is called once per frame
	void Update () {
		castRays(transform.position, angle, coneDistance);
		drawLight(rayHitsArray);
	}

	void drawLight(RaycastHit2D[] rayHitsArray)
	{
		if(coneWidth >= 359f)
		{
			Vector2[] thePoints = new Vector2[rayHitsArray.Length];
			for(int n = 0; n < rayHitsArray.Length; n ++)
			{
				thePoints[n] = rayHitsArray[n].point;
			}
			toLocalSpace(thePoints);
			filter.mesh = createMesh(thePoints);
		}
		else
		{
			Vector2[] thePoints = new Vector2[rayHitsArray.Length+1];
			thePoints[0] = transform.position;
			for(int n = 1; n < rayHitsArray.Length+1; n ++)
			{
				thePoints[n] = rayHitsArray[n-1].point;
			}
			toLocalSpace(thePoints);
			filter.mesh = createMesh(thePoints);
			//set rotation to 0 to fix
		}
	}

	//angles goes COUNTER-CLOCKWISE from start to end, angles should be normalized before using
	void castRays(Vector2 origin, float angle, float distance)
	{
		//get all polygonCollider2d corners on screen (stored in DataBase)
		//draw a ray toward the angles and all corners withing the angles
		rayHits.Clear();//clear previous frame's rayHits

		float angleStart = normalizeAngle (angle - coneWidth/2);
		float angleEnd = normalizeAngle (angle + coneWidth/2);

		Vector2 directionStart = convertToPoint(angleStart);
		Vector2 directionEnd = convertToPoint(angleEnd);

		RaycastHit2D a = Physics2D.Raycast(origin, directionStart, distance, (1 << LayerMask.NameToLayer("SolidWallLayer")));
		RaycastHit2D b = Physics2D.Raycast(origin, directionEnd, distance, (1 << LayerMask.NameToLayer("SolidWallLayer")));
		a.centroid = origin;
		b.centroid = origin;
		if(!a.collider) a.point = (directionStart.normalized*distance) + origin;
		if(!b.collider) b.point = (directionEnd.normalized*distance) + origin;
		rayHits.Add(a);
		rayHits.Add(b);

		float smoothness = 7f;//draw a ray every 5 degree
		float newAngle = angleStart+smoothness;
		float amount = coneWidth / smoothness;
		while(newAngle < angleStart+amount*smoothness)
		{
			Vector2 hurdur = convertToPoint(newAngle);
			
			RaycastHit2D derp = Physics2D.Raycast(origin, hurdur, distance, (1 << LayerMask.NameToLayer("SolidWallLayer")));
			derp.centroid = origin;
			if(!derp.collider) derp.point = (hurdur.normalized*distance) + origin;
			rayHits.Add(derp);

			newAngle += smoothness;
		}

		for(int n = 0; n < DataBase.corners.Count; n ++)
		{
			for(int i = 0; i < DataBase.corners[n].points.Length; i ++)
			{
				Vector2 temp = toWorldSpace(DataBase.corners[n], i);
				Vector2 direction = new Vector2(temp.x - origin.x, temp.y - origin.y);
				float dirAngle = convertToAngle(direction);

				//if the corner is withing the light cone, cast a ray toward it
				if(isBetween(angleStart, angleEnd, dirAngle))
				{
					//cast 2 rays offset by 0.0001f so the ray will keep going if it hits a corner
					Vector2 offSet1 = convertToPoint(dirAngle + 0.01f);
					Vector2 offSet2 = convertToPoint(dirAngle - 0.01f);
					RaycastHit2D c = Physics2D.Raycast(origin, offSet1, distance, (1 << LayerMask.NameToLayer("SolidWallLayer")));
					RaycastHit2D d = Physics2D.Raycast(origin, offSet2, distance, (1 << LayerMask.NameToLayer("SolidWallLayer")));
					c.centroid = origin;
					d.centroid = origin;

					//if both miss then this thing is out of range
					if(!c.collider && !d.collider)
					{
						//dont bother doing anything
					}
					//if both offsets hit, just use one ray(reduce triangles)
					else if(c.collider == d.collider)
					{
						RaycastHit2D e = Physics2D.Raycast(origin, direction, distance, (1 << LayerMask.NameToLayer("SolidWallLayer")));
						e.centroid = origin;
						rayHits.Add (e);
					}
					else//only one of the offsets must have missed(cant both miss unless out of range)
					{
						if(!c.collider) 
							c.point = offSet1.normalized*distance + origin;
						if(!d.collider)
							d.point = offSet2.normalized*distance + origin;

						rayHits.Add (c);
						rayHits.Add (d);
					}
				}
			}
		}
		rayHitsArray = rayHits.ToArray();
		sortRayHits(rayHitsArray, angleEnd+0.01f);
	}

	///////////////////////CALCULATION FUNCTIONS/////////////////////////////
	/////////////////////////////////////////////////////////////////////////

	//return a angle between 0 and 359
	static float normalizeAngle(float angle)
	{
		float a = angle % 360;
		if (a < 0)
			a += 360;
		return a;
	}

	//calculates the angle of a line given origin and end point
	static float calcAngle(Vector2 orig, Vector2 end)
	{
		Vector2 e = end - orig;
		return normalizeAngle(Mathf.Atan2(e.y, e.x)*Mathf.Rad2Deg);
	}
	static float calcAngle(RaycastHit2D a)
	{
		return calcAngle(a.centroid, a.point);
	}

	//return true if angle a is between angle start and end (counter-clockwise from start to end)
	static bool isBetween(float start, float end, float a)
	{
		float barriar = 360f;
		float barriar2 = 0f;
		if (start < end)//end not cross barriar
			return (a > start && a < end);
		else//end crosses barriar
			return (a > start && a <= barriar) || (a >= barriar2 && a < end);
	}
	
	//returns the world points equvalent of the local point in polygoncollider2d
	Vector2 toWorldSpace(PolygonCollider2D c, int index)
	{
		Vector2 temp = c.transform.position;
		return temp + c.points[index];
	}
	
	//converts an array or world points to local point centered around origin
	void toLocalSpace(Vector2[] worldPoints)
	{
		for(int n = 0; n < worldPoints.Length; n++)
		{
			worldPoints[n] = worldPoints[n] - (Vector2)transform.position;
		}
	}
	
	float convertToAngle(Vector2 v)
	{
		return normalizeAngle(Mathf.Atan2(v.y, v.x)*Mathf.Rad2Deg);
	}
	Vector2 convertToPoint(float a)
	{
		return new Vector2 (Mathf.Cos (Mathf.Deg2Rad * a), Mathf.Sin (Mathf.Deg2Rad * a));
	}

	///////////////////////HELPER CLASSES and SORTING FUNCTION///////////////
	/////////////////////////////////////////////////////////////////////////

	//use this ONLY to sort RaycastHit2D!!!
	private class AngleSorter : IComparer
	{
		float startingAngle;
		public AngleSorter(float s)
		{
			startingAngle = s;
		}
		int IComparer.Compare(object a, object b)
		{
			RaycastHit2D hit1 = (RaycastHit2D)a;
			RaycastHit2D hit2 = (RaycastHit2D)b;
			if (isBetween (startingAngle, calcAngle(hit2), calcAngle(hit1)))
				return 1;
			else if (isBetween (startingAngle, calcAngle(hit1), calcAngle(hit2)))
				return -1;
			else
				return 0;
		}
	}

	//sort the raycasts by their angle, starting from angleStart (CLOCKWISE)
	void sortRayHits(RaycastHit2D[] hits, float angleStart)
	{
		AngleSorter sort = new AngleSorter(angleStart);
		Array.Sort(hits, sort);
	}

	///////////////////////DEBUGGING FUNCTIONS///////////////////////////////
	/////////////////////////////////////////////////////////////////////////

	//create a mesh that represent where the light hits
	Mesh createMesh(Vector2[] points)
	{
		Triangulator t = new Triangulator (points);
		
		// Use the triangulator to get indices for creating triangles
		int[] indices = t.Triangulate();
		
		// Create the Vector3 vertices
		Vector3[] vertices = new Vector3[points.Length];
		for (int i=0; i<vertices.Length; i++) {
			vertices[i] = new Vector3(points[i].x, points[i].y, 0);
		}
		
		// Create the mesh
		Mesh msh = new Mesh();
		msh.vertices = vertices;
		msh.triangles = indices;
		msh.RecalculateNormals();
		msh.RecalculateBounds();
		
		return msh;
	}	

	//print angle of rayHitList
	void check()
	{
		for(int i=0; i<rayHitsArray.Length; i++) 
		{
			Debug.Log(calcAngle(rayHitsArray[i]));
		}
	}

	//uses world space, so call this before converting to local space
	void debugLines()
	{
		for(int n = 0; n < rayHitsArray.Length; n++)
		{
			Vector2 direction = new Vector2 (rayHitsArray[n].point.x - rayHitsArray[n].centroid.x,
			                                 rayHitsArray[n].point.y - rayHitsArray[n].centroid.y);
			if(n == 0 || n == rayHitsArray.Length-1)
				Debug.DrawRay(rayHitsArray[n].centroid, direction, Color.yellow);
			else
				Debug.DrawRay(rayHitsArray[n].centroid, direction, Color.white);
		}
	}
	/////////////////////////////////////////////////////////////////////////
	/////////////////////////////////////////////////////////////////////////
}
