using UnityEngine;
using System.Collections;

public class SingleplayerButtons : MonoBehaviour
{

	void Start(){}
	
	void Update()
	{

		if(Input.GetKey("escape"))
		{
			
			GoMainMenu();
			
		}
	
	}

	public void GoMainMenu()
	{

		Application.LoadLevel(0);

	}

	public void StartGame()
	{

		Application.LoadLevel(5);

	}

}
