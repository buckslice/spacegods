using UnityEngine;
using System.Collections;


[RequireComponent(typeof(God))]
public class GodController : MonoBehaviour {
    // player number, used for input
    public int player = 0;

    private Transform model;
    private Rigidbody2D myRigidbody;
    private God god;

    // x and y velocity
    private float dx;
    private float dy;

    // track the original orientation of the model
    private float flipX;

    // Use this for initialization
    void Start() {
        model = transform.Find("Model");
        myRigidbody = GetComponent<Rigidbody2D>();
        god = GetComponent<God>();
        flipX = model.localScale.x;
        Game.instance.addPlayer(this);
    }

    // Update is called once per frame
    void Update() {
        // flips sprite based on last horizontal movement direction
        // as well as the default flip orientation of gods sprite
        if (!Mathf.Approximately(dx, 0f)) {
            int flip = dx < 0 ? 1 : -1;
            model.localScale = new Vector3(flipX * flip, model.localScale.y, model.localScale.z);
        }
    }

    void FixedUpdate() {
        // add input force to rigidbody
        // define input axis' under Edit->Project Settings->Input
        // so far just 2 player keyboard input set up, but can later expand to 4+ people with controllers
        dx = Input.GetAxis("Horizontal" + player) * god.acceleration;
        dy = Input.GetAxis("Vertical" + player) * god.acceleration;
        myRigidbody.AddForce(new Vector2(dx, dy), ForceMode2D.Force);

        // limit velocity to maxSpeed of god
        if (myRigidbody.velocity.sqrMagnitude > god.maxSpeed * god.maxSpeed) {
            myRigidbody.velocity = myRigidbody.velocity.normalized * god.maxSpeed;
        }

        // if no inputs then slowly dampen velocity (maybe only do this when hitting spacebar or something)
        if (Mathf.Approximately(dx, 0f) && Mathf.Approximately(dy, 0f)) {
            myRigidbody.velocity = myRigidbody.velocity * god.dampeningFactor;
        }
    }

    public Bounds getCameraBounds() {
        return model.renderer.bounds;
    }
}
