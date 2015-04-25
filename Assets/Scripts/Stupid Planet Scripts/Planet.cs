using UnityEngine;
using System.Collections;

public enum PlanetType {
    BASKETBALL,
    GOLD,
    ICY,
    LAVA,
    METAL,
    ROCKY,
    TROPICAL
}

public class Planet : MonoBehaviour {
    public float radius;
    public float mass;
    public int health = 2;

    public bool beingHeld = false;
    public GodController lastHolder;

    private float invulnTime;

    private Transform texture;
    private Transform shade;
    private Vector3 origTextScale;
    private Vector3 origShadeScale;
    private SpriteRenderer sr;
    public Rigidbody2D rb;
    public CircleCollider2D cc;

    public PlanetType type;

    void Start() {
        sr = transform.Find("Texture").GetComponent<SpriteRenderer>();

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
        cc = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        updateVariables();
    }

    void Update() {
        invulnTime -= Time.deltaTime;
        if (health <= 0) {
            if (!beingHeld) {  // don't destroy if you are being held, god will do it
                Destroy(gameObject);
            }
        } else if (health == 1) {
            sr.color = Color.red;
        }

        // update planets based on their enum later?
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
}
