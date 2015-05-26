using UnityEngine;
using System.Collections;

public class MenuButtons : MonoBehaviour {

	void Start(){}
	void Update()
	{

		if(Input.GetKey("escape"))
		{

			GoConfirmExit();

		}

	}

	public void GoSingleplayer()
	{

		Application.LoadLevel(1);

	}

	public void GoMultiplayer()
	{

		Application.LoadLevel(2);

	}

	public void GoOptions()
	{

		Application.LoadLevel(3);

	}

	public void GoConfirmExit()
	{

		Application.LoadLevel(4);

	}

}
