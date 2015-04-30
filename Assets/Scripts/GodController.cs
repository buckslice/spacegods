﻿using UnityEngine;
using System.Collections;

public class GodController : MonoBehaviour {
    // player number, used for input
    public int player = 0;

    private God god;
    private Transform model;
    private Rigidbody2D myRigidbody;
    private Planet myPlanet;
    private CircleCollider2D planetCollider;
    private SpriteRenderer sr;

    // compare trigger values in previous frame
    private bool oldTrigger = false;
    private bool newTrigger = false;
    private bool releaseButtonFire = true;
    private float timeSinceTryCatch;

    // track the original orientation of the model for turning
    private float flipX;
    private float modelPosX;
    private bool isFlipped = false; // true when facing right; false when facing left

    public bool freezeInputs = true;
    private bool usingJoysticks = true;

    private float invincible;

    // use this for initialization
    void Start() {
		initializeVariables ();
    }

    // update is called once per frame
    void Update() {
		checkForDeath ();
		handleVelocityAndOrientation ();
		handleThrow ();
		updateVariables ();
		handleGodPassives ();
    }

    void OnCollisionEnter2D(Collision2D collision) {
		handleGodCollision (collision);
		handlePlanetCollision (collision);
    }

    void OnTriggerStay2D(Collider2D col) {
		handleSunDamage (col);
		handlePlanetCatch (col);
    }

	private void initializeVariables(){
		Game.instance.addPlayer(this);
		model = transform.Find("Model");
		myRigidbody = GetComponent<Rigidbody2D>();
		god = GetComponent<God>();
		flipX = model.localScale.x;
		modelPosX = model.localPosition.x;
		planetCollider = gameObject.AddComponent<CircleCollider2D>();
		planetCollider.enabled = false;
		sr = model.GetComponent<SpriteRenderer>();
		
		usingJoysticks = true;
		string[] joysticks = Input.GetJoystickNames();
		if (joysticks.Length == 0 || (joysticks.Length == 1 && joysticks[0] == "")) {
			usingJoysticks = false;
		}
		
		if (!usingJoysticks && player > 2) {
			// temporary fix until we implement more keyboard stuff
			player = 1;
		}
	}
    private void holdPlanet(Planet planet) {
        myPlanet = planet;
        myPlanet.lastHolder = this;
        myPlanet.state = PlanetState.HELD;
        myPlanet.rb.simulated = false;  // disable planets physics
        myRigidbody.mass += myPlanet.rb.mass; // add planets mass to your own
        myPlanet.transform.parent = transform;
        planetCollider.radius = myPlanet.getRadius();   // set our planetCollider equal to radius of planet
        planetCollider.enabled = true;
    }

    private void throwPlanet(Vector2 aim) {
        releaseButtonFire = false;
        myPlanet.state = PlanetState.THROWN;
        myPlanet.transform.parent = null;
        myPlanet.rb.simulated = true;  //reenable planets physics
        myPlanet.rb.velocity = myRigidbody.velocity + aim * god.throwStrength;
        myRigidbody.mass -= myPlanet.rb.mass; // subtract off planets mass
        planetCollider.enabled = false;
        myPlanet = null;
    }

	private void handleThrow(){
		newTrigger = Input.GetAxis("Fire_360_" + player) < 0.0;
		if (myPlanet) {     // if your god is holding a planet
			if (myPlanet.getHealth() > 0) {
				bool fireInput = usingJoysticks ? !oldTrigger && newTrigger : Input.GetButtonDown("Fire" + player);
				
				float xAim = usingJoysticks ? Input.GetAxis("Horizontal_aim_360_" + player) : isFlipped ? -1f : 1f;
				float yAim = usingJoysticks ? Input.GetAxis("Vertical_aim_360_" + player) : 0f;
				Vector2 aim = new Vector2(xAim, yAim).normalized;
				
				if (fireInput) {    // throw planet
					throwPlanet(aim);
				} else {    // move planet where aiming for blocking
					float holdDistance = 3.5f;
					Vector2 holdPos = aim * holdDistance;
					Vector3 target = transform.position + new Vector3(holdPos.x, holdPos.y, 0);
					myPlanet.transform.position = Vector3.Lerp(myPlanet.transform.position, target, 6f * Time.deltaTime);
					planetCollider.offset = holdPos;
					
				}
			} else {    //planet died before it was thrown
				myRigidbody.mass -= myPlanet.rb.mass;
				planetCollider.enabled = false;
				Destroy(myPlanet.gameObject);
				myPlanet = null;
				--PlanetSpawner.planetNum;
			}
		}
	}

