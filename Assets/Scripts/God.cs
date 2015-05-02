﻿using UnityEngine;
using System.Collections;

public enum Gods {
    ZEUS,
    POSEIDON,
    ANUBIS,
    THOR,
    ODIN,
    ATHENA,
    MICHAEL_JORDAN,
    CTHULU,
	HERMES,
	SHIVA,
	SUN_WUKONG
}

public class God : MonoBehaviour {

    public Gods god;
    public float maxHealth;
	private float currentHealth;
    public float maxSpeed;
    public float acceleration;
    public float throwStrength;
    private float startingThrowStrength;
    private float counter;   // counter variable
    public bool special;    // event variable 

    // use this for initialization
    void Start() {
		currentHealth = maxHealth;
        startingThrowStrength = throwStrength;
		counter = 0.0f;
    }

    void Update() {
        counter += Time.deltaTime;
    }
	public void changeHealth(float damage){
		currentHealth -= damage;
	}

	public float getCurrentHealth(){
		return currentHealth;
	}

	public void resetCounter(){
		counter = 0.0f;
	}
	public float getCounter(){
		return counter;
	}

    public float getStartingThrowStrength() {
        return startingThrowStrength;
    }
}
