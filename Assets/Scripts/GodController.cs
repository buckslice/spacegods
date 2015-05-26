using UnityEngine;

public class GodController : MonoBehaviour {

    public int id { get; set; }    // player joystick id

    public God god { get; private set; }  // state and variable holder
    private Rigidbody2D myRigidbody;
    private Planet myPlanet;     // planet you are holding
    private Planet myPlanet2;   // extra for Artemis&Apollo
    private CircleCollider2D planetCollider;   // planet collider  
    private CircleCollider2D planetCollider2; // extra for Artemis&Apollo

    // input tracking
    public bool freezeInputs { get; set; }
    private bool oldTrigger;
    private bool newTrigger;
    private bool releasedTrigger;
    private float timeSinceTrigger; // time since the player hit the trigger
    private float timeSinceCatch;   // time since the player caught a planet
    private bool usingJoysticks;

    // sprite orientation variables
    private Transform model;
    private float flipX;
    private float modelPosX;
    private bool isFlipped; // true when facing right; false when facing left
    private float holdDistance = 3f;    // range of holding
    private Animator anim;

    private GameObject go;
    private PlanetSpawner spawnerScript;

    // use this for initialization
    void Start() {
        // getting components
        god = GetComponent<God>();
        myRigidbody = GetComponent<Rigidbody2D>();
        planetCollider = gameObject.AddComponent<CircleCollider2D>();

        go = GameObject.Find("SCRIPTS");
        spawnerScript = (PlanetSpawner)go.GetComponent<PlanetSpawner>();

        anim = god.gameObject.GetComponent<Animator>();

        // add god controller to game
        Game.instance.addPlayer(this);

        // setting all the bools to their initial values
        freezeInputs = releasedTrigger = true;
        oldTrigger = newTrigger = isFlipped = planetCollider.enabled = usingJoysticks = false;

        // sprite flipping
        model = transform.Find("Sprite");
        flipX = model.localScale.x;
        modelPosX = model.localPosition.x;

        if (god.type == GodType.ARTEMIS_APOLLO || god.type == GodType.KITSUNE) {
            planetCollider2 = gameObject.AddComponent<CircleCollider2D>();
            planetCollider2.enabled = false;
        }

        // joystick handling
        string[] joysticks = Input.GetJoystickNames();
        for (int i = 0; i < joysticks.Length; i++) {
            if (joysticks[i] != "") {
                usingJoysticks = true;
            }
        }
    }

    // update is called once per frame
    void Update() {
        updateVariables();
        handleVelocityAndOrientation();
        handleThrow();
        handleGodPassives();

        //change animation if it exists
        if (anim) {
            anim.SetFloat("Velocity", myRigidbody.velocity.x);
        }
    }

    private void updateVariables() {
        timeSinceTrigger = (usingJoysticks ? !oldTrigger && newTrigger : Input.GetButtonDown("Fire" + id)) ? 0 : timeSinceTrigger;
        releasedTrigger = (usingJoysticks ? oldTrigger && !newTrigger : Input.GetButtonUp("Fire" + id)) ? true : releasedTrigger;
        oldTrigger = newTrigger;
        timeSinceTrigger += Time.deltaTime;
        timeSinceCatch += Time.deltaTime;
    }

    private void handleVelocityAndOrientation() {
        // making sure players cant press keyboard AND controller for speed boost
        float dx = 0f, dy = 0f;
        if (!freezeInputs) {
            if (usingJoysticks) {
                dx = Input.GetAxis("Horizontal_360_" + id);
                dy = Input.GetAxis("Vertical_360_" + id);
            } else {
                dx = Input.GetAxis("Horizontal" + id);
                dy = Input.GetAxis("Vertical" + id);
            }
        }
        if (god.state == GodState.DRUNK) {
            float temp = dx;
            dx = dy;
            dy = temp;
        }

        // target is maxspeed (in meters/s) * inputs
        // lerp torwards that by the gods accel "factor" (not in meters2 but whatever)
        // gods mass is still ignored here, need to use rigidbody.addforce if we want that
        Vector2 targetVelocity = new Vector2(dx, dy).normalized * god.maxSpeed;
        myRigidbody.velocity = Vector2.Lerp(myRigidbody.velocity, targetVelocity, god.acceleration * Time.deltaTime * 1.3f);

        // limit velocity to maxSpeed of god with an opposing force
        // this way gods can briefly travel faster than their maxSpeed due to outside forces
        if (myRigidbody.velocity.sqrMagnitude > god.maxSpeed * god.maxSpeed) {
            myRigidbody.AddForce(-myRigidbody.velocity.normalized * (myRigidbody.velocity.magnitude - god.maxSpeed));
        }

        // flips sprite based on last horizontal movement direction
        // as well as the default flip orientation of gods sprite
        if (!Mathf.Approximately(dx, 0f) || !Mathf.Approximately(Input.GetAxis("Horizontal_aim_360_" + id), 0f)) {
            isFlipped = Mathf.Approximately(Input.GetAxis("Horizontal_aim_360_" + id), 0f) ? dx < 0 :
                Input.GetAxis("Horizontal_aim_360_" + id) < 0f;
            int flip = isFlipped ? -1 : 1;
            model.localScale = new Vector3(flipX * flip, model.localScale.y, model.localScale.z);
            model.localPosition = new Vector3(modelPosX * flip, model.localPosition.y, model.localPosition.z);
        }
    }

