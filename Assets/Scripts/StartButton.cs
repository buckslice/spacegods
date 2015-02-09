using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StartButton : MonoBehaviour 
{
	public Button button;
	public string scene_to_load;

	// use this for initialization
	void Start () 
	{
		button.onClick.AddListener (() => whenClicked ());
	}

	void whenClicked()
	{
		if (scene_to_load.Equals("ExitGame"))
			Application.Quit ();
		else
			Application.LoadLevel (scene_to_load);
	}

	// update is called once per frame
	void Update () 
	{
	
	}
}
