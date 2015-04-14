using UnityEngine;
using System.Collections;

public class MetalPlanet : Planet {
	private float MIN_MASS = 0.5f, MAX_MASS = 1.5f;
	private float MIN_SIZE = 0.75f, MAX_SIZE = 2.25f;


	// Use this for initialization
	void Start() {
		float size = Random.Range(MIN_SIZE, MAX_SIZE);
		float mass = Random.Range (MIN_MASS, MAX_MASS);
		transform.localScale = Vector3.one * size;
		GetComponent<Rigidbody2D>().mass = mass;
	}
}
