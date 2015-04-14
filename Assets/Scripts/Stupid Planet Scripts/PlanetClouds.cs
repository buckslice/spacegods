﻿using UnityEngine;
using System.Collections;

public class PlanetClouds : MonoBehaviour 
{
	
	public Sprite[] PossibleSprites;
	
	private Transform model;
	
	void Start(){
		//model = transform.Find("Model");
		Sprite sprite = PossibleSprites[Random.Range(0,PossibleSprites.Length)];
		//transform.localScale = new Vector3(2f/sprite.bounds.size.x,2f/sprite.bounds.size.y,1);
		transform.Rotate(new Vector3(0,0,Random.value*360));
		transform.GetComponent<SpriteRenderer>().sprite = sprite;
	}
	
	void Update () 
	{
		
	}
}
