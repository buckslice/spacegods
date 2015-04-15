using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviour {


    //private string gods[]
    public Sprite[] godSprites;
    public GameObject[] images;

    public Sprite nameBackground;
    public Sprite playerSprite;
    public Font font;
    public Font playerFont;

    private string[][] gods = new string[2][] {
        new string[] {"Zeus",  "Poseidon", "Anubis", "Thor"},
        new string[] {"Odin",  "Athena",   "Michael Jordan", "Cthulu"}};

    // cooldown between joystick movements
    private float moveCooldown = .20f;

    // joystick has to be pushed at least this far in certain direction
    private float minMag = .5f;

    private List<Player> players = new List<Player>();
    private int len = 0;

    // Use this for initialization
    void Start() {
        len = 0;
        for (int i = 0; i < gods.Length; i++) {
            for (int j = 0; j < gods[i].Length; j++) {
                Debug.Log(gods[i][j]);
                len++;
            }
        }

        images = new GameObject[len];
        int xL = gods.Length;
        for (int y = 0; y < xL; y++) {
            int yL = gods[0].Length;
            for (int x = 0; x < yL; x++) {
                if (x >= gods[y].Length) {
                    break;
                }
                // add gods image
                GameObject imgGO = new GameObject();
                images[y * yL + x] = imgGO;
                imgGO.name = gods[y][x];
                imgGO.transform.parent = gameObject.transform;
                Image img = imgGO.AddComponent<Image>();
                if (godSprites != null && godSprites.Length > y * yL + x) {
                    img.sprite = godSprites[y * yL + x];
                }
                //img.preserveAspect = true;
                float p = .05f;
                float padX = (1f - p * (yL + 1)) / yL;
                float padY = (.96f - p * (xL + 1)) / xL;    // offset a little to give more room on bottom
                img.rectTransform.anchorMin = new Vector2(x * padX + p * (x + 1), 1f - (y + 1) * padY - p * (y + 1));
                img.rectTransform.anchorMax = new Vector2((x + 1) * padX + p * (x + 1), 1f - y * padY - p * (y + 1));
                img.rectTransform.offsetMin = Vector2.zero;
                img.rectTransform.offsetMax = Vector2.zero;

                // background for gods name text
                GameObject minipanel = new GameObject("panel");
                minipanel.transform.parent = imgGO.transform;
                Image textBg = minipanel.AddComponent<Image>();
                textBg.type = Image.Type.Sliced;
                textBg.sprite = nameBackground;
                textBg.rectTransform.anchorMin = Vector2.zero;
                textBg.rectTransform.anchorMax = new Vector2(1f, 0f);
                textBg.rectTransform.offsetMin = Vector2.zero;
                textBg.rectTransform.offsetMax = new Vector2(0, 50);
                Vector3 bgPos = textBg.rectTransform.anchoredPosition3D;
                textBg.rectTransform.anchoredPosition3D = new Vector3(bgPos.x, 0, bgPos.z);

                // displays gods name
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
                txt.rectTransform.offsetMin = Vector2.zero;
                txt.rectTransform.offsetMax = new Vector2(0, 50);
                Vector3 txtPos = txt.rectTransform.anchoredPosition3D;
                txt.rectTransform.anchoredPosition3D = new Vector3(txtPos.x, 0, txtPos.z);

            }
        }
    }

    // Update is called once per frame
    void Update() {

        // check for newly connected joysticks
        // for pretending likes theres more joysticks
        //string[] connectedJoysticks = new string[] { "hi", "hi", "hi", "hi" };
        string[] connectedJoysticks = Input.GetJoystickNames();
        int currentNumberOfPlayers = connectedJoysticks.Length;
        bool playerNumChanged = false;
        for (int i = 0; i < connectedJoysticks.Length; i++) {
            if (connectedJoysticks[i] != "") {
                // check to see if connected joystick is new
                bool isNew = true;
                foreach (Player p in players) {
                    if (p.id == i + 1) {
                        isNew = false;
                        break;
                    }
                }

                // add new player if new joystick is found
                if (isNew) {
                    Player p = new Player(i + 1, images[0].transform, playerFont, playerSprite);
                    p.refreshAnchors(currentNumberOfPlayers);
                    players.Add(p);
                    playerNumChanged = true;
                }
            }
        }

        // remove players who have disconnected
        List<Player> toRemove = players.FindAll(p => p.id > connectedJoysticks.Length || connectedJoysticks[p.id - 1] == "");
        playerNumChanged = toRemove.Count > 0 ? true : playerNumChanged;
        foreach (Player p in toRemove) {
            players.Remove(p);
            Destroy(p.img.gameObject);
        }

        // process input for joysticks and move them
        foreach (Player p in players) {

            if (Input.GetButtonDown("Submit" + p.id)) {
                p.setSelected(true);
            }
            if (Input.GetButtonDown("Cancel" + p.id)) {
                p.setSelected(false);
            }

            // check to see if player is allowed to move again
            if (p.inputCooldown < Time.time && p.chosen == "") {
                float x = Input.GetAxis("Horizontal_360_" + p.id);
                float y = Input.GetAxis("Vertical_360_" + p.id);
                bool moved = true;
                if (x > minMag) {
                    p.x++;
                    if (p.x >= gods[p.y].Length) {
                        p.x = 0;
                    }
                } else if (x < -minMag) {
                    p.x--;
                    if (p.x < 0) {
                        p.x = gods[p.y].Length - 1;
                    }
                } else if (y > minMag) {
                    p.y--;
                    if (p.y < 0) {
                        p.y = (len - p.x) / gods[0].Length;
                    }
                } else if (y < -minMag) {
                    p.y++;
                    if (p.y > (len - p.x) / gods[0].Length) {
                        p.y = 0;
                    }
                } else {
                    moved = false;
                }
                if (moved) {
                    // change parent of player selector
                    if (p.parentName != gods[p.y][p.x]) {
                        p.setParent(images[p.y * gods[0].Length + p.x].transform);
                        p.refreshAnchors(currentNumberOfPlayers);
                    }

                    p.inputCooldown = Time.time + moveCooldown;
                }
            }

            if (playerNumChanged) {
                p.refreshAnchors(currentNumberOfPlayers);
            }
        }

    }
}

