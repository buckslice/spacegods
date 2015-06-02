using UnityEngine;
using System.Collections;

public class SuperNovaBaby : MonoBehaviour {
    public float timeUntilShitGetsReal = 60f;
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
        SuperNova();

        if (targetRadius > PlanetSpawner.current.boundaryRadius * maxSize) {
            targetRadius = PlanetSpawner.current.boundaryRadius * maxSize;
        }

        circCol.radius = Mathf.Lerp(circCol.radius, targetRadius, Time.deltaTime);

        sprite.Rotate(0, 0, Time.deltaTime * 5f);
        sprite.localScale = Vector3.one * (circCol.radius / 3f + .2f);

        system.maxParticles = (int)(2500f / circCol.radius);
        system.emissionRate = (system.maxParticles / system.startLifetime);
        system.startSpeed = circCol.radius / 2f;
        system.startSize = circCol.radius * 2f;
    }

    private void SuperNova() {
        growth += Time.deltaTime;
        float rate = Mathf.Pow(1.1f, growth - timeUntilShitGetsReal);
        if (rate > 1.1f) {
            targetRadius += rate * Time.deltaTime;
        }
        if (Time.time > timeUntilShitGetsReal) {
            maxSize = 1.1f;
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "Planet" && Time.time < timeUntilShitGetsReal) {
            CircleCollider2D planetCol = col.gameObject.GetComponent<CircleCollider2D>();
            targetRadius += planetCol.radius / 2f;
        }
    }
}
