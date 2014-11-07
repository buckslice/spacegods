using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {

	private float x, y, distance;
	// Use this for initialization
	void Start () {

		//checks what quadrant the planet spawned in
		if (transform.position.x < 0 && transform.position.y < 0) {
						x = Random.Range(1.0f, 15.0f);
			y = 0.0f; //Random.Range (1.0f, 2.0f);
				} else if (transform.position.x > 0 && transform.position.y < 0) {
			x = 0.0f; //Random.Range(-2.0f, -1.0f);
			y = Random.Range (1.0f, 15.0f);
				} else if (transform.position.x < 0 && transform.position.y > 0) {
			x = 0.0f; //Random.Range(1.0f, 2.0f);
			y = Random.Range (-15.0f, -1.0f);
				} else {
			x = Random.Range(-15.0f, -1.0f);
			y = 0.0f; //Random.Range (-2.0f, -1.0f);
				}
		// Deletes planet after 10 seconds

	}
	
	// Update is called once per frame
	void Update () {
		distance = Mathf.Sqrt ((transform.position.x * transform.position.x) + (transform.position.y * transform.position.y));
		if (distance <= 2.5f || transform.position.x <= -100f || transform.position.x >= 100f
		    || transform.position.y <= -100f || transform.position.y >= 100f)
						DeletePlanet ();
		updateVel (ref x, ref y);
		transform.Translate (new Vector3 (x, y, 0f) * Time.deltaTime);


	}

	public void DeletePlanet()
	{
		Destroy (gameObject);
		--Planet.planetNum;
		}

	public void updateVel(ref float x, ref float y)
	{

		float gravConst = 1f;
		float sunMass = 50f, planetMass = 10f;
		float force, acc;
		//float xFrac, yFrac, deltaMomentum, deltaVel;

		float dist = Mathf.Sqrt ((transform.position.x * transform.position.x) + (transform.position.y * transform.position.y));

		if (dist == 0)
						dist = .0000000000001f;

		force = gravConst * ((sunMass * planetMass) / Mathf.Pow (dist, 2f));

		/*
		xFrac = transform.position.x / dist * -1f;
		yFrac = transform.position.y / dist * -1f;

		deltaMomentum = force / planetMass;
		deltaVel = deltaMomentum / planetMass;

		x += xFrac * deltaVel;
		y += yFrac * deltaVel;
		*/

		acc = force / planetMass;
		x += transform.position.x * (acc / dist) * -1f;
		y += transform.position.y * (acc / dist) * -1f;




	}
}