    private void handleThrow() {
        newTrigger = Input.GetAxis("Fire_360_" + id) < 0.0;
        bool fireInput = usingJoysticks ? !oldTrigger && newTrigger : Input.GetButtonDown("Fire" + id);

        Vector2 aim;
        if (!usingJoysticks) {
            aim = myRigidbody.velocity.normalized;
        } else {
            float xAim = Input.GetAxis("Horizontal_aim_360_" + id);
            float yAim = Input.GetAxis("Vertical_aim_360_" + id);
            aim = new Vector2(xAim, yAim).normalized;
            if (aim.sqrMagnitude < .01f * .01f) {
                aim = myRigidbody.velocity.normalized;
            }
        }

        if (myPlanet) {
            bool canThrow = !myPlanet.particles.isPlaying;
            if (fireInput && canThrow) {    // throw planet
                throwPlanet(aim);
            } else {    // move planet where aiming for blocking
                blockWithPlanet(aim);
            }
        }

        if (god.type == GodType.CTHULHU && god.coolDown < 0f && fireInput) {
            //transform.position += new Vector3(xAim, yAim, 0f).normalized * god.getStartingThrowStrength();
            myRigidbody.AddForce(aim * god.startingThrowStrength * 2f, ForceMode2D.Impulse);
            god.resetCooldown();
            AudioManager.instance.playSound("Monster Growl", transform.position, 0.5f);

        }
    }

    // some are implemented here and others in God class
    private void handleGodPassives() {
        switch (god.type) {
            case GodType.ZEUS:
                if (myPlanet) {
                    // this needs testing for sure to find good balance
                    myPlanet.changeMass(Time.deltaTime / 10f);
                    myRigidbody.mass += Time.deltaTime / 10f;
                    myPlanet.changeRadius(Time.deltaTime / 10f);
                    planetCollider.radius = myPlanet.getRadius();
                    if (!god.particles.isPlaying) {
                        god.particles.Play(false);
                    }
                } else {
                    god.particles.Stop();
                }
                break;
            case GodType.MICHAEL_JORDAN:
                if (myPlanet) {
                    if (myPlanet.type == PlanetType.BASKETBALL) {
                        god.throwStrength = god.startingThrowStrength * 3f;
                    } else {
                        god.throwStrength = god.startingThrowStrength;
                    }
                }
                break;

            case GodType.JESUS:
                if (myPlanet) {
                    if (myPlanet.type == PlanetType.WATER) {
                        myPlanet.sr.color = Color.red;
                    }
                }
                break;
            case GodType.KHONSU:
                if (myPlanet) {
                    if (myPlanet.type == PlanetType.MOON) {
                        Time.timeScale = 0.5f;
                        god.acceleration = god.startingAcceleration * 2f;
                        god.maxSpeed = god.startingMaxSpeed * 2f;
                    }
                } else {
                    Time.timeScale = 1f;
                    god.acceleration = god.startingAcceleration;
                    god.maxSpeed = god.startingMaxSpeed;
                }
                break;
            default:
                break;
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("God") && collision.gameObject.GetComponent<God>().type == GodType.CTHULHU) {
            float damage = -collision.relativeVelocity.magnitude * collision.gameObject.GetComponent<Rigidbody2D>().mass;
            god.changeHealth(damage * .5f);
        }

        if (collision.gameObject.CompareTag("Planet")) {
            handleCollisionWithPlanet(collision);
        }
    }

