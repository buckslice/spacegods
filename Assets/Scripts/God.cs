using UnityEngine;
using UnityEngine.UI;

public enum GodType {
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
    ARTEMIS_APOLLO,
    JESUS,
    NIKE,
    HADES,
    KHONSU,
    MORRIGAN,
    KITSUNE
}

public enum GodStatus {
    NORMAL,
    DRUNK,
    FROZEN,
    POISONED
}

public class God : MonoBehaviour {
    // variables visible in inspector
    // need to be set in the prefab
    public GodType type;
    public float maxHealth;
    public float maxSpeed;
    public float acceleration;
    public float throwStrength;
    public float abilityCooldown;
    public Sprite idleSprite;
    public Sprite moveSprite;
    public Sprite attackSprite;
    /////////////////////////////////

    public GodStatus status { get; set; }
    public float startingThrowStrength { get; private set; }
    public float startingAcceleration { get; private set; }
    public float startingMaxSpeed { get; private set; }
    public bool special { get; set; }    // generic bool for god abilities
    public float coolDown { get; set; }  // generic counter for god abilities
    public float CCTimer { get; set; }   // time left on abnormal status
    private float invincible;   // tracks whether the god is immune to damage
    private float currentHealth;
    public ParticleSystem particles { get; private set; }
    private GodController controller;

    // animation stuff
    public bool moveInput { get; set; }
    private float attackAnimTime = 0f;

    // renderers
    public SpriteRenderer sr { get; set; }
    private SpriteRenderer[] frozenSrs;
    private SpriteRenderer drunkSr;
    private SpriteRenderer poisonedSr;

    // health bar
    private Vector3 screenPoint;
    private GameObject mainBar;
    private Image healthBar;
    private Image cooldownBar;
    private Image background;

    // Morrigan components
    private SpriteRenderer enragedSr;
    public CircleCollider2D auraCollider { get; private set; }

    public bool isKitsune { get; private set; }

    // use this for initialization
    void Start() {
        // fetch component references
        controller = GetComponent<GodController>();
        sr = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        Transform stateComponent = transform.Find("State");
        frozenSrs = stateComponent.Find("Frozen").GetComponentsInChildren<SpriteRenderer>();
        drunkSr = stateComponent.Find("Drunk").GetComponent<SpriteRenderer>();
        poisonedSr = stateComponent.Find("Poisoned").GetComponent<SpriteRenderer>();

        switch (type) {
            case GodType.MORRIGAN:
                sr = transform.Find("Sprite").Find("Normal").GetComponent<SpriteRenderer>();
                enragedSr = transform.Find("Sprite").Find("Enraged").GetComponent<SpriteRenderer>();
                particles = GetComponent<ParticleSystem>();
                auraCollider = gameObject.AddComponent<CircleCollider2D>();
                auraCollider.radius = 6f;
                auraCollider.isTrigger = true;
                auraCollider.enabled = false;
                break;
            case GodType.KITSUNE:
                isKitsune = true;
                break;
            case GodType.HADES:
            case GodType.ZEUS:
                particles = GetComponent<ParticleSystem>();
                break;
            default:
                break;
        }

        // settings variables
        status = GodStatus.NORMAL;
        currentHealth = maxHealth;
        throwStrength *= 2f;
        startingThrowStrength = throwStrength;
        startingAcceleration = acceleration;
        startingMaxSpeed = maxSpeed;
        invincible = CCTimer = 0f;
        coolDown = abilityCooldown;
        special = false;

        // the rest below is just UI bullshit
        /////////////////////////////////////
        mainBar = new GameObject(gameObject.name + " bars");
        mainBar.transform.parent = GameObject.Find("Canvas").transform;
        mainBar.transform.SetAsFirstSibling();

        GameObject backgroundGO = new GameObject(gameObject.name + " background");
        backgroundGO.transform.parent = mainBar.transform;
        background = backgroundGO.AddComponent<Image>();
        background.rectTransform.sizeDelta = new Vector2(100, 5);
        background.color = Color.grey;
        background.rectTransform.pivot = Vector2.zero;

        // cooldown bar if needed
        if (abilityCooldown != 0f) {
            GameObject cooldownGO = new GameObject(gameObject.name + " cooldownBar");
            cooldownGO.transform.parent = mainBar.transform;
            cooldownBar = cooldownGO.AddComponent<Image>();
            cooldownBar.color = Color.yellow;
            cooldownBar.rectTransform.pivot = Vector2.zero;
            background.rectTransform.sizeDelta = new Vector2(100, 10);
        }

        // healthbar
        GameObject healthGO = new GameObject(gameObject.name + " healthbar");
        healthGO.transform.parent = mainBar.transform;
        healthBar = healthGO.AddComponent<Image>();
        healthBar.rectTransform.pivot = Vector2.zero;

        setBars();
    }

