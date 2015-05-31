using UnityEngine;
using System.Collections;

public class TurnTowardsMouse : MonoBehaviour {

	public float turnSpeed;
	// Use this for initialization
	void Start () {
		turnSpeed = 5f;
	}
	
	// Update is called once per frame
	void Update () {
		turn ();
	}

	float normalizeAngle(float angle)//return a angle between 0 and 359
	{
		float a = angle % 360;
		if (a < 0)
			a += 360;
		return a;
	}

	void turn()
	{
		Vector2 mouseLocation = Camera.main.ScreenToWorldPoint (Input.mousePosition);


		float angle = normalizeAngle(transform.rotation.eulerAngles.z);
		float newAngle = normalizeAngle(Mathf.Atan2(mouseLocation.y - transform.position.y, mouseLocation.x - transform.position.x) * Mathf.Rad2Deg);

		float turnDegree = newAngle - angle;//how far to turn to match newAngle
		float turnRate = turnSpeed;

		if(turnRate > Mathf.Abs(turnDegree))//will over turn
		{
			turnRate = Mathf.Abs(turnDegree);
		}
		
		if(turnDegree < 180 && turnDegree > 0)
		{
			setRotation(angle+turnRate);
		}
		else if(turnDegree >= 180)
		{
			setRotation(angle-turnRate);
		}
		else if(turnDegree < 0 && turnDegree >= -180)
		{
			setRotation(angle-turnRate);
		}
		else if(turnDegree < -180)
		{
			setRotation(angle+turnRate);
		}
	}

	void setRotation(float z)
	{
		transform.Rotate(0,0,z - transform.rotation.eulerAngles.z);
	}


}
