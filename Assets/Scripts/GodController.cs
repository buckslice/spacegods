using UnityEngine;
using System.Collections;

public class GodController : MonoBehaviour {
    // player number, used for input
    private int player = 0;

    private God god;
    private Transform model;
    private Rigidbody2D myRigidbody;
    private Planet myPlanet;
    private CircleCollider2D planetCollider;
    private SpriteRenderer sr;

    // compare trigger values in previous frame
    private bool oldTrigger;
    private bool newTrigger;
    private bool releaseButtonFire;
    private float timeSinceTryCatch;

    // track the original orientation of the model for turning
    private float flipX;
    private float modelPosX;
    private bool isFlipped; // true when facing right; false when facing left

    private bool freezeInputs;
    private float frozenTime = 10f;
    private bool usingJoysticks;

    private float invincible;

    // use this for initialization
    void Start() {
        initializeVariables();
    }

    // update is called once per frame
    void Update() {
        checkForDeath();
        handleVelocityAndOrientation();
        handleThrow();
        updateVariables();
        handleGodPassives();
    }

    void OnCollisionEnter2D(Collision2D collision) {
        handleGodCollision(collision);
        handlePlanetCollision(collision);
    }

    void OnTriggerStay2D(Collider2D col) {
        handleSunDamage(col);
        handlePlanetCatch(col);
    }

