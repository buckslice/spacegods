using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlanetGravity : MonoBehaviour {

    public float gravity = 1f;  // should make sun class eventually and move this there

    private Transform gravitationTarget;
    private Rigidbody2D myRigidBody;

    void Start() {
        myRigidBody = GetComponent<Rigidbody2D>();
        gravitationTarget = GameObject.Find("Sun").transform;

        // add some random velocity tangent to the direction of gravity
        Vector3 dir = (gravitationTarget.transform.position - myRigidBody.transform.position).normalized;
        Vector3 tangent = Vector3.Cross(dir, new Vector3(0, 0, Random.Range(-1f, 1f)));
        
        myRigidBody.velocity = 10f * tangent;

    }

    void Update() {
        // add gravity force towards sun
        Vector3 g = (gravitationTarget.transform.position - myRigidBody.transform.position).normalized * gravity;
        myRigidBody.AddForce(g * myRigidBody.mass);
    }

    void OnCollisionEnter2D(Collision2D collision) {
        // kill planet if it hits sun
        if (collision.gameObject.tag == "Sun") {
            Destroy(gameObject);
            --PlanetSpawner.planetNum;
        }
    }
}
