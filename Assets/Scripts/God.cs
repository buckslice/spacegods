using UnityEngine;
using System.Collections;

public enum Gods {
    ZEUS,
    POSEIDON,
    ANUBIS,
    THOR,
    ODIN,
    ATHENA,
    MICHAEL_JORDAN,
    CTHULU
}

public class God : MonoBehaviour {

    public Gods god;
    public float health = 100;
    public float maxHealth;
    public float maxSpeed;
    public float acceleration;
    public float throwStrength;
    public float counter;   // counter variable
    public bool special;    // event variable 

    // use this for initialization
    void Start() {
        maxHealth = health;
    }

    void Update() {
        counter += Time.deltaTime;
    }

}
