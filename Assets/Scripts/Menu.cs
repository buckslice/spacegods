using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour 
{
	public bool isQuit=false;
	public bool isTutorial=false;
	
	void OnMouseUp(){
		if (isQuit) {
			//Quit game
			Application.Quit();
		}
		else if (isTutorial){
			//Go to tutorial page, which hasn't been written yet
		}
		else {
			//Load game
			Application.LoadLevel(1);
		}
	}
	
	void Update(){
		//Quit game if escape key is pressed
		if (Input.GetKey(KeyCode.Escape)) { 
			Application.Quit();
		}
	}
}
