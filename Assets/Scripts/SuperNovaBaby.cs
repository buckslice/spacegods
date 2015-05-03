﻿using UnityEngine;
using System.Collections;

public class SuperNovaBaby : MonoBehaviour 
{
    public float timeUntilShitGetsReal = 60f;
    private ParticleSystem system;
    private Transform sprite;
    private CircleCollider2D circCol;
    private float growth;
    // use this for initialization
    void Start() 
	{
		growth = 0f;
        circCol = GetComponent<CircleCollider2D>();
        system = GetComponent<ParticleSystem>();
        sprite = transform.Find("sprite").transform;
		circCol.radius = 3f;
    }

    // Update is called once per frame
    void Update() 
	{
		SuperNova ();
		sprite.localScale = Vector3.one * (circCol.radius / 3f);
    }

	private void SuperNova()
	{
		sprite.Rotate(0, 0, Time.deltaTime * 5f);
		growth += Time.deltaTime;
		float rate = Mathf.Pow(1.1f, growth - timeUntilShitGetsReal);
		if (rate > 1.1f) 
		{
			circCol.radius += rate * Time.deltaTime;
			system.startSpeed += rate * Time.deltaTime;
			system.startSize += rate * 2f * Time.deltaTime;
		}
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.tag == "Planet") 
		{
			CircleCollider2D planetCol = col.gameObject.GetComponent<CircleCollider2D>();
			circCol.radius += planetCol.radius;
			system.startSpeed = 1f + circCol.radius / 3f;
			system.startSize = 6f + circCol.radius / 1.5f;
		}
	}
}