    private void initializeVariables() {
        Game.instance.addPlayer(this);
        freezeInputs = true;
        releaseButtonFire = true;
        oldTrigger = false;
        newTrigger = false;
        isFlipped = false;
        model = transform.Find("Model");
        myRigidbody = GetComponent<Rigidbody2D>();
        god = GetComponent<God>();
        flipX = model.localScale.x;
        modelPosX = model.localPosition.x;
        planetCollider = gameObject.AddComponent<CircleCollider2D>();
        planetCollider.enabled = false;
        sr = model.GetComponent<SpriteRenderer>();

        usingJoysticks = false;
        string[] joysticks = Input.GetJoystickNames();
        for(int i = 0; i < joysticks.Length; i++) {
            if(joysticks[i] != "") {
                usingJoysticks = true;
            }
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
    private void blockWithPlanet(Vector2 aim) {
        float holdDistance = 3f; // should be halo radius
        Vector2 holdPos = aim * holdDistance;
        Vector3 target = transform.position + new Vector3(holdPos.x, holdPos.y, 0);
        myPlanet.transform.position = Vector3.Lerp(myPlanet.transform.position, target, 6f * Time.deltaTime);
        planetCollider.offset = holdPos;
    }

    private void destroyDeadHeldPlanet() {
        if (god.god == Gods.SHIVA) {
            god.changeHealth(-myPlanet.rb.mass * 10f);
        }
        myRigidbody.mass -= myPlanet.rb.mass;
        planetCollider.enabled = false;
        PlanetSpawner.current.returnPlanet(myPlanet.gameObject);
        myPlanet = null;
    }

    private void updateVariables() {
        timeSinceTryCatch = (usingJoysticks ? !oldTrigger && newTrigger : Input.GetButtonDown("Fire" + player)) ? 0 : timeSinceTryCatch;
        releaseButtonFire = (usingJoysticks ? oldTrigger && !newTrigger : Input.GetButtonUp("Fire" + player)) ? true : releaseButtonFire;
        oldTrigger = newTrigger;
        timeSinceTryCatch += Time.deltaTime;
        invincible += Time.deltaTime;
        frozenTime -= Time.deltaTime;
        if (frozenTime < 0f) {
            freezeInputs = false;
            sr.color = Color.white;
        }
    }

    private void handlePlanetCollision(Collision2D collision) {
        if (collision.gameObject.tag == "Planet") {
            // since were using mainly circle colliders, the first contact point will probably be the only one
            ContactPoint2D first = collision.contacts[0];
            // if either of the colliders are our planetCollider then we dont take damage
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

                Planet planetThatHitMe = collision.gameObject.GetComponent<Planet>();
                if (planetThatHitMe.lastHolder == this) {
                    return;
                }
                // otherwise one of our gods colliders have been hit so we take damage
                AudioManager.instance.playSound("GodHurt", transform.position, 1f);
                float damage = collision.relativeVelocity.magnitude * planetThatHitMe.getMass();
                god.changeHealth(damage * .5f);
                invincible = 0f;

                if (planetThatHitMe.lastHolder && planetThatHitMe.lastHolder.god.god == Gods.POSEIDON && planetThatHitMe.type == PlanetType.ICY) {
                    freezeInputs = true;
                    sr.color = Color.blue;
                    frozenTime = 3f;
                }
            }
        }
    }

    private void handleThrow() {
        newTrigger = Input.GetAxis("Fire_360_" + player) < 0.0;
        bool fireInput = usingJoysticks ? !oldTrigger && newTrigger : Input.GetButtonDown("Fire" + player);
        float xAim = usingJoysticks ? Input.GetAxis("Horizontal_aim_360_" + player) : isFlipped ? -1f : 1f;
        float yAim = usingJoysticks ? Input.GetAxis("Vertical_aim_360_" + player) : 0f;
        Vector2 aim = new Vector2(xAim, yAim).normalized;
        if (myPlanet) {     // if your god is holding a planet
            if (myPlanet.getHealth() > 0) {
                if (fireInput) {    // throw planet
                    throwPlanet(aim);
                } else {    // move planet where aiming for blocking
                    blockWithPlanet(aim);
                }
            } else {    //planet died before it was thrown
                destroyDeadHeldPlanet();
            }
        }

        if (god.god == Gods.CTHULHU && god.getCounter() > 5f && fireInput) {
            //transform.position += new Vector3(xAim, yAim, 0f).normalized * god.getStartingThrowStrength();
            myRigidbody.AddForce(new Vector3(xAim, yAim, 0f).normalized * god.getStartingThrowStrength() * 2f, ForceMode2D.Impulse);
            god.resetCounter();
        }
    }

    private void checkForDeath() {
        if (god.getCurrentHealth() <= 0 && !Game.instance.gameIsOver()) {
            Game.instance.removePlayer(this);
            GetComponent<HealthBar>().deleteGameObject();
            Destroy(gameObject);
        }
    }

    private void handleVelocityAndOrientation() {
        // making sure players cant press keyboard AND controller for speed boost
        float dx = 0f, dy = 0f;
        if (!freezeInputs) {
            if (usingJoysticks) {
                dx = Input.GetAxis("Horizontal_360_" + player);
                dy = Input.GetAxis("Vertical_360_" + player);
            } else {
                dx = Input.GetAxis("Horizontal" + player);
                dy = Input.GetAxis("Vertical" + player);
            }
        }

        // target is maxspeed (in meters/s) * inputs
        // lerp torwards that by the gods accel "factor" (not in meters2 but whatever)
        // gods mass is still ignored here, need to use rigidbody.addforce if we want that
        Vector2 targetVelocity = new Vector2(dx, dy).normalized * god.maxSpeed;
        myRigidbody.velocity = Vector2.Lerp(myRigidbody.velocity, targetVelocity, god.acceleration * Time.deltaTime);

        // limit velocity to maxSpeed of god
        if (myRigidbody.velocity.sqrMagnitude > god.maxSpeed * god.maxSpeed) {
            myRigidbody.AddForce(-myRigidbody.velocity.normalized * (myRigidbody.velocity.magnitude - god.maxSpeed));
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

    private void handleGodCollision(Collision2D collision) {
        if (collision.gameObject.tag == "God" && collision.gameObject.name == "Cthulhu") {
            float damage = collision.relativeVelocity.magnitude * collision.gameObject.GetComponent<Rigidbody2D>().mass;
            god.changeHealth(damage * .5f);
        }
    }

    private void handleGodPassives() {
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
                        god.throwStrength = god.getStartingThrowStrength() * 3f;
                    } else {
                        god.throwStrength = god.getStartingThrowStrength();
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
            case Gods.SUN_WUKONG:
                bool inputFire = (usingJoysticks) ? Input.GetAxis("Fire_360_" + player) < 0.0 : Input.GetButton("Fire" + player);
                if (!myPlanet && inputFire) {
                    //passive goes here
                }
                break;

            case Gods.ODIN:
                float r = god.getCurrentHealth() / god.maxHealth;
                god.throwStrength = god.getStartingThrowStrength() * (3f - r * 2f);
                sr.color = new Color(1f, r, r);
                break;

            case Gods.ATHENA:
                if (god.getCurrentHealth() < god.maxHealth / 2f) {
                    god.changeHealth(-5f * Time.deltaTime);
                }
                break;
            default:
                break;

        }
    }

    private void handleSunDamage(Collider2D col) {
        if (col.tag == "Sun") {
            god.changeHealth(Time.deltaTime * 10f);
        }
    }

    private void handlePlanetCatch(Collider2D col) {
        bool inputFire = (usingJoysticks) ? Input.GetAxis("Fire_360_" + player) < 0.0 : Input.GetButton("Fire" + player);
        if (col.tag == "Planet" && inputFire && !myPlanet && god.god != Gods.CTHULHU) {
            Planet planet = col.gameObject.GetComponent<Planet>();
            bool canCatch = planet.lastHolder == this || planet.state == PlanetState.ORBITING || timeSinceTryCatch < .25f;
            if (releaseButtonFire && canCatch) {
                holdPlanet(planet);
            }
        }
    }

    public void unlock() {
        freezeInputs = false;
    }

    public void setPlayer(int player) {
        this.player = player;
    }

    public int getPlayer() {
        return player;
    }
}

