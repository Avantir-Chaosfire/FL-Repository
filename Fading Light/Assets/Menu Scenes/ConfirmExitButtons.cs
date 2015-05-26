using UnityEngine;
using System.Collections;

public class ConfirmExitButtons : MonoBehaviour
{
	
	void Start(){}
	
	void Update()
	{
		
		if(Input.GetKey(KeyCode.Escape))
		{
			
			GoMainMenu();
			
		}
		else if(Input.GetKey(KeyCode.Return))
		{
			
			Quit();
			
		}
		
	}

	public void Quit()
	{

		Application.Quit();

	}
	
	public void GoMainMenu()
	{
		
		Application.LoadLevel(0);
		
	}
	
}