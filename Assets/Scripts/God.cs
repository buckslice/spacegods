using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class God : MonoBehaviour {

    public int health = 100;
    private int maxHealth;
    public float maxSpeed = 100f;
    public float acceleration = 10f;
    public float throwStrength = 20f;

    // Use this for initialization
    void Start() {
        maxHealth = health;
    }

    public int getMaxHealth() {
        return maxHealth;
    }

    // Update is called once per frame
    void Update() {
        // could update certain gods based on enum type later
    }

    void FixedUpdate() {

    }

}
