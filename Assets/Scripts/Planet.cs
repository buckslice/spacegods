using UnityEngine;
using System.Collections;

public class Planet : MonoBehaviour {
	
	public GameObject muhThang;
	static public int planetNum = 0;
	private float x = 0f, y = 0f;

	// Use this for initialization
	void Start () {
		InvokeRepeating ("SpawnPlanet", 1f, 1f);
	}
	
	// Update is called once per frame
	void Update () {


	}


	public void SpawnPlanet()
	{

		if (planetNum < 20) {

			int selectSide = Random.Range(1, 4);

			switch (selectSide)
			{
			case 1:
				x = -0.1f;
				y = Random.Range(0f, 1f);
				break;

			case 2:
				x = 1.1f;
				y = Random.Range(0, 1f);
				break;

			case 3:
				y = -0.1f;
				x = Random.Range(0f, 1f);
				break;

			case 4:
				y = 1.1f;
				x = Random.Range(0f, 1f);
				break;

			}

			Vector3 p = camera.ViewportToWorldPoint(new Vector3(x, y, 10f));
			Instantiate (muhThang, p, Quaternion.identity);
			++planetNum;
		}
	}
}
