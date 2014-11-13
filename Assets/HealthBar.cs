using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {
	private God god;
	private Vector3 screenPoint;

	// Use this for initialization
	void Start () {
		god = GetComponent<God>();
	}
	
	// Update is called once per frame
	void Update () {
		screenPoint = Camera.main.WorldToScreenPoint(transform.position);
		screenPoint.y = Screen.height - screenPoint.y;
	}

	void OnGUI() {
		//makes healthbar green/yellow/red
		Texture2D texture = new Texture2D(1, 1);
		if (god.health > 50)
			texture.SetPixel (0, 0, Color.green);
		else if (god.health <= 50 && god.health > 20)
			texture.SetPixel (0, 0, Color.yellow);
		else
			texture.SetPixel (0, 0, Color.red);

		texture.Apply();
		GUI.skin.box.normal.background = texture;
		GUI.Box(new Rect(screenPoint.x-50, screenPoint.y+70, god.health, 5), "");
	}
}
