using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SolidWallScript : MonoBehaviour {

	PolygonCollider2D col;
	// Use this for initialization
	void Start () {
		col = GetComponent<PolygonCollider2D>();
		SpriteRenderer renderer = GetComponent<SpriteRenderer> ();
		// Set up game object with mesh;
		//Rigidbody2D body = gameObject.AddComponent<Rigidbody2D>();
		//body.isKinematic = true;
		//gameObject.AddComponent(typeof(MeshRenderer));
		//MeshFilter filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
		//filter.mesh = createMesh(col.points);
		gameObject.layer = LayerMask.NameToLayer("SolidWallLayer");//(SolidWallLayer)
		renderer.sortingLayerName = "Overhead";// and .sortingOrder = blah;
		//MeshRenderer r = GetComponent<MeshRenderer> ();

		transform.position = new Vector3(transform.position.x, transform.position.y, DataBase.objectZ);
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
