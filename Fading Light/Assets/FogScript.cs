using UnityEngine;
using System.Collections;

public class FogScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// Set up game object with mesh;
		gameObject.layer = LayerMask.NameToLayer("FogLayer");
		gameObject.AddComponent(typeof(MeshRenderer));
		MeshFilter filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
		MeshRenderer r = GetComponent<MeshRenderer>();
		r.material.shader = Shader.Find ("Custom/FogShader");
		r.material.SetTexture("_MainTex", (Texture)Resources.Load("black"));
	}
	
	// Update is called once per frame
	void Update () {
		drawFog ();
		transform.position = new Vector3 (Camera.main.transform.position.x,
		                                  Camera.main.transform.position.y, transform.position.z);
	}

	//fog is just a rectangle mesh that covers up to what the main camera displays
	void drawFog()
	{
		Vector2[] corners = new Vector2[4];
		corners[0] = new Vector2(Screen.width / 2.0f / Camera.main.orthographicSize/-2, Screen.height / 2.0f / Camera.main.orthographicSize/2);
		corners[1] = new Vector2(Screen.width / 2.0f / Camera.main.orthographicSize/2, Screen.height / 2.0f / Camera.main.orthographicSize/2);
		corners[2] = new Vector2(Screen.width / 2.0f / Camera.main.orthographicSize/2, Screen.height / 2.0f / Camera.main.orthographicSize/-2);
		corners[3] = new Vector2(Screen.width / 2.0f / Camera.main.orthographicSize/-2, Screen.height / 2.0f / Camera.main.orthographicSize/-2);
		MeshFilter filter = GetComponent<MeshFilter> ();
		filter.mesh = createMesh(corners);
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
