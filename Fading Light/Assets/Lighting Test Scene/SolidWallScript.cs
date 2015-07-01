using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (SpriteRenderer))]
[RequireComponent (typeof (PolygonCollider2D))]
public class SolidWallScript : MonoBehaviour {

	PolygonCollider2D col;
	SpriteRenderer sRenderer;
	Vector2[] worldPoints;
	// Use this for initialization
	void Start () {
		col = GetComponent<PolygonCollider2D>();
		sRenderer = GetComponent<SpriteRenderer> ();

		gameObject.layer = LayerMask.NameToLayer("SolidWallLayer");//(SolidWallLayer)
		sRenderer.sortingLayerName = "Doodads";// and .sortingOrder = blah;

		transform.position = new Vector3(transform.position.x, transform.position.y, DataBase.objectZ);

		worldPoints = col.GetPath(0);
		toWorld(worldPoints);
	}
	
	// Update is called once per frame
	void Update () {

	}

	//remove this collider from the list of visible collider in database
	void OnBecameInvisible() {

		DataBase.corners.Remove(worldPoints);
	}

	//add this collider to the list of visible collider is database
	void OnBecameVisible() {

		DataBase.corners.Add(worldPoints);
	}

	Vector2 convertToPoint(float a)
	{
		return new Vector2 (Mathf.Cos (Mathf.Deg2Rad * a), Mathf.Sin (Mathf.Deg2Rad * a));
	}

	void toWorld(Vector2[] local)
	{
		for(int n = 0; n < local.Length; n ++)
		{
			local[n] = transform.TransformPoint(local[n]);
		}
	}

	float smaller(float a, float b)
	{
		if(a < b)
		{
			return a;
		}
		return b;
	}
	float bigger(float a, float b)
	{
		if(a > b)
		{
			return a;
		}
		return b;
	}

	//assumes hit has been modified so that centroid = origin, fraction = angle, distance = maxDistance
	public Vector2 getRayExit(RaycastHit2D hit)
	{
		Vector2 secNearest = hit.point;

		float r_dx = hit.normal.x;
		float r_dy = hit.normal.y;
		float r_px = hit.centroid.x;
		float r_py = hit.centroid.y;

		float secNDistance = Mathf.Infinity;
		float origDistance = (hit.point.x-r_px)/r_dx + 0.02f;//hit.point.x = r_px + r_dx * T1;

		//Debug.Log ("(" + hit.normal.x +"," + hit.normal.y +")");
		//Debug.Log ("(" + r_dx +"," + r_dy +")");

		for(int n = 0; n < worldPoints.Length; n ++)
		{
			int nxtP = n+1;
			if(n+1 >= worldPoints.Length) nxtP = 0;

			float s_dx = worldPoints[nxtP].x-worldPoints[n].x;
			float s_dy = worldPoints[nxtP].y-worldPoints[n].y;
			float s_px = worldPoints[n].x;
			float s_py = worldPoints[n].y;
			if((s_dy/s_dx) != (r_dy/r_dx))//not parrellel
			{
				// Solve for T2!
				float T2 = (r_dx * (s_py - r_py) + r_dy * (r_px - s_px)) / (s_dx * r_dy - s_dy * r_dx);
				
				// Plug the value of T2 to get T1
				float T1 = (s_px+s_dx*T2-r_px)/r_dx;
				if(T1 >= 0f && T1 <= hit.distance && T2 >= -0.01f && T2 <= 1.01f)
				{
					//Debug.DrawRay(new Vector2(r_px+r_dx*T1,r_py+r_dy*T1),new Vector2(0.01f,0f),Color.green);
					if(T1 < secNDistance && T1 > origDistance)
					{
						secNDistance = T1;
						secNearest = new Vector2(r_px+r_dx*T1,r_py+r_dy*T1);
					}
				}
			}
		}
		return secNearest;
	}


	//DEBUGGGING purposes//
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

}