    void OnTriggerStay2D(Collider2D col) {
        if (col.CompareTag("Planet")) {
            bool inputFire = (usingJoysticks) ? Input.GetAxis("Fire_360_" + id) < 0.0 : Input.GetButton("Fire" + id);
            if (inputFire && !myPlanet) {
                Planet planet = col.gameObject.GetComponent<Planet>();
                bool canCatch = planet.lastHolder == this || planet.state == PlanetState.ORBITING || timeSinceTrigger < .25f;
                if (releasedTrigger && canCatch) {
                    holdPlanet(planet);
                }
            }
        } else if (col.CompareTag("Sun")) {
            god.changeHealth(-Time.deltaTime * 10f, true);
        } else if (col.CompareTag("God")) {
            God colGod = col.GetComponent<God>();
            if (colGod.type == GodType.MORRIGAN && colGod.auraCollider.enabled) {
                god.changeHealth(-Time.deltaTime * 2f, true);
            }
        }
    }

    private void holdPlanet(Planet planet) {
        myPlanet = planet;
        if (myPlanet.lastHolder && god.type == GodType.THOR && god.special && myPlanet.lastHolder.god.type == GodType.THOR && timeSinceTrigger > 3f) {
            god.changeHealth(myPlanet.rb.mass * god.throwStrength / 2f, true);
        }
        timeSinceCatch = 0f;
        myPlanet.lastHolder = this;
        myPlanet.state = PlanetState.HELD;
        myPlanet.rb.simulated = false;  // disable planets physics
        myPlanet.transform.parent = transform;
        if (god.type != GodType.ARTEMIS_APOLLO) {
            myRigidbody.mass += myPlanet.rb.mass; // add planets mass to your own
            planetCollider.radius = myPlanet.getRadius();   // set our planetCollider equal to radius of planet
            planetCollider.enabled = true;
        } else {    // splits planet between myPlanet and myPlanet2
            myPlanet.rb.mass = myPlanet.rb.mass / 2f;
            myPlanet.cc.radius = myPlanet.cc.radius / 2f;
            if (myPlanet.rb.mass < .3f) {
                myPlanet.rb.mass = .3f;
            }
            if (myPlanet.cc.radius < .5f) {
                myPlanet.cc.radius = .5f;
            }

            myPlanet2 = PlanetSpawner.current.getPlanet().GetComponent<Planet>();
            myPlanet2.gameObject.SetActive(true);
            myPlanet2.name = "A&A Duplicate Planet";
            myPlanet2.initializeVariables();
            myPlanet2.transform.position = transform.position;
            myPlanet2.transform.parent = transform;
            myPlanet2.sr.sprite = myPlanet.sr.sprite;
            myPlanet2.type = myPlanet.type;
            myPlanet2.lastHolder = this;
            myPlanet2.state = PlanetState.HELD;
            myPlanet2.rb.simulated = false;
            myPlanet2.transform.parent = transform;
            myPlanet2.rb.mass = myPlanet.rb.mass;
            myPlanet2.cc.radius = myPlanet.cc.radius;
            myRigidbody.mass += myPlanet.rb.mass + myPlanet2.rb.mass;
            planetCollider.radius = myPlanet.getRadius();
            planetCollider.enabled = true;
            planetCollider2.radius = myPlanet2.getRadius();
            planetCollider2.enabled = true;
        }
    }

    private void throwPlanet(Vector2 aim) {
        if (timeSinceCatch < .1f) {
            return;
        }
        releasedTrigger = false;
        myPlanet.state = PlanetState.THROWN;
        myPlanet.transform.parent = null;
        myPlanet.transform.position = transform.position + new Vector3(aim.x, aim.y, 0) * holdDistance;
        myPlanet.rb.simulated = true;  //reenable planets physics
        myPlanet.rb.velocity = myRigidbody.velocity + aim * god.throwStrength;
        myRigidbody.mass -= myPlanet.rb.mass; // subtract off planets mass
        planetCollider.enabled = false;
        if (god.type == GodType.SUN_WUKONG && god.coolDown < 0f) {
            myPlanet.hide();
            god.resetCooldown();
        }
        //change animation if it exists kinda clunky maybe find a better way to check if null
        if (god.gameObject.GetComponent<Animator>() != null) {
            anim.SetTrigger("Shoot");
        }
        myPlanet = null;

        if (god.type == GodType.ARTEMIS_APOLLO && myPlanet2) {
            myPlanet2.state = PlanetState.THROWN;
            myPlanet2.transform.parent = null;
            myPlanet2.transform.position = transform.position - new Vector3(aim.x, aim.y, 0) * holdDistance;
            myPlanet2.rb.simulated = true;  //reenable planets physics
            myPlanet2.rb.velocity = -myRigidbody.velocity - aim * god.throwStrength;
            planetCollider2.enabled = false;
            myPlanet2 = null;
        }

        if (god.type == GodType.THOR && !god.special) {
            god.special = true;
        }

    }

