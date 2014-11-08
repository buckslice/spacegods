using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class God : MonoBehaviour {

    public int health = 100;
    public float maxSpeed = 100f;
    public float acceleration = 10f;

    // .99 is a pretty good value here it seems
    [Range(.9f, 1f)]
    public float dampeningFactor = 0.99f;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        // could update certain gods based on enum type later
    }

    void FixedUpdate() {

    }

}
