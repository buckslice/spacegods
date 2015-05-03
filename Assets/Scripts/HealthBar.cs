using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
    private God god;
    private Vector3 screenPoint;
    private RawImage img;
    private Texture2D texture;

    // use this for initialization
    void Start() {
        god = GetComponent<God>();

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
    }

    // update is called once per frame
    void Update() {
        screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        screenPoint.y -= 700f / Camera.main.orthographicSize;
        img.rectTransform.anchoredPosition = screenPoint;

        Color32[] pixels = new Color32[500];
        float cur = god.getCurrentHealth();
        float max = god.maxHealth;
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

    public void deleteGameObject() {
        Destroy(img.gameObject);
    }


}