    private void blockWithPlanet(Vector2 aim) {
        Vector2 holdPos = aim * holdDistance;
        Vector3 target = transform.position + new Vector3(holdPos.x, holdPos.y, 0);
        myPlanet.transform.position = Vector3.Lerp(myPlanet.transform.position, target, 15f * Time.deltaTime);
        planetCollider.offset = holdPos;
        if (god.type == GodType.ARTEMIS_APOLLO && myPlanet2) {
            Vector3 target2 = transform.position - new Vector3(holdPos.x, holdPos.y, 0);
            myPlanet2.transform.position = Vector3.Lerp(myPlanet2.transform.position, target2, 15f * Time.deltaTime);
            planetCollider2.offset = -holdPos;
        }
    }

    public void heldPlanetDestroyed(Planet planet) {
        if (god.type == GodType.SHIVA) {
            god.changeHealth(myPlanet.rb.mass * 10f);
        }

        myRigidbody.mass -= planet.rb.mass;

        if (god.type == GodType.ARTEMIS_APOLLO) {
            planetCollider2.enabled = false;
            if (planet == myPlanet2) {  //null second
                myPlanet2 = null;
            } else if (myPlanet2) {        //replace first with second
                myPlanet = myPlanet2;
            } else {                     //null first
                myPlanet = null;
                planetCollider.enabled = false;
            }
        } else {
            myPlanet = null;
            planetCollider.enabled = false;
        }
    }

    private void handleCollisionWithPlanet(Collision2D collision) {
        // since were using mainly circle colliders, the first contact point will probably be the only one
        ContactPoint2D first = collision.contacts[0];
        // if either of the colliders are our planetCollider then we dont take damage
        if (myPlanet) {
            if (myPlanet2) {
                if ((first.collider == planetCollider2 || first.otherCollider == planetCollider2)) {
                    AudioManager.instance.playSound("Collision", collision.contacts[0].point, 1f);
                    if (myPlanet2.type != PlanetType.SMASH)
                        myPlanet2.damage();
                    return;
                }
            }
            if ((first.collider == planetCollider || first.otherCollider == planetCollider)) {
                AudioManager.instance.playSound("Collision", collision.contacts[0].point, 1f);
                if (myPlanet.type != PlanetType.SMASH)
                    myPlanet.damage();
                return;
            }
        }

        // return if you recently threw this planet
        Planet planetThatHitMe = collision.gameObject.GetComponent<Planet>();
        if (planetThatHitMe.lastHolder == this) {
            return;
        }

        float damage = -collision.relativeVelocity.magnitude * planetThatHitMe.getMass();
        if (planetThatHitMe.state == PlanetState.ORBITING) {
            damage *= .25f;
        }
        if (god.changeHealth(damage * .5f)) {   // if successfully damage this god
            AudioManager.instance.playSound("GodHurt", transform.position, 1f);
            // if no recent holder of planet then return
            if (!planetThatHitMe.lastHolder) {
                return;
            }
            if (planetThatHitMe.lastHolder.god.type == GodType.POSEIDON && planetThatHitMe.type == PlanetType.ICY) {
                god.state = GodState.FROZEN;
                god.CCTimer = 3f;
            }
            if (planetThatHitMe.lastHolder.god.type == GodType.QUETZALCOATL && planetThatHitMe.type == PlanetType.TROPICAL) {
                god.state = GodState.POISONED;
                god.CCTimer = 4f;
            }
            if (planetThatHitMe.lastHolder.god.type == GodType.JESUS && planetThatHitMe.type == PlanetType.WATER) {
                god.state = GodState.DRUNK;
                god.CCTimer = 5f;
            }
            if (planetThatHitMe.lastHolder.god.type == GodType.NIKE) {
                planetThatHitMe.lastHolder.god.throwStrength += 4f;
            }

            if (planetThatHitMe.type == PlanetType.SMASH) {
                god.changeHealth(-1000, true);
                planetThatHitMe.killPlanet();
                spawnerScript.setSmashPresence(false);
            }

            if (planetThatHitMe.lastHolder.god.isKitsune) {
                planetThatHitMe.lastHolder.god.type = god.type;
                planetThatHitMe.lastHolder.god.sr.sprite = god.sr.sprite;
            }
        }
    }

    public Vector3 getVelocity() {
        return myRigidbody.velocity;
    }

}

