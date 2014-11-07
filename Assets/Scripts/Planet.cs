using UnityEngine;
using System.Collections;

public class Planet : MonoBehaviour {

	//Class wide number to keep track of how many planets are in existence
	static public int planetNum = 0;

	public GameObject muhThang;
	private float x = 0f, y = 0f;

	// Use this for initialization
	void Start () {
		//Spawns a planet every second
		InvokeRepeating ("SpawnPlanet", 1f, 1f);
	}
	
	// Update is called once per frame
	void Update () {


	}

	//Spawns a planet
	public void SpawnPlanet()
	{

		//Chooses one of four possible spawn points off screen
		if (planetNum < 20) {

			int selectSide = Random.Range(1, 4);

			switch (selectSide)
			{
			case 1:
				x = -0.1f;
				y = Random.Range(0f, 0.25f);
				break;

			case 2:
				x = 1.1f;
				y = Random.Range(0.75f, 1f);
				break;

			case 3:
				y = -0.1f;
				x = Random.Range(0.75f, 1f);
				break;

			case 4:
				y = 1.1f;
				x = Random.Range(0f, 0.25f);
				break;

			}

			//Uses camera position to find out where off screen is
			Vector3 p = camera.ViewportToWorldPoint(new Vector3(x, y, 10f));
			Instantiate (muhThang, p, Quaternion.identity);
			++planetNum;
		}
	}
}
