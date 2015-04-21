using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(Rigidbody2D))]
public class PlanetGravity : MonoBehaviour {

    public float gravity = 1f;  // should make sun class eventually and move this there

    private Transform gravitationTarget;
    private Rigidbody2D myRigidBody;
    public AudioClip collision;
    public AudioClip explosion;
    public PhysicsMaterial2D noBounce;

    public bool catchBool;

    void Start() {
        catchBool = true;
        myRigidBody = GetComponent<Rigidbody2D>();
        gravitationTarget = GameObject.Find("Sun").transform;

        // add some random velocity tangent to the direction of gravity
        Vector3 dir = (gravitationTarget.transform.position - myRigidBody.transform.position).normalized;

        // Random.Range(-1f,1f) instead of 1f at the end will make orbits go either way
        Vector3 tangent = Vector3.Cross(dir, new Vector3(0, 0, 1f));
		Vector3 halfway = (dir + tangent.normalized).normalized;
		halfway = (dir + halfway).normalized;
		Vector3 position = new Vector3 (Random.Range (Mathf.Min (halfway.x, tangent.x), Mathf.Max (halfway.x, tangent.x)),
		                               Random.Range (Mathf.Min (halfway.y, tangent.y), Mathf.Max (halfway.y, tangent.y)),
		                               Random.Range (Mathf.Min (halfway.z, tangent.z), Mathf.Max (halfway.z, tangent.z)));

        myRigidBody.velocity = Random.Range(10f, 15f) * position;

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
        // destroy planet if 100f away from sun or within 2f of the sun
		if ((gravitationTarget.position - transform.position).sqrMagnitude > 100 * 100){
            --PlanetSpawner.planetNum;
            Destroy(gameObject);
        }
        // change when hit sun collider
    }

    public void makeFalse() {
        Invoke("makeTooFalse", .001f);
    }
    public void makeTooFalse() {
        catchBool = false;
    }

    public void makeTrue() {
        Invoke("makeTooTrue", 5f);
    }
    public void makeTooTrue() {
        catchBool = true;
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Player") {
            myRigidBody.GetComponent<Collider2D>().sharedMaterial = noBounce;
        }

        if (collision.gameObject.tag == "Planet") {
            // only want one of the planets to play the sound so base it off random factor like x position
            if (transform.position.x > collision.transform.position.x) {
                //AudioManager.instance.playSound("Collision", transform.position, 1f);

            }

            gameObject.GetComponent<Planet>().damage();


        } else if (collision.gameObject.tag == "Sun") { // kill planet if it hits sun
            //AudioManager.instance.playSound("Explosion0", transform.position, .25f);
            --PlanetSpawner.planetNum;
            Destroy(gameObject);
        }
    }
}
