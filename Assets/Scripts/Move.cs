using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {

	private float x, y, distance;

	// Use this for initialization
	void Start () {

		//checks what quadrant the planet spawned in and gives it appropriate velocity

		//Bottom left
		if (transform.position.x < 0 && transform.position.y < 0) {
						x = Random.Range(5.0f, 15.0f);
						y = -6.0f;

				//Bottom Right
				} else if (transform.position.x > 0 && transform.position.y < 0) {
						x = 6.0f;
						y = Random.Range (5.0f, 15.0f);

				//Top left
				} else if (transform.position.x < 0 && transform.position.y > 0) {
						x = -6.0f;
						y = Random.Range (-15.0f, -5.0f);

				//Top Right
				} else {
						x = Random.Range(-15.0f, -5.0f);
						y = 6.0f;
				}

	}
	
	// Update is called once per frame
	void Update () {
		//Distance from sun
		distance = Mathf.Sqrt ((transform.position.x * transform.position.x) + (transform.position.y * transform.position.y));

		//Deletes planet if it falls into sun or goes too far off screen
		if (distance <= 2.5f || transform.position.x <= -100f || transform.position.x >= 100f
		    || transform.position.y <= -100f || transform.position.y >= 100f)
						DeletePlanet ();

		//Updates velocity and moves planet
		updateVel (ref x, ref y, distance);
		transform.Translate (new Vector3 (x, y, 0f) * Time.deltaTime);


	}

	//deletes planet and decrements class wide count
	public void DeletePlanet()
	{
		Destroy (gameObject);
		--Planet.planetNum;
		}

	//Uses newtons law to move velocity relative to sun
	public void updateVel(ref float x, ref float y, float dist)
	{

		float gravConst = 1f;
		float sunMass = 50f, planetMass = 10f;
		float force, acc;

		//avoids dividing by zero
		if (dist == 0)
			dist = .0000000000001f;

		//Newtons law
		force = gravConst * ((sunMass * planetMass) / Mathf.Pow (dist, 2f));

		acc = force / planetMass;
		x += transform.position.x * (acc / dist) * -1f;
		y += transform.position.y * (acc / dist) * -1f;




	}
}
