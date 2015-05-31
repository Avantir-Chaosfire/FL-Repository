using UnityEngine;
using System.Collections;

public class Final : MonoBehaviour {

	// Use this for initialization
	void Start () {

		// Set up game object with mesh;
		MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
		MeshFilter filter = gameObject.AddComponent<MeshFilter>();
		renderer.material.shader = Shader.Find ("Custom/FinalPass");
		renderer.material.color = DataBase.fogColor;
		renderer.sortingLayerName = "Fog";
	}
	
	// Update is called once per frame
	void Update () {
		drawFog ();
		transform.position = new Vector3 (Camera.main.transform.position.x,
		                                  Camera.main.transform.position.y, DataBase.fogZ);
	}
	
	//fog is just a rectangle mesh that covers up to what the main camera displays
	void drawFog()
	{
		Camera c = Camera.main;
		Vector2[] corners = new Vector2[4];
		corners[0] = worldToLocal(c.ScreenToWorldPoint(new Vector2(0, c.pixelHeight)));
		corners[1] = worldToLocal(c.ScreenToWorldPoint(new Vector2(c.pixelWidth, c.pixelHeight)));
		corners[2] = worldToLocal(c.ScreenToWorldPoint(new Vector2(c.pixelWidth, 0)));
		corners[3] = worldToLocal(c.ScreenToWorldPoint(new Vector2(0, 0)));
		MeshFilter filter = GetComponent<MeshFilter> ();
		filter.mesh = createMesh(corners);
	}

	//using camera.main as the local point
	Vector2 worldToLocal(Vector2 world)
	{
		return new Vector2(world.x - Camera.main.transform.position.x,
		                   world.y - Camera.main.transform.position.y);
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