	private void updateVariables(){
		timeSinceTryCatch = (usingJoysticks ? !oldTrigger && newTrigger : Input.GetButtonDown("Fire" + player)) ? 0 : timeSinceTryCatch;
		releaseButtonFire = (usingJoysticks ? oldTrigger && !newTrigger : Input.GetButtonUp("Fire" + player)) ? true : releaseButtonFire;
		oldTrigger = newTrigger;
		timeSinceTryCatch += Time.deltaTime;
		invincible += Time.deltaTime;
	}

	private void checkForDeath(){
		if (god.getCurrentHealth() <= 0) {
			Game.instance.removePlayer(this);
			Destroy(gameObject);
		}
	}
	private void handleVelocityAndOrientation(){
		// making sure players cant press keyboard AND controller for speed boost
		float dx, dy;
		if (usingJoysticks) {
			dx = Input.GetAxis("Horizontal_360_" + player);
			dy = Input.GetAxis("Vertical_360_" + player);
		} else {
			dx = Input.GetAxis("Horizontal" + player);
			dy = Input.GetAxis("Vertical" + player);
		}
		
		if (freezeInputs) {
			dx = dy = 0;
		}
		
		// target is maxspeed (in meters/s) * inputs
		// lerp torwards that by the gods accel "factor" (not in meters2 but whatever)
		// gods mass is still ignored here, need to use rigidbody.addforce if we want that
		Vector2 targetVelocity = new Vector2(dx, dy) * god.maxSpeed;
		myRigidbody.velocity = Vector2.Lerp(myRigidbody.velocity, targetVelocity, god.acceleration * Time.deltaTime);
		
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
			model.localPosition = new Vector3(modelPosX * flip, model.localPosition.y, model.localPosition.z);
		}
	}

	private void handleGodCollision(Collision2D collision){
		if (collision.gameObject.tag == "God" && collision.gameObject.name == "Cthulhu") {
			float damage = collision.relativeVelocity.magnitude * collision.gameObject.GetComponent<Rigidbody2D>().mass;
			god.damage(damage * .5f);
		}
	}
	private void handlePlanetCollision(Collision2D collision){
		if (collision.gameObject.tag == "Planet") {
			// since were using mainly circle colliders, the first contact point will probably be the only one
			ContactPoint2D first = collision.contacts[0];
			// if either are our planetCollider then we dont take damage
			if ((first.collider == planetCollider || first.otherCollider == planetCollider) && myPlanet) {
				AudioManager.instance.playSound("Collision", collision.contacts[0].point, 1f);
				myPlanet.damage();
				return;
			} else if (invincible > .5f) {
				switch (god.god) {
				case Gods.THOR:
					if (god.getCounter() > 10f) {
						god.resetCounter();
						invincible = 0f;
						//myRigidbody.AddForce(-collision.relativeVelocity);
						return;
					}
					break;
				}
				
				// otherwise one of our gods colliders have been hit so we take damage
				AudioManager.instance.playSound("GodHurt", transform.position, 1f);
				float damage = collision.relativeVelocity.magnitude * collision.gameObject.GetComponent<Planet>().getMass();
				
				god.damage(damage * .5f);
				invincible = 0f;
			}
		}
	}
	private void handleGodPassives(){
		switch (god.god) {
		case Gods.THOR:
			if (god.getCounter() > 10f) {
				sr.color = Color.grey;
			} else {
				sr.color = Color.white;
			}
			break;
			
		case Gods.MICHAEL_JORDAN:
			if (myPlanet) {
				if (myPlanet.type == PlanetType.BASKETBALL) {
					god.throwStrength = 60f;
				} else {
					god.throwStrength = 20f;
				}
			}
			break;
			
		case Gods.ZEUS:
			if (myPlanet) {
				// this needs testing for sure to find good balance
				myPlanet.changeMass(Time.deltaTime / 10f);
				myRigidbody.mass += Time.deltaTime / 10f;
				myPlanet.changeRadius(Time.deltaTime / 10f);
				planetCollider.radius = myPlanet.getRadius();
			}
			break;
			
		default:
			break;
			
		}
	}

	private void handleSunDamage(Collider2D col){
		if (col.tag == "Sun") {
			god.damage(Time.deltaTime * 10f);
		}
	}
	private void handlePlanetCatch(Collider2D col){
		if (name != "Cthulhu") {
			bool inputFire = (usingJoysticks) ? Input.GetAxis ("Fire_360_" + player) < 0.0 : Input.GetButton ("Fire" + player);
			if (col.tag == "Planet" && inputFire && !myPlanet) {
				bool canCatch = col.gameObject.GetComponent<Planet> ().lastHolder == this || col.gameObject.GetComponent<Planet> ().state == PlanetState.ORBITING || timeSinceTryCatch < .25f;
				if (releaseButtonFire && canCatch) {
					holdPlanet (col.gameObject.GetComponent<Planet> ());
				}
			}
		}
	}
}

