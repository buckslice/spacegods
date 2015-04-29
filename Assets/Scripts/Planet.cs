using UnityEngine;
using System.Collections;

public enum PlanetType {
    BASKETBALL,
    GOLD,
    ICY,
    FIRE,
    LAVA,
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
    private float radius;
    private float mass;
    private int health;
    public float gravity = 1f;  // should make sun class eventually and move this there
    private Transform gravitationTarget;
    public GodController lastHolder;

    private float invulnTime;

    private Transform texture;
    private Transform shade;
    private Vector3 origTextScale;
    private Vector3 origShadeScale;
    private SpriteRenderer sr;
    public Rigidbody2D rb;
    public CircleCollider2D cc;
    public PhysicsMaterial2D noBounce;
    public PlanetType type;
    public PlanetState state;

    void Start() {
        sr = transform.Find("Texture").GetComponent<SpriteRenderer>();
        state = PlanetState.ORBITING;
        health = 2;

        // randomize size and mass
        switch (Random.Range(0, 3)) {
            case 0:             // small
                radius = .76f;
                mass = .6f;
                break;
            case 1:             // medium
                radius = 1.0f;
                mass = 1.0f;
                break;
            case 2:             // large
                radius = 1.5f;
                mass = 1.4f;
                break;
            default:
                break;
        }

        texture = transform.Find("Texture").transform;
        shade = transform.Find("Shade").transform;
        origTextScale = texture.localScale;
        origShadeScale = shade.localScale;
        gravitationTarget = GameObject.Find("Sun").transform;

        // add some random velocity tangent to the direction of gravity
        Vector3 dir = (gravitationTarget.transform.position - rb.transform.position).normalized;

        // Random.Range(-1f,1f) instead of 1f at the end will make orbits go either way
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
        float a = ToAngle(shade.transform.position.x, shade.transform.position.y);
        shade.transform.rotation = Quaternion.Euler(0, 0, a * 180f / 3.1415f + 225);
        invulnTime -= Time.deltaTime;
        switch (state) {
            case PlanetState.THROWN:
                if (health <= 0) {
                    // don't destroy if you are being held, god will do it
                    Destroy(gameObject);
                }
                break;
            case PlanetState.HELD:
                break;
            case PlanetState.ORBITING:
                break;
        }
        if (health == 1) {
            sr.color = Color.red;
        }

        // update planets based on their enum later?
    }
    void FixedUpdate() {
        // less realistic constant gravity (gravity constant was 4)
        //Vector3 g = (gravitationTarget.transform.position - myRigidBody.transform.position).normalized * gravity;

        // more realistic gravity (scales with distance)
        Vector3 dist = (gravitationTarget.position - transform.position) / 10f;
        Vector3 g = Mathf.Max(gravity / dist.sqrMagnitude, 1f) * dist.normalized;

        rb.AddForce(g * rb.mass);
    }

    private float ToAngle(float x, float y) {
        if (x == 0)
            return y >= 0 ? Mathf.PI / 2 : Mathf.PI * 3 / 2;

        if (y == 0)
            return x < 0 ? Mathf.PI : 0;

        float r = Mathf.Atan(y / x);
        return x < 0 ? Mathf.PI + r : r;
    }

    public void updateVariables() {
        texture.localScale = origTextScale * radius;
        shade.localScale = origShadeScale * radius;

        cc.radius = radius;
        rb.mass = mass;
    }

    public void damage() {
        if (invulnTime < 0f) {
            health--;
            invulnTime = 1f;
        }
    }
    public float getRadius() {
        return radius;
    }

    public float getHealth() {
        return health;
    }

    public float getMass() {
        return mass;
    }

    public void changeRadius(float change) {
        radius += change;
    }

    public void changeMass(float change) {
        mass += change;
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
                    gameObject.GetComponent<Planet>().damage();
                    break;
                case PlanetState.HELD:
                    gameObject.GetComponent<Planet>().damage();
                    break;
                case PlanetState.ORBITING:
                    break;
            }

        } else if (collision.gameObject.tag == "Boundary") {
            --PlanetSpawner.planetNum;
            Destroy(gameObject);
        }
    }


    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag == "Sun") { // kill planet if it hits sun
            //AudioManager.instance.playSound("Explosion0", transform.position, .25f);
            --PlanetSpawner.planetNum;
            Destroy(gameObject);
        }
    }
}