    // make this late update maybe
    void Update() {
        coolDown -= Time.deltaTime;
        CCTimer -= Time.deltaTime;
        invincible -= Time.deltaTime;
        attackAnimTime -= Time.deltaTime;

        handleGodPassives();
        handleGodStates();
        setBars();
    }

    private void handleGodPassives() {
        switch (type) {
            case GodType.CTHULHU:
                if (coolDown < 0f) {
                    sr.color = Color.grey;
                } else {
                    sr.color = Color.white;
                }
                break;

            case GodType.ODIN:
                float r = currentHealth / maxHealth;
                throwStrength = startingThrowStrength * (3f - r * 2f);
                sr.color = new Color(1f, r, r);
                break;

            case GodType.ATHENA:
                if (currentHealth < maxHealth / 2f) {
                    changeHealth(3f * Time.deltaTime);
                }
                break;

            case GodType.MORRIGAN:
                if (coolDown < -10f) {
                    sr.enabled = true;
                    enragedSr.enabled = false;
                    resetCooldown();
                    auraCollider.enabled = false;
                    particles.Stop();
                    acceleration = startingAcceleration;
                    maxSpeed = startingMaxSpeed;
                } else if (coolDown < 0f) {
                    enragedSr.enabled = true;
                    sr.color = Color.white;
                    sr.enabled = false;
                    auraCollider.enabled = true;
                    if (!particles.isPlaying) {
                        particles.Play(false);
                    }
                    acceleration = startingAcceleration * 2f;
                    maxSpeed = startingMaxSpeed * 2f;
                } else {
                    sr.color = new Color(1f + coolDown / 30f, 1f + coolDown / 30f, 1f + coolDown / 30f);
                    particles.Stop();
                }
                break;
            default:
                break;
        }
    }

    private void handleGodStates() {
        if (CCTimer < 0f) {
            status = GodStatus.NORMAL;
        }

        switch (status) {
            case GodStatus.NORMAL:
                frozenSrs[0].enabled = false;
                frozenSrs[1].enabled = false;
                drunkSr.enabled = false;
                poisonedSr.enabled = false;
                if (Game.instance.hasBegun()) {
                    controller.freezeInputs = false;
                }
                break;
            case GodStatus.DRUNK:
                drunkSr.enabled = true;
                break;
            case GodStatus.FROZEN:
                frozenSrs[0].enabled = true;
                frozenSrs[1].enabled = true;
                controller.freezeInputs = true;
                break;
            case GodStatus.POISONED:
                poisonedSr.enabled = true;
                changeHealth(-controller.getVelocity().magnitude * Time.deltaTime * .5f, true);
                break;
        }

        // only do this if sprites are defined
        if (idleSprite && moveSprite && attackSprite) {
            if (attackAnimTime > 0f) {
                sr.sprite = attackSprite;
            } else if (moveInput) {
                sr.sprite = moveSprite;
            } else {
                sr.sprite = idleSprite;
            }
        }
    }

    public void setAttackTimer() {
        attackAnimTime = .5f;
    }

    public bool changeHealth(float diff, bool forced = false) {
        if (diff < 0f && !forced) {
            if (invincible > 0f) {
                return false;
            }
            invincible = .5f;
        }
        currentHealth += diff;
        checkForDeath();
        return true;
    }

    private void checkForDeath() {
        if (currentHealth <= 0 && !Game.instance.gameIsOver()) {
            if (type == GodType.HADES && coolDown < 0f) {
                resetCooldown();
                currentHealth = maxHealth / 4f;
                particles.Play(false);
                return;
            }
            Game.instance.removePlayer(controller); // remove player from list
            Destroy(mainBar);    // destroy healthbar GameObject
            Destroy(gameObject);        // destroy this GameObject
        }
        // clamp health if you are still alive
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    private void setBars() {
        screenPoint = Camera.main.WorldToScreenPoint(transform.transform.position);
        screenPoint.x -= 50f;
        screenPoint.y -= 700f / Camera.main.orthographicSize;
        healthBar.rectTransform.anchoredPosition = screenPoint;

        float cur = currentHealth;
        float max = maxHealth;

        healthBar.rectTransform.sizeDelta = new Vector2(cur / max * 100f, 5f);
        if (cur > max / 2f) {
            healthBar.color = Color.green;
        } else if (cur <= max / 2f && cur > max / 4f) {
            healthBar.color = Color.yellow;
        } else {
            healthBar.color = Color.red;
        }

        if (abilityCooldown != 0f) {
            cooldownBar.rectTransform.anchoredPosition = new Vector2(screenPoint.x, screenPoint.y - 5f);
            background.rectTransform.anchoredPosition = new Vector2(screenPoint.x, screenPoint.y - 5f);
            float cd = Mathf.Clamp01(1f - coolDown / abilityCooldown) * 100f;
            cooldownBar.rectTransform.sizeDelta = new Vector2(cd, 5f);
        } else {
            background.rectTransform.anchoredPosition = screenPoint;
        }
    }

    public void resetCooldown() {
        coolDown = abilityCooldown;
    }
}
