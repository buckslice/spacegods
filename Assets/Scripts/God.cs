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
    APHRODITE
}

public enum GodState {
    NORMAL,
    DRUNK,
    FROZEN,
    POISONED
}

public class God : MonoBehaviour {
    // variables set in prefab
    public GodType type;
    public float maxHealth;
    public float maxSpeed;
    public float acceleration;
    public float throwStrength;
    public bool special;

    public GodState state { get; set; }
    public float startingThrowStrength { get; private set; }
    public float coolDown { get; set; }  // generic variable for various god abilities
    public float CCTimer { get; set; }   // time left on abnormal state (another good WoW reference jeffrey lol)
    private float invincible;   // tracks whether the god is immune to damage
    private float currentHealth;
    private GodController controller;

    // renderers
    private SpriteRenderer sr;
    private SpriteRenderer[] frozenSrs;
    private SpriteRenderer drunkSr;
    private SpriteRenderer poisonedSr;

    // health bar
    private Vector3 screenPoint;
    private RawImage img;
    private Texture2D texture;

    // use this for initialization
    void Start() {
        // getting components
        controller = GetComponent<GodController>();
        sr = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        Transform stateComponent = transform.Find("State");
        frozenSrs = stateComponent.Find("Frozen").GetComponentsInChildren<SpriteRenderer>();
        drunkSr = stateComponent.Find("Drunk").GetComponent<SpriteRenderer>();
        poisonedSr = stateComponent.Find("Poisoned").GetComponent<SpriteRenderer>();

        // settings variables
        state = GodState.NORMAL;
        currentHealth = maxHealth;
        startingThrowStrength = throwStrength;
        coolDown = invincible = CCTimer = 0f;
        special = false;

        // health bar creation
        GameObject imgGO = new GameObject(gameObject.name + " healthbar");
        imgGO.transform.parent = GameObject.Find("Canvas").transform;
        imgGO.transform.SetAsFirstSibling();
        img = imgGO.AddComponent<RawImage>();
        img.rectTransform.anchorMin = Vector2.zero;
        img.rectTransform.anchorMax = Vector2.zero;
        img.rectTransform.offsetMin = new Vector2(0, 0);
        img.rectTransform.offsetMax = new Vector2(100, 5);
        texture = new Texture2D(100, 5);
        img.texture = texture;
        setHealthBar();
        moveHealthBar();
    }

    // make this late update maybe
    void Update() {
        coolDown -= Time.deltaTime;
        CCTimer -= Time.deltaTime;
        invincible -= Time.deltaTime;

        handleGodPassives();
        handleGodStates();
        moveHealthBar();
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
            case GodType.NIKE:
                if (Game.instance.players.Count > 1f) {
                    throwStrength = startingThrowStrength * Game.instance.numPlayers / (Game.instance.players.Count - 1);
                }
                break;
            default:
                break;
        }
    }

    private void handleGodStates() {
        if (CCTimer < 0f) {
            state = GodState.NORMAL;
        }

        switch (state) {
            case GodState.NORMAL:
                frozenSrs[0].enabled = false;
                frozenSrs[1].enabled = false;
                drunkSr.enabled = false;
                poisonedSr.enabled = false;
                if (Game.instance.hasBegun()) {
                    controller.freezeInputs = false;
                }
                break;
            case GodState.DRUNK:
                drunkSr.enabled = true;
                break;
            case GodState.FROZEN:
                frozenSrs[0].enabled = true;
                frozenSrs[1].enabled = true;
                controller.freezeInputs = true;
                break;
            case GodState.POISONED:
                poisonedSr.enabled = true;
                changeHealth(-controller.getSpeed() * Time.deltaTime, true);
                break;
        }
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
        setHealthBar();
        return true;
    }

    private void checkForDeath() {
        if (currentHealth <= 0 && !Game.instance.gameIsOver()) {
            if(type == GodType.HADES && coolDown < 0f) {
                coolDown = 60f;
                currentHealth = maxHealth / 5f;
                return;
            }
            Game.instance.removePlayer(controller); // remove player from list
            Destroy(img.gameObject);    // destroy healthbar GameObject
            Destroy(gameObject);        // destroy this GameObject
        }
        // clamp health if you are still alive
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    private void moveHealthBar() {
        screenPoint = Camera.main.WorldToScreenPoint(transform.transform.position);
        screenPoint.y -= 700f / Camera.main.orthographicSize;
        img.rectTransform.anchoredPosition = screenPoint;
    }

    private void setHealthBar() {
        Color32[] pixels = new Color32[500];
        float cur = currentHealth;
        float max = maxHealth;
        Color32 color;

        if (cur > max / 2f) {
            color = Color.green;
        } else if (cur <= max / 2f && cur > max / 4f) {
            color = Color.yellow;
        } else {
            color = Color.red;
        }

        for (int i = 0; i < pixels.Length; i++) {
            pixels[i] = i >= 100 ? pixels[i - 100] : cur / max * 100 > i ? color : (Color32)Color.gray;
        }

        texture.SetPixels32(pixels);
        texture.Apply();
    }

}
