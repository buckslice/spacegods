using UnityEngine;
using System.Collections;

public enum PlanetType {
    BASKETBALL,
    ICY,
    MOLTEN,
    METAL,
    ROCKY,
    TROPICAL
}

public enum PlanetState {
    HELD,
    THROWN,
    ORBITING
}

public class Planet : MonoBehaviour {
    private int health;
    private float gravity = 20f;
    private Transform gravitationTarget;
    public GodController lastHolder;

    private float invulnTime;
	private float particleTimer;
    private Transform texture;
    private Transform shade;
    private Transform cracked;
    private Vector3 origTextScale;
    private Vector3 origShadeScale;
    private Vector3 origCrackedScale;
    private SpriteRenderer crackedsr;
	private SpriteRenderer shadesr;
    public SpriteRenderer sr;
    public Rigidbody2D rb;
    public CircleCollider2D cc;
    public PhysicsMaterial2D noBounce;
    public PlanetType type;
    public PlanetState state;
	public float maxSpeed = 200f;
	public ParticleSystem particles;

    void Awake() {
        // only need to do these once
        texture = transform.Find("Texture").transform;
        shade = transform.Find("Shade").transform;
        cracked = transform.Find("Cracked").transform;
		shadesr = shade.GetComponent<SpriteRenderer> ();
        crackedsr = cracked.GetComponent<SpriteRenderer>();
        sr = texture.GetComponent<SpriteRenderer>();
        origTextScale = texture.localScale;
        origShadeScale = shade.localScale;
        origCrackedScale = cracked.localScale;
        gravitationTarget = GameObject.Find("Sun").transform;

        // planet spawner will call this
        //initializeVariables();
    }

    public void initializeVariables() {
        state = PlanetState.ORBITING;
        health = 2;
		particleTimer = 0f;
        crackedsr.enabled = false;
        invulnTime = -1f;

        // randomize size and mass
        switch (Random.Range(0, 3)) {
            case 0:             // small
                cc.radius = .76f;
                rb.mass = .6f;
                break;
            case 1:             // medium
                cc.radius = 1.0f;
                rb.mass = 1.0f;
                break;
            case 2:             // large
                cc.radius = 1.5f;
                rb.mass = 1.4f;
                break;
            default:
                break;
        }

        // add some random velocity tangent to the direction of gravity
        Vector3 dir = (gravitationTarget.position - rb.transform.position).normalized;

        // not sure what this is even doing but alright
        Vector3 tangent = Vector3.Cross(dir, new Vector3(0, 0, 1f));
        Vector3 halfway = (dir + tangent.normalized).normalized;
        halfway = (dir + halfway).normalized;
        Vector3 position = new Vector3(Random.Range(Mathf.Min(halfway.x, tangent.x), Mathf.Max(halfway.x, tangent.x)),
                                       Random.Range(Mathf.Min(halfway.y, tangent.y), Mathf.Max(halfway.y, tangent.y)),
                                       Random.Range(Mathf.Min(halfway.z, tangent.z), Mathf.Max(halfway.z, tangent.z)));

        rb.velocity = Random.Range(10f, 15f) * position;
        updateVariables();
    }

    void Update() {
        updateVariables();
        handlePlanetStates();

    }

    public void updateVariables() {
        float a = ToAngle(shade.position.x, shade.position.y);
        shade.rotation = Quaternion.Euler(0, 0, a * 180f / 3.1415f + 225);
        invulnTime -= Time.deltaTime;
        texture.localScale = origTextScale * cc.radius;
        shade.localScale = origShadeScale * cc.radius;
        cracked.localScale = origCrackedScale * cc.radius;
		if (particleTimer > 0f) {
			particleTimer += Time.deltaTime;	
		}
    }

    private void handlePlanetStates() {
        switch (state) {
            case PlanetState.THROWN:
                if (health <= 0) {
                    // don't destroy if you are being held, god will do it
					if(particleTimer == 0f){
						particleTimer = 0.001f;
						sr.enabled = crackedsr.enabled = shadesr.enabled = rb.simulated = false;
						particles.Play();
					}
					if(particleTimer > 1.5f){
                    	PlanetSpawner.current.returnPlanet(gameObject);
					}
                }
                break;
            case PlanetState.HELD:
                break;
            case PlanetState.ORBITING:
                break;
        }
        if (health == 1) {
            crackedsr.enabled = true;
        }
    }

    void FixedUpdate() {

		if (rb.velocity.magnitude > maxSpeed){
			rb.velocity = rb.velocity.normalized * maxSpeed;
		}
        // realistic gravity (scales with distance)
        Vector3 dist = (gravitationTarget.position - transform.position) / 10f;
        Vector3 g = Mathf.Max(gravity / dist.sqrMagnitude, gravity / 10f) * dist.normalized;
        rb.AddForce(g * rb.mass);
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Player") {
            cc.sharedMaterial = noBounce;
        } else if (collision.gameObject.tag == "Planet") {
            // only want one of the planets to play the sound so base it off random factor like x position
            if (transform.position.x > collision.transform.position.x) {
                //AudioManager.instance.playSound("Collision", transform.position, 1f);
            }
            switch (state) {
                case PlanetState.THROWN:
                    damage();
                    break;
                case PlanetState.HELD:
                    // this will never get called since this gameObjects collider
                    // isn't the one being hit
                    break;
                case PlanetState.ORBITING:
                    break;
            }

        } else if (collision.gameObject.tag == "Boundary") {
            PlanetSpawner.current.returnPlanet(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag == "Sun") { // kill planet if it hits sun
            //AudioManager.instance.playSound("Explosion0", transform.position, .25f);
			if (lastHolder){
				if (lastHolder.gameObject.name != "Anubis"){
            		PlanetSpawner.current.returnPlanet(gameObject);
				}
			}
			else{
				PlanetSpawner.current.returnPlanet(gameObject);
			}
        }
    }

    public void damage() {
        if (invulnTime < 0f) {
            health--;
            invulnTime = 1f;
        }
    }

    public float getRadius() {
        return cc.radius;
    }

    public float getHealth() {
        return health;
    }

    public float getMass() {
        return rb.mass;
    }

    public void changeRadius(float change) {
        cc.radius += change;
    }

    public void changeMass(float change) {
        rb.mass += change;
    }

    private float ToAngle(float x, float y) {
        if (x == 0)
            return y >= 0 ? Mathf.PI / 2 : Mathf.PI * 3 / 2;

        if (y == 0)
            return x < 0 ? Mathf.PI : 0;

        float r = Mathf.Atan(y / x);
        return x < 0 ? Mathf.PI + r : r;
    }
}
