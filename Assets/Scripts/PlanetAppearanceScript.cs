using UnityEngine;
using System.Collections;

public class PlanetAppearanceScript : MonoBehaviour {

	public Sprite[] PossibleSprites;

	private Transform model;

	void Start(){
		//model = transform.Find("Model");
		transform.GetComponent<SpriteRenderer>().sprite = PossibleSprites[Random.Range(0,PossibleSprites.Length)];
	}

	void Update () {
	
	}
}
