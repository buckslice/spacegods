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
        new string[] {"Zeus","Poseidon","Anubis","Thor"},
        new string[] {"Odin","Athena","Michael Jordan","Cthulu"}};

    // cooldown between joystick movements
    private float moveCooldown = .25f;

    // joystick has to be pushed at least this far in certain direction
    private float minMag = .5f;

    private List<Player> players = new List<Player>();
    private int len = 0;
    private bool findingLost = false;
    private float gameStartCountdown;
    private Text countDown;
    private GameObject overLay;
    private AudioSource menuMusic;

    // for keyboard testing
    private int keyboardPlayer1 = 0;
    private int keyboardPlayer2 = 0;

    // Use this for initialization
    void Start() {
        // clear previous player preferences
        PlayerPrefs.DeleteAll();
        menuMusic = GameObject.Find("Start Music").GetComponent<AudioSource>();
        len = 0;
        for (int i = 0; i < gods.Length; i++) {
            for (int j = 0; j < gods[i].Length; j++) {
                //Debug.Log(gods[i][j]);
                len++;
            }
        }

        // make sure it starts enabled in the inspector
        countDown = GameObject.Find("CountDown").GetComponent<Text>();
        overLay = GameObject.Find("Overlay");
        countDown.enabled = false;
        overLay.SetActive(false);

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

                // gods name text
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
        // if no joysticks it will have one entry ""
        string[] connectedJoysticks = Input.GetJoystickNames();
        // KEYBOARD TESTING MODE (this is so filthy dont look)
        if (connectedJoysticks.Length == 0 || (connectedJoysticks.Length == 1 && connectedJoysticks[0] == "")) {
            if (Input.GetKey(KeyCode.Backspace)) {
                string choice = Input.inputString;
                if (choice != "") {
                    if (keyboardPlayer1 <= 0 || keyboardPlayer1 > 8) {
                        int.TryParse(choice[0].ToString(), out keyboardPlayer1);
                    } else {
                        int.TryParse(choice[0].ToString(), out keyboardPlayer2);

                        if (keyboardPlayer2 > 0 && keyboardPlayer2 < 8) {
                            menuMusic.Stop();
                            Destroy(menuMusic.gameObject);
                            PlayerPrefs.DeleteAll();
                            PlayerPrefs.SetInt("Number of players", 2);
                            PlayerPrefs.SetInt("Player0 ", 1);
                            PlayerPrefs.SetInt("Player1 ", 2);

                            PlayerPrefs.SetString("Player1", images[keyboardPlayer1 - 1].name);
                            PlayerPrefs.SetString("Player2", images[keyboardPlayer2 - 1].name);
                            Application.LoadLevel("Main");
                        }
                    }
                }
            } else {    // reset if you let go
                keyboardPlayer1 = 0;
                keyboardPlayer2 = 0;
            }
        }

        // NORMAL JOYSTICK MODE
        bool playerNumChanged = false;
        for (int i = 0; i < connectedJoysticks.Length; i++) {
            if (connectedJoysticks[i] != "" && connectedJoysticks.Length > players.Count) {
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
                    //p.refreshAnchors(currentNumberOfPlayers);
                    players.Add(p);
                    playerNumChanged = true;
                }
            }
        }

        // if true it means a controller has disconnected
        // now we need to figure out which controller
        // dont boot player until all other players have moved
        if (players.Count > connectedJoysticks.Length) {
            if (!findingLost) {
                foreach (Player p in players) {
                    p.hasMovedRecently = false;
                }
                findingLost = true;
            } else {
                List<Player> afks = players.FindAll(p => !p.hasMovedRecently);
                if (afks.Count == 1) {
                    Player toRemove = afks[0];
                    players.Remove(toRemove);
                    Destroy(toRemove.img.gameObject);
                    findingLost = false;
                    playerNumChanged = true;
                }
            }
        }

        // if player number changed then resort player list based on id
        // then set anchors based on which players are present
        if (playerNumChanged) {
            players.Sort((p1, p2) => p1.id.CompareTo(p2.id));

            for (int i = 0; i < players.Count; i++) {
                players[i].calculateAnchors(i + 1, players.Count);
                players[i].refreshAnchors();
            }
        }

        if (players.Count > 0) {
            bool p1Cancel = Input.GetButtonDown("Cancel" + players[0].id);
            if (p1Cancel && players[0].chosen == "") {
                Application.LoadLevel("Menu");
            }
        }

        // need at least 2 players to start game
        bool allPlayersDecided = players.Count > 1;
        // process input for joysticks and move each player
        foreach (Player p in players) {

            if (Input.GetButtonDown("Submit" + p.id)) {
                p.setSelected(true);
                p.hasMovedRecently = true;
            }
            if (Input.GetButtonDown("Cancel" + p.id)) {
                p.setSelected(false);
                p.hasMovedRecently = true;
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
                        p.y = (len - 1 - p.x) / gods[0].Length;
                    }
                } else if (y < -minMag) {
                    p.y++;
                    if (p.y > (len - 1 - p.x) / gods[0].Length) {
                        p.y = 0;
                    }
                } else {
                    moved = false;
                }
                if (moved) {    // if successfully moved then set your parent and reset anchors
                    p.hasMovedRecently = true;
                    // change parent of player selector
                    if (p.parentName != gods[p.y][p.x]) {
                        p.setParent(images[p.y * gods[0].Length + p.x].transform);
                        p.refreshAnchors();
                    }

                    p.inputCooldown = Time.time + moveCooldown;
                }
            }

            if (p.chosen == "") {
                allPlayersDecided = false;
            }
        }

        if (allPlayersDecided) {
            // start countdown
            gameStartCountdown -= Time.deltaTime;
            countDown.enabled = true;
            countDown.text = "" + (int)(gameStartCountdown + 1);
            overLay.SetActive(true);
            menuMusic.volume = gameStartCountdown / 3f;

            if (gameStartCountdown < 0f) {
                menuMusic.Stop();
                Destroy(menuMusic.gameObject);
                // save all the players choices
                PlayerPrefs.SetInt("Number of players", players.Count);
                for (int i = 0; i < players.Count; i++) {
                    Player p = players[i];
                    //Debug.Log(p.id + " " + p.chosen);
                    PlayerPrefs.SetInt("Player" + i + " ", p.id);
                    PlayerPrefs.SetString("Player" + p.id, p.chosen);
                }
                Application.LoadLevel("Main");
            }
        } else {
            // stop countdown
            countDown.enabled = false;
            overLay.SetActive(false);
            gameStartCountdown = 3f;
            menuMusic.volume = 1f;
        }
    }
}

class Player {
    public int x = 0;
    public int y = 0;
    public string chosen = "";

    public float inputCooldown;
    public int id;
    public bool hasMovedRecently;

    public Image img;
    public Text txt;
    public Text btxt;
    public string parentName;

    private Vector2 relativeAnchor;

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

    public void calculateAnchors(int relativePosition, int numPlayers) {
        relativeAnchor = new Vector2(1f - (float)relativePosition / numPlayers, 1f - (float)(relativePosition - 1) / numPlayers);
    }

    public void refreshAnchors() {
        img.rectTransform.anchorMin = new Vector2(-0.1f, relativeAnchor.x);
        img.rectTransform.anchorMax = new Vector2(0f, relativeAnchor.y);
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
        btxt.rectTransform.anchoredPosition3D = new Vector3(btxtPos.x + 2, -27, btxtPos.z);

    }
}
