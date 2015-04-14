using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviour {


    //private string gods[]
    public Sprite[] godSprites;
    public Sprite background;
    public Font font;
    private string[][] gods = new string[2][] {
        new string[] {"Zeus",  "Poseidon", "Anubis", "Thor"},
        new string[] {"Odin",  "Athena",   "Michael Jordan", "Cthulu"}};
        //new string[] {"bucky"}};

    private Vector2[] cursors;  // too bad theres no Vector2i

    // Use this for initialization
    void Start() {
        //int numberOfJoysticks = Input.GetJoystickNames().Length;

        for (int i = 0; i < gods.Length; i++) {
            for (int j = 0; j < gods[i].Length; j++) {
                Debug.Log(gods[i][j]);
            }
        }

        int xL = gods.Length;
        for (int y = 0; y < xL; y++) {
            int yL = gods[0].Length;
            for (int x = 0; x < yL; x++) {
                if (x >= gods[y].Length) {
                    break;
                }
                GameObject imgGO = new GameObject();
                imgGO.name = gods[y][x];
                imgGO.transform.parent = gameObject.transform;
                Image img = imgGO.AddComponent<Image>();
                if (godSprites != null && godSprites.Length > y * yL + x) {
                    img.sprite = godSprites[y * yL + x];
                }
                //img.preserveAspect = true;
                float p = .05f;
                float padX = (1f - p * (yL + 1)) / yL;
                float padY = (.95f - p * (xL + 1)) / xL;
                img.rectTransform.anchorMin = new Vector2(x * padX + p * (x + 1), 1f - (y + 1) * padY - p * (y + 1));
                img.rectTransform.anchorMax = new Vector2((x + 1) * padX + p * (x + 1), 1f - y * padY - p * (y + 1));
                img.rectTransform.offsetMin = Vector2.zero;
                img.rectTransform.offsetMax = Vector2.zero;

                GameObject minipanel = new GameObject("panel");
                minipanel.transform.parent = imgGO.transform;
                Image mp = minipanel.AddComponent<Image>();
                mp.type = Image.Type.Sliced;
                mp.sprite = background;
                mp.rectTransform.anchorMin = Vector2.zero;
                mp.rectTransform.anchorMax = new Vector2(1f, 0f);
                mp.rectTransform.offsetMin = Vector2.zero;
                mp.rectTransform.offsetMax = new Vector2(0, 50);
                Vector3 mpPos = mp.rectTransform.anchoredPosition3D;
                mp.rectTransform.anchoredPosition3D = new Vector3(mpPos.x, 0, mpPos.z);

                GameObject txtGO = new GameObject("text");
                txtGO.transform.parent = imgGO.transform;
                Text txt = txtGO.AddComponent<Text>();
                txt.text = gods[y][x];
                txt.font = font;
                txt.color = new Color(1f, 1f, .6f);
                txt.rectTransform.anchorMin = Vector2.zero;
                txt.rectTransform.anchorMax = new Vector2(1f, 0f);
                txt.alignment = TextAnchor.MiddleCenter;
                txt.resizeTextForBestFit = true;
                txt.resizeTextMinSize = 10;
                txt.resizeTextMaxSize = 100;
                txt.rectTransform.offsetMax = new Vector2(0, 50);
                txt.rectTransform.offsetMin = Vector2.zero;
                Vector3 txtPos = txt.rectTransform.anchoredPosition3D;
                txt.rectTransform.anchoredPosition3D = new Vector3(txtPos.x, 0, txtPos.z);

            }
        }
    }

    // Update is called once per frame
    void Update() {

    }
}
