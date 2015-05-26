using UnityEngine;
using System.Collections;

public class OptionsButtons : MonoBehaviour
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
	
}