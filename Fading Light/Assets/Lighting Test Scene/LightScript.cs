using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LightScript : MonoBehaviour {

	private List<RaycastHit2D> rayHits;
	private RaycastHit2D[] rayHitsArray;

	public float coneWidth;
	public float coneDistance;
	public Color color;

	private MeshRenderer mRenderer;
	private MeshFilter filter;

	// Use this for initialization
	void Start () {

		transform.position = new Vector3 (transform.position.x,transform.position.y,-1f);
		rayHits = new List<RaycastHit2D> ();

		mRenderer = gameObject.AddComponent<MeshRenderer>();
		filter = gameObject.AddComponent<MeshFilter>();
		mRenderer.material.shader = Shader.Find ("Custom/LightShader");
		mRenderer.material.color = color;
		mRenderer.sortingLayerName = "Light";

		coneWidth = 359f;
		coneDistance = 4f;
	}
	
	// Update is called once per frame
	void Update () {

		castRays2(transform.position, coneDistance);

		drawLight2(rayHitsArray);

		//debugLines ();
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
	
	//converts an array or world points to local point centered around origin
	void toLocalSpace(Vector2[] worldPoints)
	{
		for(int n = 0; n < worldPoints.Length; n++)
		{
			worldPoints[n] = transform.InverseTransformPoint(worldPoints[n]);
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

				Debug.DrawRay(rayHitsArray[n].centroid, direction, Color.white);
		}
	}
	/////////////////////////////////////////////////////////////////////////
	/////////////////////////////////////////////////////////////////////////
	/// EXPERIMENTAL//////////////////////

	//COUNTER CLOCKWISE
	float distanceBetween(float start, float end)
	{
		float barriar = 360f;
		float ans = 0f;
		if(start < end)
		{
			ans = end - start;
		}
		else
		{
			ans = (barriar - start) + end;
		}
		return ans;
	}

	void drawLight2(RaycastHit2D[] rayHitsArray)
	{
		List<Vector2> thePoints = new List<Vector2> ();
		if(coneWidth < 350f)//not a circle
		{
			thePoints.Add(transform.position);
		}
		thePoints.Add(rayHitsArray[0].point);//assumes array will not be empty
		for(int n = 0; n < rayHitsArray.Length-1; n ++)
		{
			if((!rayHitsArray[n].collider || !rayHitsArray[n+1].collider)||(rayHitsArray[n].collider != rayHitsArray[n+1].collider))
			{
				float smooth = 5f;
				float total = distanceBetween(rayHitsArray[n+1].fraction, rayHitsArray[n].fraction);
				int nw = Mathf.FloorToInt(total/smooth);
				float newSmooth = total/nw;

				for(int count = 1; count < nw; count ++)
				{
					float ang = rayHitsArray[n].fraction-count*newSmooth;
					thePoints.Add(new Vector2(transform.position.x + coneDistance*Mathf.Cos(Mathf.Deg2Rad*ang),
					                          transform.position.y + coneDistance*Mathf.Sin(Mathf.Deg2Rad*ang)));
				}
			}
			if(rayHitsArray[n].point != rayHitsArray[n+1].point)
			thePoints.Add(rayHitsArray[n+1].point);
		}
		Vector2[] p = thePoints.ToArray();
		toLocalSpace(p);
		filter.mesh = createMesh(p);
	}
	
	//angles goes COUNTER-CLOCKWISE from start to end, angles should be normalized before using
	void castRays2(Vector2 origin, float distance)
	{
		float angle = transform.rotation.eulerAngles.z;
		//transform.TransformPoint() 

		//get all polygonCollider2d corners on screen (stored in DataBase)
		//draw a ray toward the angles and all corners withing the angles
		rayHits.Clear();//clear previous frame's rayHits
		
		float angleStart = normalizeAngle (angle - coneWidth/2);
		float angleEnd = normalizeAngle (angle + coneWidth/2);
		
		Vector2 directionStart = convertToPoint(angleStart);
		Vector2 directionEnd = convertToPoint(angleEnd);
		
		RaycastHit2D a = Physics2D.Raycast(origin, directionStart, distance, (1 << LayerMask.NameToLayer("SolidWallLayer")));
		RaycastHit2D b = Physics2D.Raycast(origin, directionEnd, distance, (1 << LayerMask.NameToLayer("SolidWallLayer")));
		RaycastHit2D mid = Physics2D.Raycast(origin, convertToPoint(angle), distance, (1 << LayerMask.NameToLayer("SolidWallLayer")));
		a.centroid = origin;
		b.centroid = origin;
		mid.centroid = origin;
		if(!a.collider) a.point = (directionStart.normalized*distance) + origin;
		if(!b.collider) b.point = (directionEnd.normalized*distance) + origin;
		if(!mid.collider) mid.point = (convertToPoint(angle).normalized*distance) + origin;
		a.fraction = angleStart;
		b.fraction = angleEnd;
		mid.fraction = angle;//USE FRACTION TO STORE ANGLE
		a.distance = distance;
		b.distance = distance;
		mid.distance = distance;
		a.normal = directionStart.normalized;
		b.normal = directionEnd.normalized;
		mid.normal = convertToPoint(angle).normalized;

		if(a.collider) a.point = a.collider.gameObject.GetComponent<SolidWallScript>().getRayExit(a);
		if(b.collider) b.point = b.collider.gameObject.GetComponent<SolidWallScript>().getRayExit(b);
		if(mid.collider) mid.point = mid.collider.gameObject.GetComponent<SolidWallScript>().getRayExit(mid);

		rayHits.Add(a);
		rayHits.Add(b);
		rayHits.Add(mid);


		for(int n = 0; n < DataBase.corners.Count; n ++)
		{
			for(int i = 0; i < DataBase.corners[n].Length; i ++)
			{
				Vector2 direction = new Vector2(DataBase.corners[n][i].x - origin.x, DataBase.corners[n][i].y - origin.y);
				float dirAngle = convertToAngle(direction);
				
				//if the corner is withing the light cone, cast a ray toward it
				if(isBetween(angleStart, angleEnd, dirAngle))
				{
					//cast 2 rays offset by 0.0001f so the ray will keep going if it hits a corner
					Vector2 offSet1 = convertToPoint(dirAngle + 0.02f);
					Vector2 offSet2 = convertToPoint(dirAngle - 0.02f);
					RaycastHit2D c = Physics2D.Raycast(origin, offSet1.normalized, distance, (1 << LayerMask.NameToLayer("SolidWallLayer")));
					RaycastHit2D d = Physics2D.Raycast(origin, offSet2.normalized, distance, (1 << LayerMask.NameToLayer("SolidWallLayer")));
					c.centroid = origin;
					d.centroid = origin;
					c.fraction = normalizeAngle(dirAngle + 0.02f);
					d.fraction = normalizeAngle(dirAngle - 0.02f);
					c.distance = distance;
					d.distance = distance;
					c.normal = offSet1.normalized;
					d.normal = offSet2.normalized;
					
					
					//if both miss then this thing is out of range
					if(!c.collider && !d.collider)
					{
						//dont bother doing anything
					}
					//if both offsets hit, just use one ray(reduce triangles)
					else if(c.collider == d.collider)
					{
						RaycastHit2D e = Physics2D.Raycast(origin, direction.normalized, distance, (1 << LayerMask.NameToLayer("SolidWallLayer")));
						e.centroid = origin;
						e.fraction = dirAngle;
						e.distance = distance;
						e.normal = direction.normalized;
						if(!e.collider) e.point = direction.normalized*distance + origin;
						if(e.collider) e.point = e.collider.gameObject.GetComponent<SolidWallScript>().getRayExit(e);
						rayHits.Add (e);
					}
					else
					{
						if(!c.collider) c.point = offSet1.normalized*distance + origin;
						if(!d.collider)d.point = offSet2.normalized*distance + origin;

						if(c.collider) c.point = c.collider.gameObject.GetComponent<SolidWallScript>().getRayExit(c);
						if(d.collider) d.point = d.collider.gameObject.GetComponent<SolidWallScript>().getRayExit(d);
						
						rayHits.Add (c);
						rayHits.Add (d);
					}
				}
			}
		}
		rayHitsArray = rayHits.ToArray();
		sortRayHits(rayHitsArray, angleEnd+0.01f);
	}
}
