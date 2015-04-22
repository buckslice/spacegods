using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class God : MonoBehaviour 
{

    public enum godName {
        ZEUS,
        POSEIDON,
        ANUBIS,
        THOR,
        ODIN,
        ATHENA,
        MICHAEL_JORDAN,
        CTHULU
    }

    public godName myName;
    public int health = 100;
    private int maxHealth;
    public float maxSpeed = 100f;
    public float acceleration = 10f;
    public float throwStrength = 20f;
    private GodController controller;

    // use this for initialization
    void Start() 
	{
        controller = GetComponent<GodController>();
        maxHealth = health;
    }

    public int getMaxHealth() 
	{
        return maxHealth;
    }

    // update is called once per frame
    void Update() 
	{
        switch (myName) {
            case godName.MICHAEL_JORDAN:
                if (controller.planetScript != null) {
                    if (controller.planetScript.type == Planet.planetType.BASKETBALL) {
                        throwStrength = 60f;
                    } else {
                        throwStrength = 20f;
                    }
                }
                break;

            default:
                break;

        }
    }

    void FixedUpdate() 
	{

    }
}
