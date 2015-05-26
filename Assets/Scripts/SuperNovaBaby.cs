using UnityEngine;
using System.Collections;

public class SuperNovaBaby : MonoBehaviour {
    [Range(.1f, 1f)]
    public float maxSize = .5f;   // in relation to level boundary

    private ParticleSystem system;
    private Transform sprite;
    private CircleCollider2D circCol;
    private float growth;
    private float targetRadius;

    void Start() {
        growth = 0f;
        circCol = GetComponent<CircleCollider2D>();
        system = GetComponent<ParticleSystem>();
        sprite = transform.Find("sprite").transform;
        targetRadius = circCol.radius;
    }

    // Update is called once per frame
    void Update() {
        sprite.Rotate(0, 0, Time.deltaTime * 5f);
        circCol.radius = Mathf.Lerp(circCol.radius, targetRadius, Time.deltaTime);
        sprite.localScale = Vector3.one * (circCol.radius / 3f + .2f);
        system.maxParticles = (int)(2500f / circCol.radius);
        system.emissionRate = (system.maxParticles / system.startLifetime);
        system.startSpeed = circCol.radius / 2f;
        system.startSize = circCol.radius * 2f;
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "Planet") {
            CircleCollider2D planetCol = col.gameObject.GetComponent<CircleCollider2D>();
            targetRadius += planetCol.radius / 2f;
            if (targetRadius > PlanetSpawner.current.boundaryRadius * maxSize) {
                targetRadius = PlanetSpawner.current.boundaryRadius * maxSize;
            }
        }
    }
}
