using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(Rigidbody2D))]
public class PlanetGravity : MonoBehaviour {

    public float gravity = 1f;  // should make sun class eventually and move this there

    private Transform gravitationTarget;
    private Rigidbody2D myRigidBody;
    public AudioClip collision;
    public AudioClip explosion;

	public bool catchBool;

    void Start() {
		catchBool = true;
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
            --PlanetSpawner.planetNum;
            Destroy(gameObject);
        }
    }

	public void makeFalse() {
		Invoke ("makeTooFalse", .5f);
		}

	public void makeTooFalse() {
		catchBool = false;
		}

	public void makeTrue() {
		Invoke ("makeTooTrue", 10f);
		}
	public void makeTooTrue() {
		catchBool = true;
		}

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Planet") {
            // only want one of the planets to play the sound so base it off random factor like x position
            if (transform.position.x > collision.transform.position.x) {
                AudioManager.instance.playSound("Collision", transform.position, 1f);
            }
        } else if (collision.gameObject.tag == "Sun") { // kill planet if it hits sun
            AudioManager.instance.playSound("Explosion0", transform.position, .25f);
            --PlanetSpawner.planetNum;
            Destroy(gameObject);
        }
    }
}
