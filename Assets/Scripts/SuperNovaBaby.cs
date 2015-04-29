using UnityEngine;
using System.Collections;

public class SuperNovaBaby : MonoBehaviour {

    public float timeUntilShitGetsReal = 60f;
    private ParticleSystem system;
    private Transform sprite;
    private CircleCollider2D circCol;
    private float growth = 0f;
    // Use this for initialization
    void Start() {
        circCol = GetComponent<CircleCollider2D>();
        system = GetComponent<ParticleSystem>();
        sprite = transform.Find("sprite").transform;
    }

    // Update is called once per frame
    void Update() {
		SuperNova ();
    }

	private void SuperNova(){
		sprite.Rotate(0, 0, Time.deltaTime * 5f);
		growth += Time.deltaTime;
		float rate = Mathf.Pow(1.1f, growth - timeUntilShitGetsReal);
		transform.localScale = Vector3.one + Vector3.one * rate;
		circCol.radius = 3f - (.5f / (1f + rate));
		
		system.startLifetime = 5f + rate;
		system.startSpeed = .5f + rate;
		system.startSize = 5f + rate * 2f;
	}
}
