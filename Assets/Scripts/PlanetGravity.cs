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

        // Random.Range(-1f,1f) instead of 1f at the end will make orbits go either way
        Vector3 tangent = Vector3.Cross(dir, new Vector3(0, 0, 1f));
        
        myRigidBody.velocity = 10f * tangent;

    }

    void FixedUpdate() {
        // less realistic constant gravity (gravity constant was 4)
        //Vector3 g = (gravitationTarget.transform.position - myRigidBody.transform.position).normalized * gravity;

        // more realistic gravity (scales with distance)
        Vector3 dist = (gravitationTarget.position - transform.position) / 10f;
        Vector3 g = Mathf.Max(gravity / dist.sqrMagnitude, 1f) * dist.normalized;

        myRigidBody.AddForce(g * myRigidBody.mass);
    }

    void Update() {
        // destroy planet if 100f away from sun
        if ((gravitationTarget.position - transform.position).sqrMagnitude > 100 * 100) {
            Destroy(gameObject);
            --PlanetSpawner.planetNum;
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        // kill planet if it hits sun
        if (collision.gameObject.tag == "Sun") {
            Destroy(gameObject);
            --PlanetSpawner.planetNum;
        }
    }
}
