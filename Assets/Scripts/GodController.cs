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

    //Compare trigger values in previous frame
    private bool oldTrigger;
    private bool newTrigger;

    //catchable
    int catchable;

    // track the original orientation of the model
    private float flipX;
    private bool isFlipped = false; // true when facing right, false when facing left

    private GameObject myPlanet;
    private CircleCollider2D planetCollider;
    private bool releaseButtonFire = true;

    // Use this for initialization
    void Start() {
        catchable = 0;
        oldTrigger = false;
        model = transform.Find("Model");
        myRigidbody = GetComponent<Rigidbody2D>();
        god = GetComponent<God>();
        flipX = model.localScale.x;
        Game.instance.addPlayer(this);
        planetCollider = gameObject.AddComponent<CircleCollider2D>();
        planetCollider.enabled = false;
    }

    // Update is called once per frame
    void Update() {
        // add input force to rigidbody
        // define input axis' under Edit->Project Settings->Input
        // so far just 2 player keyboard input set up, but can later expand to 4+ people with controllers
        dx = Input.GetAxis("Horizontal" + player) * god.acceleration;
        //Making sure players cant press keyboard AND controller for speed boost
        if (dx == 0.0) {
            dx = Input.GetAxis("Horizontal_360_" + player) * god.acceleration;
        }
        dy = Input.GetAxis("Vertical" + player) * god.acceleration;
        if (dy == 0.0) {
            dy = Input.GetAxis("Vertical_360_" + player) * god.acceleration;
        }

        // lerp velocity for more responsive movement
        float rate = .03f * (0.02f) / Time.deltaTime;
        myRigidbody.velocity = Vector2.Lerp(myRigidbody.velocity, new Vector2(dx, dy) * 2f, rate);
        //myRigidbody.AddForce(new Vector2(dx, dy), ForceMode2D.Force);

        // limit velocity to maxSpeed of god
        if (myRigidbody.velocity.sqrMagnitude > god.maxSpeed * god.maxSpeed) {
            myRigidbody.velocity = myRigidbody.velocity.normalized * god.maxSpeed;
        }


        // flips sprite based on last horizontal movement direction
        // as well as the default flip orientation of gods sprite
        if (!Mathf.Approximately(dx, 0f) || !Mathf.Approximately(Input.GetAxis("Horizontal_aim_360_" + player), 0f)) {
            isFlipped = Mathf.Approximately(Input.GetAxis("Horizontal_aim_360_" + player), 0f) ? dx < 0 :
                Input.GetAxis("Horizontal_aim_360_" + player) < 0f;
            int flip = isFlipped ? -1 : 1;
            model.localScale = new Vector3(flipX * flip, model.localScale.y, model.localScale.z);
        }


        if (god.health <= 0) {
            Game.instance.removePlayer(this);
            Destroy(gameObject);
        }

        newTrigger = Input.GetAxis("Fire_360_" + player) < 0.0;
        if (myPlanet != null) { // if your god is holding a planet
            if (Input.GetButtonDown("Fire" + player) || (!oldTrigger && newTrigger)) { //throw planet
                PlanetGravity myGrav = myPlanet.GetComponent<PlanetGravity>();
                myGrav.makeFalse();
                myGrav.makeTrue();
                releaseButtonFire = false;
                Rigidbody2D planetBody = myPlanet.GetComponent<Rigidbody2D>();
                planetBody.simulated = true;  //renable planets physics

                // this throws to left or right but once controllers are added
                // we should throw according to direction of right thumbstick
                Vector2 launch = new Vector2(isFlipped ? -god.throwStrength : god.throwStrength,
                                             Input.GetAxis("Vertical_aim_360_" + player) * god.throwStrength);
                planetBody.velocity = myRigidbody.velocity + launch;

                myRigidbody.mass -= planetBody.mass; // subtract off planets mass
                myPlanet.transform.parent = null;
                planetCollider.enabled = false;
                myPlanet = null;
                myGrav = null;
            } else {    // move the planet in front of god for blocking (should be based of right thumsbtick later too)
                float xHoldDistance = isFlipped ? -2f : 2f;
                float yHoldDistance = Input.GetAxis("Vertical_aim_360_" + player) * 2f;
                Vector3 target = transform.position + Vector3.right * xHoldDistance + Vector3.up * yHoldDistance;
                myPlanet.transform.position = Vector3.Lerp(myPlanet.transform.position, target, .1f);
                planetCollider.center = new Vector2(xHoldDistance, 0f);
            }
        }
        if (Input.GetButtonDown("Fire" + player) || (!oldTrigger && newTrigger)) {
            catchable = 0;
        }

        if (Input.GetButtonUp("Fire" + player) || (oldTrigger && !newTrigger)) {
            releaseButtonFire = true;
        }
        oldTrigger = newTrigger;
        ++catchable;
    }

    void FixedUpdate() {
        // moved movement code out of FixedUpdate since its no longer using AddForce     
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Planet") {
            PlanetGravity planCol = collision.gameObject.GetComponent<PlanetGravity>();
            if (((planCol.catchBool == true || (planCol.catchBool == false && catchable < 30))
                 && ((Input.GetButton("Fire" + player) || Input.GetAxis("Fire_360_" + player) < 0.0) && myPlanet == null))) {  // catch planet if button is down and we dont have one
                if (releaseButtonFire) { // incase you just threw planet and still holding down button you dont want to pick up same one
                    myPlanet = collision.gameObject;
                    Rigidbody2D planetBody = myPlanet.GetComponent<Rigidbody2D>();
                    planetBody.simulated = false;  //disable planets physics
                    myRigidbody.mass += planetBody.mass; // add planets mass to your own
                    myPlanet.transform.parent = transform;
                    planetCollider.enabled = true;
                    planetCollider.radius = myPlanet.GetComponent<CircleCollider2D>().radius;   //set our planetCollider equal to radius of planet
                }
            } else {
                // since were using mainly circle colliders, the first contact point will probably be the only one
                ContactPoint2D first = collision.contacts[0];
                // if either are our planetCollider then we dont take damage
                if (first.collider == planetCollider || first.otherCollider == planetCollider) {
                    AudioManager.instance.playSound("Collision", collision.contacts[0].point, 1f);
                } else {    // otherwise one of our gods colliders have been hit so we take damage
                    AudioManager.instance.playSound("GodHurt", transform.position, 1f);
                    god.health -= 20;
                }
            }
        }
    }



    public Bounds getCameraBounds() {
        return model.renderer.bounds;
    }
}
