using UnityEngine;
using System.Collections;

public enum PlanetType {
    BASKETBALL,
    ICY,
    MOLTEN,
    MOON,
    ROCKY,
    TROPICAL,
    WATER,
	SMASH
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
    private float thrownTimer;
    private float invulnerabe;
    private float particleTimer;
    private Transform texture;
    private Transform shade;
    private Transform cracked;
    private Vector3 origTextScale;
    private Vector3 origShadeScale;
    private Vector3 origCrackedScale;
    private SpriteRenderer crackedsr;
    private SpriteRenderer shadesr;

    public GodController lastHolder;
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    public CircleCollider2D cc;
    public PlanetType type;
    public PlanetState state;
    public float maxSpeed = 200f;
    public ParticleSystem particles;
    private ParticleSystem thrownParticles;
    //private Object explode;

	private GameObject go;
	private PlanetSpawner spawnerScript;

    void Awake() {
        // only need to do these once
        texture = transform.Find("Texture").transform;
        shade = transform.Find("Shade").transform;
        cracked = transform.Find("Cracked").transform;
        shadesr = shade.GetComponent<SpriteRenderer>();
        crackedsr = cracked.GetComponent<SpriteRenderer>();
        particles = GetComponent<ParticleSystem>();
        thrownParticles = texture.GetComponent<ParticleSystem>();
        sr = texture.GetComponent<SpriteRenderer>();
        origTextScale = texture.localScale;
        origShadeScale = shade.localScale;
        origCrackedScale = cracked.localScale;
        gravitationTarget = GameObject.Find("Sun").transform;

		go = GameObject.Find("SCRIPTS"); 
		spawnerScript = (PlanetSpawner) go.GetComponent<PlanetSpawner> ();
    }

    public void initializeVariables() {
        state = PlanetState.ORBITING;
        health = 2;
        lastHolder = null;
        particleTimer = 0f;
        thrownTimer = 0f;
        crackedsr.enabled = false;
        invulnerabe = -1f;
        sr.enabled = shadesr.enabled = rb.simulated = cc.enabled = true;
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

        updateVariables();
    }

    void Update() {
        updateVariables();
        handlePlanetStates();
    }

    public void updateVariables() {
        float a = ToAngle(shade.position.x, shade.position.y);
        shade.rotation = Quaternion.Euler(0, 0, a * 180f / 3.1415f + 225);
        texture.localScale = origTextScale * cc.radius;
        shade.localScale = origShadeScale * cc.radius;
        cracked.localScale = origCrackedScale * cc.radius;
        thrownParticles.startSize = cc.radius * 1f;
        particles.startSize = cc.radius * 2f;

        invulnerabe -= Time.deltaTime;
        particleTimer -= Time.deltaTime;
        thrownTimer -= Time.deltaTime;
    }

    private void handlePlanetStates() {
        switch (state) {
            case PlanetState.THROWN:
                if (thrownTimer < 0f) {
                    if (!thrownParticles.isPlaying) {
                        thrownParticles.Play();
                        thrownTimer = 3f;
                    } else {
                        if (lastHolder && lastHolder.god.type == GodType.THOR && lastHolder.god.special) {
                            gravity = 0f;
                            rb.AddForce(new Vector2(lastHolder.transform.position.x - transform.position.x, lastHolder.transform.position.y - transform.position.y).normalized * lastHolder.god.throwStrength);
                            return;
                        }
                        gravity = 20f;
                        state = PlanetState.ORBITING;
                        if (thrownParticles.isPlaying) {
                            thrownParticles.Stop();
                        }
                    }
                }
                break;
            case PlanetState.HELD:
                thrownTimer = 0f;
                if (thrownParticles.isPlaying) {
                    thrownParticles.Stop();
                }
                break;
            case PlanetState.ORBITING:
                lastHolder = null;
                if (type == PlanetType.WATER) {
                    sr.color = Color.white;
                }
                gravity = 20f;
                if (thrownParticles.isPlaying) {
                    thrownParticles.Stop();
                }
                show();
                break;
        }

        if (health == 0) {
            if (particleTimer < 0f) {
                if (!particles.isPlaying) {
                    particles.Play();
                    particleTimer = 1.8f;
                    sr.enabled = shadesr.enabled = rb.simulated = cc.enabled = false;
                } else {
                    if (state == PlanetState.HELD) {
                        lastHolder.heldPlanetDestroyed(this);
                    }
                    if (lastHolder && lastHolder.god.type == GodType.THOR) {
                        lastHolder.god.special = false;
                    }
                    PlanetSpawner.current.returnPlanet(gameObject);
                }
            }
        }
        crackedsr.enabled = health == 1;
    }

    void FixedUpdate() {
        if (rb.velocity.magnitude > maxSpeed) {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
        // realistic gravity (scales with distance)
        Vector3 dist = (gravitationTarget.position - transform.position) / 10f;
        Vector3 g = Mathf.Max(gravity / dist.sqrMagnitude, gravity / 10f) * dist.normalized;
        rb.AddForce(g * rb.mass);
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Planet")) {
            // only want one of the planets to play the sound so base it off random factor like x position
            if (transform.position.x > collision.transform.position.x) {
                //AudioManager.instance.playSound("Collision", transform.position, 1f);
            }
            switch (state) {
                case PlanetState.THROWN:
                    if (type != PlanetType.SMASH)
						damage();
                    break;
                case PlanetState.HELD:
                    // this will never get called since this gameObjects collider
                    // isn't the one being hit
                    break;
                case PlanetState.ORBITING:
                    break;
            }
        } else if (collision.gameObject.CompareTag("Boundary")) {
            if (lastHolder && lastHolder.god.type == GodType.THOR) {
                lastHolder.god.special = false;
            }
            PlanetSpawner.current.returnPlanet(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("Sun")) {
            // kill planet if it hits sun
            //AudioManager.instance.playSound("Explosion0", transform.position, .25f);
            if (lastHolder && lastHolder.god.type == GodType.ANUBIS) {
                gravity = 0f;
            } else {
                if (lastHolder && lastHolder.god.type == GodType.THOR) {
                    lastHolder.god.special = false;
                }
                PlanetSpawner.current.returnPlanet(gameObject);
            }
        }
    }

    public void damage() {
        if (invulnerabe < 0f) {
            health--;
            invulnerabe = 1f;
        }
    }

	public void killPlanet() {
		health = 0;
		}

    public void hide() {
        sr.color = new Color(1f, 1f, 1f, 0.1f);
        shadesr.color = new Color(1f, 1f, 1f, 0.1f);
        crackedsr.color = new Color(1f, 1f, 1f, 0.1f);
        thrownParticles.enableEmission = false;
    }

    private void show() {
        sr.color = new Color(1f, 1f, 1f, 1f);
        shadesr.color = new Color(1f, 1f, 1f, 1f);
        crackedsr.color = new Color(1f, 1f, 1f, 1f);
        thrownParticles.enableEmission = true;
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
