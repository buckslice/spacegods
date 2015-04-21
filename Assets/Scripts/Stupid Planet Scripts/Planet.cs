using UnityEngine;
using System.Collections;

public class Planet : MonoBehaviour {
    private int health = 2;
    public float radius;
    public float mass;
    // maybe have planetType enum here
    private SpriteRenderer sr;
    private float invulnTime;

    public planetType type;

    public enum planetType {
        BASKETBALL,
        GOLD,
        ICY,
        LAVA,
        METAL,
        ROCKY,
        TROPICAL
    }

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

        Transform texture = transform.Find("Texture").transform;
        texture.localScale = texture.localScale * radius;
        Transform shade = transform.Find("Shade").transform;
        shade.localScale = shade.localScale * radius;
        
        GetComponent<CircleCollider2D>().radius = radius;
        GetComponent<Rigidbody2D>().mass = mass;
    }

    void Update() {
        invulnTime -= Time.deltaTime;
        if (health <= 0) {
            Destroy(gameObject);
        } else if (health == 1) {
            sr.color = Color.red;
        }

        // update planets based on their enum later
    }

    public void damage() {
        if (invulnTime < 0f) {
            health--;
            invulnTime = 1f;
        }
    }
    public int getHealth() {
        return health;
    }
}
