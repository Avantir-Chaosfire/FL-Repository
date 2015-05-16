using UnityEngine;
using System.Collections;

public class LightScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		float angle1 = normalizeAngle (transform.rotation.eulerAngles.z - 60);
		float angle2 = normalizeAngle (transform.rotation.eulerAngles.z + 60);

		drawRays(transform.position, angle1, angle2, 7f);
	}

	//return a angle between 0 and 359
	float normalizeAngle(float angle)
	{
		float a = angle % 360;
		if (a < 0)
			a += 360;
		return a;
	}

	//return true if angle a is between angle start and end (counter-clockwise from start to end)
	bool isBetween(float start, float end, float a)
	{
		if (start < end)
			return a > start && a < end;
		else
			return !(a < start && a > end); 
	}

	//returns the world points equvalent of the local point in polygoncollider2d
	Vector2 toWorldSpace(PolygonCollider2D c, int index)
	{
		Vector2 temp = c.transform.position;
		return temp + c.points[index];
	}

	//angles goes COUNTER-CLOCKWISE from start to end
	void drawRays(Vector2 origin, float angleStart, float angleEnd, float distance)
	{
		//get all collider2d corners on screen
		//draw a ray toward the angles and all corners withing the angles
		//angleStart = normalizeAngle (angleStart);
		//angleEnd = normalizeAngle (angleEnd);

		Vector2 angle1 = new Vector2 (Mathf.Cos (Mathf.Deg2Rad * angleStart), Mathf.Sin (Mathf.Deg2Rad * angleStart));
		Vector2 angle2 = new Vector2 (Mathf.Cos (Mathf.Deg2Rad * angleEnd), Mathf.Sin (Mathf.Deg2Rad * angleEnd));

		Physics2D.Raycast(origin, angle1, distance);
		Physics2D.Raycast(origin, angle2, distance);

		Debug.DrawRay(origin, angle1.normalized*distance, Color.green);
		Debug.DrawRay(origin, angle2.normalized*distance, Color.green);

		for(int n = 0; n < DataBase.corners.Count; n ++)
		{
			for(int i = 0; i < DataBase.corners[n].points.Length; i ++)
			{
				Vector2 temp = toWorldSpace(DataBase.corners[n], i);
				Vector2 direction = new Vector2(temp.x - origin.x,
				                                temp.y - origin.y);

				float d = normalizeAngle(Mathf.Atan2(direction.y, direction.x)*Mathf.Rad2Deg);

				//if the corner is withing the angles, cast a ray toward it
				if(isBetween(angleStart, angleEnd, d))
				{
					Physics2D.Raycast(origin, direction, distance);
					Debug.DrawRay(origin, direction.normalized*distance, Color.green);
				}
				//REMEMBER TO CAST ANOTHER RAY AFTER THIS SO IF ITS THE EDGE THE LIGHT WILL KEEP GOING
				//!!!!!!!!!!!!!! REMEMBER TO DO ITTTT!!!!!!!!!
			}
		}
	}

	void helpDebug()
	{

	}
}
