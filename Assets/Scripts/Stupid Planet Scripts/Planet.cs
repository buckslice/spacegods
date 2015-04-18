using UnityEngine;
using System.Collections;

public class Planet : MonoBehaviour {
    private int health = 2;
    private float radius;
    private float mass;
    // maybe have planetType enum here

    void Start() {
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
        if (health <= 0) {
            Destroy(gameObject);
        }

        // update planets based on their enum later
    }

    public void damage() {
        health--;
    }
    public int getHealth() {
        return health;
    }
}
