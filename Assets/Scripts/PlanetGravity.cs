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
        // resulting in many more collisions and a generally less stable system
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

//	private Rigidbody2D myRigidBody;
//	private float x, y, distance;
//	
//	// Use this for initialization
//	void Start () {
//		myRigidBody = GetComponent<Rigidbody2D>();
//		
//		//checks what quadrant the planet spawned in and gives it appropriate velocity
//		//Bottom left
//		if (myRigidBody.transform.position.x < 0 && myRigidBody.transform.position.y < 0) {
//			x = Random.Range(5.0f, 15.0f);
//			y = -6.0f;
//			
//			//Bottom Right
//		} else if (myRigidBody.transform.position.x > 0 && myRigidBody.transform.position.y < 0) {
//			x = 6.0f;
//			y = Random.Range (5.0f, 15.0f);
//			
//			//Top left
//		}
//		else if (myRigidBody.transform.position.x < 0 && myRigidBody.transform.position.y > 0)
//		{
//			x = -6.0f;
//			y = Random.Range (-15.0f, -5.0f);
//			
//			//Top Right
//		} else {
//			x = Random.Range(-15.0f, -5.0f);
//			y = 6.0f;
//		}
//	}
//	
//	// Update is called once per frame
//	void Update () {
//		//Distance from sun
//		distance = Mathf.Sqrt((myRigidBody.transform.position.x * myRigidBody.transform.position.x) + (myRigidBody.transform.position.y * myRigidBody.transform.position.y));
//		
//		//Deletes planet if it goes too far off screen
//		if (myRigidBody.transform.position.x <= -100f || myRigidBody.transform.position.x >= 100f
//		    || myRigidBody.transform.position.y <= -100f || myRigidBody.transform.position.y >= 100f)
//			DeletePlanet ();
//		
//		//Updates velocity and moves planet
//		updateVel (ref x, ref y, distance);
//		myRigidBody.transform.Translate(new Vector3(x, y, 0f) * Time.deltaTime);
//		
//		
//	}
//
//	void OnCollisionEnter2D(Collision2D collision) {
//		// kill planet if it hits sun
//		if (collision.gameObject.tag == "Sun") {
//			DeletePlanet();
//				}
//		}
//
//	//deletes planet and decrements class wide count
//	public void DeletePlanet()
//	{
//		Destroy (gameObject);
//		--PlanetSpawner.planetNum;
//	}
//	
//	//Uses newtons law to move velocity relative to sun
//	public void updateVel(ref float x, ref float y, float dist)
//	{
//		
//		float gravConst = 1f;
//		float sunMass = 50f, planetMass = 10f;
//		float force, acc;
//		
//		//avoids dividing by zero
//		if (dist == 0)
//			dist = .0000000000001f;
//		
//		//Newtons law
//		force = gravConst * ((sunMass * planetMass) / Mathf.Pow (dist, 2f));
//		
//		acc = force / planetMass;
//		x += transform.position.x * (acc / dist) * -1f;
//		y += transform.position.y * (acc / dist) * -1f;
//	}
}