class Player {
    public int x = 0;
    public int y = 0;
    public string chosen = "";

    public float inputCooldown;
    public int id;  //starting at 1

    public Image img;
    public Text txt;
    public Text btxt;
    public string parentName;

    // should probably figure out a nicer color palette lol
    private static Color[] colors = new Color[] { 
        Color.red, Color.yellow, Color.green, Color.blue, Color.magenta, Color.cyan, Color.grey, Color.black };

    public Player(int id, Transform parent, Font f, Sprite sprite) {
        this.id = id;

        // sprite image
        img = new GameObject("Player " + id).AddComponent<Image>();
        img.sprite = sprite;
        img.color = colors[id - 1];
        img.type = Image.Type.Sliced;

        // text that indicates player number
        txt = new GameObject("Player " + id + " text").AddComponent<Text>();
        txt.font = f;
        txt.text = "P" + id;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.resizeTextForBestFit = true;
        txt.resizeTextMinSize = 10;
        txt.resizeTextMaxSize = 50;

        // black background text to add contrast
        btxt = new GameObject("Player " + id + " btext").AddComponent<Text>();
        btxt.color = Color.black;
        btxt.font = f;
        btxt.text = "P" + id;
        btxt.alignment = TextAnchor.MiddleCenter;
        btxt.resizeTextForBestFit = true;
        btxt.resizeTextMinSize = 10;
        btxt.resizeTextMaxSize = 50;

        setSelected(false);
        setParent(parent);
    }

    public void setSelected(bool b) {
        img.color = new Color(img.color.r, img.color.g, img.color.b, b ? 1 : .3f);
        img.fillCenter = b;
        chosen = b ? parentName : "";
    }

    public void setParent(Transform parent) {
        img.transform.SetParent(parent);
        btxt.transform.SetParent(img.transform);
        txt.transform.SetParent(img.transform);

        parentName = parent.gameObject.name;
    }

    public void refreshAnchors(int numPlayers) {
        img.rectTransform.anchorMin = new Vector2(-.1f, 1f - (float)id / numPlayers);
        img.rectTransform.anchorMax = new Vector2(0f, 1f - (float)(id - 1) / numPlayers);
        img.rectTransform.offsetMin = Vector2.zero;
        img.rectTransform.offsetMax = Vector3.zero;

        txt.rectTransform.anchorMin = new Vector3(0f, 1f);
        txt.rectTransform.anchorMax = new Vector3(1f, 1f);
        txt.rectTransform.offsetMin = Vector2.zero;
        txt.rectTransform.offsetMax = new Vector2(0, 50);

        Vector3 txtPos = txt.rectTransform.anchoredPosition3D;
        txt.rectTransform.anchoredPosition3D = new Vector3(txtPos.x, -25, txtPos.z);

        btxt.rectTransform.anchorMin = new Vector3(0f, 1f);
        btxt.rectTransform.anchorMax = new Vector3(1f, 1f);
        btxt.rectTransform.offsetMin = Vector2.zero;
        btxt.rectTransform.offsetMax = new Vector2(0, 50);

        Vector3 btxtPos = txt.rectTransform.anchoredPosition3D;
        btxt.rectTransform.anchoredPosition3D = new Vector3(btxtPos.x+2, -27, btxtPos.z);

    }
}
