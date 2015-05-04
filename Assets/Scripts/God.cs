using UnityEngine;

public enum Gods {
    ZEUS,
    POSEIDON,
    ANUBIS,
    THOR,
    ODIN,
    ATHENA,
    MICHAEL_JORDAN,
    CTHULHU,
    HERMES,
    SHIVA,
    SUN_WUKONG,
    QUETZALCOATL,
    ARTEMIS_APOLLO
}

public class God : MonoBehaviour {
    public Gods god;
    public float maxHealth;
    private float currentHealth;
    public float maxSpeed;
    public float acceleration;
    public float throwStrength;
    private float startingThrowStrength;
    private float dot;
    private float counter;   // counter variable
    public bool special;     // event variable 

    // use this for initialization
    void Start() {
        currentHealth = maxHealth;
        startingThrowStrength = throwStrength;
        counter = 0.0f;
        dot = 0.0f;
    }

    void Update() {
        counter += Time.deltaTime;
        normalizeHealth();
        applyDot();
    }
    public void changeHealth(float damage) {
        currentHealth -= damage;
    }
    public void dotDamage(float damage) {
        dot = damage;
    }
    public float getCurrentHealth() {
        return currentHealth;
    }

    public void resetCounter() {
        counter = 0.0f;
    }
    public float getCounter() {
        return counter;
    }

    public float getStartingThrowStrength() {
        return startingThrowStrength;
    }

    private void normalizeHealth() {
        if (currentHealth < 0) {
            currentHealth = 0;
        }
        if (currentHealth > maxHealth) {
            currentHealth = maxHealth;
        }
    }

    private void applyDot() {
        if (dot > 0.0f) {
            currentHealth -= dot * Time.deltaTime;
            dot -= Time.deltaTime;
        }
    }
}
