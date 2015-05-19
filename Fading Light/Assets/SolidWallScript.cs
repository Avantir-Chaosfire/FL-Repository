using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SolidWallScript : MonoBehaviour {

	PolygonCollider2D col;
	// Use this for initialization
	void Start () {
		col = GetComponent<PolygonCollider2D>();

		// Set up game object with mesh;
		gameObject.AddComponent(typeof(MeshRenderer));
		MeshFilter filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
		filter.mesh = createMesh(col.points);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//remove this collider from the list of visible collider in database
	void OnBecameInvisible() {

		DataBase.corners.Remove(col);
	}

	//add this collider to the list of visible collider is database
	void OnBecameVisible() {

		DataBase.corners.Add(col);
	}

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
