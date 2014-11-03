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

    private Transform myTransform;
    private Rigidbody2D myRigidbody;

    // Use this for initialization
    void Start() {
        myTransform = transform;
        myRigidbody = rigidbody2D;
        Game.instance.addGodToList(this);
    }

    // Update is called once per frame
    void Update() {
        // could update certain gods based on enum type later
    }

    void FixedUpdate() {

    }

    public Bounds getBounds() {
        return myRigidbody.collider2D.bounds;
    }
}
