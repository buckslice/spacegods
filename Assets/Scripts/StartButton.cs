using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StartButton : MonoBehaviour {
	public Button button;
	// Use this for initialization
	void Start () {
		button.onClick.AddListener (() => whenClicked ());
	}
	void whenClicked(){
		Application.LoadLevel ("Main");
	}
	// Update is called once per frame
	void Update () {
	
	}
}
