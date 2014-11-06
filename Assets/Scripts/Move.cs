using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {

	float x, y;
	// Use this for initialization
	void Start () {

		if (transform.position.x < 0 && transform.position.y < 0) {
						x = Random.Range(1.0f, 10.0f);
						y = Random.Range (1.0f, 10.0f);
				} else if (transform.position.x > 0 && transform.position.y < 0) {
						x = Random.Range(-10.0f, -1.0f);
						y = Random.Range (1.0f, 10.0f);
				} else if (transform.position.x < 0 && transform.position.y > 0) {
						x = Random.Range(1.0f, 10.0f);
						y = Random.Range (-10.0f, -1.0f);
				} else {
						x = Random.Range(-10.0f, -1.0f);
						y = Random.Range (-10.0f, -1.0f);
				}

		InvokeRepeating ("DeletePlanet", 10.0f, 5f);
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (new Vector3 (x, y, 0f) * Time.deltaTime);


	}

	public void DeletePlanet()
	{
		Destroy (gameObject);
		--Planet.planetNum;
		}
}
