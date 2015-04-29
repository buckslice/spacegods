using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviour {

    //private string gods[]
    public Sprite[] godSprites;
    private GodGameObject[] godGameObjects;

    public Sprite nameBackground;
    public Sprite playerSprite;
    public Font font;
    public Font playerFont;

    private string[][] gods = new string[2][] {
        new string[] {"Zeus","Poseidon","Anubis","Thor"},
        new string[] {"Odin","Athena","Michael Jordan","Cthulhu"}
    };

    private string[] godInfo = new string[]{
        "Charges planets the longer he holds them.",
        "Freezes players with thrown ice planets.",
        "Can throw planets through the sun.",
        "Deflects a shot every 10 seconds.",
        "Strength increases as health decreases.",
        "Slowly regenerates health.",
        "Find the basketballs. Show them how to slam.",
        "can't throw. extra health. extra mass. damages players on collision."
    };

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
    private bool usingKeyboard;

    // Use this for initialization
    void Start() {
        // clear previous player preferences
        PlayerPrefs.DeleteAll();
        GameObject menuMusicObject = GameObject.Find("Start Music");
        if (!menuMusicObject) {
            GameObject startMusicGO = (GameObject)Instantiate(Resources.Load("Start Music"));
            startMusicGO.name = "Start Music";
            menuMusic = startMusicGO.GetComponent<AudioSource>();
        } else {
            menuMusic = menuMusicObject.GetComponent<AudioSource>();
        }
        len = 0;
        for (int i = 0; i < gods.Length; i++) {
            for (int j = 0; j < gods[i].Length; j++) {
                //Debug.Log(gods[i][j]);
                len++;
            }
        }

        // make sure it starts enabled in the inspector (can't Find disabled objects (thanks unity (thanks obama)))
        countDown = GameObject.Find("CountDown").GetComponent<Text>();
        overLay = GameObject.Find("Overlay");
        countDown.enabled = false;
        overLay.SetActive(false);

        godGameObjects = new GodGameObject[len];
        int xL = gods.Length;
        for (int y = 0; y < xL; y++) {
            int yL = gods[0].Length;
            for (int x = 0; x < yL; x++) {
                if (x >= gods[y].Length) {
                    break;
                }
                int godCoord1D = y * yL + x;

                GodGameObject thisGod = new GodGameObject();
                godGameObjects[godCoord1D] = thisGod;

                // add gods image
                GameObject imgGO = new GameObject();
                imgGO.name = gods[y][x];
                imgGO.transform.parent = gameObject.transform;
                thisGod.go = imgGO;
                Image img = imgGO.AddComponent<Image>();
                thisGod.image = img;
                if (godSprites != null && godSprites.Length > godCoord1D) {
                    img.sprite = godSprites[godCoord1D];
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

                // gods info text
                GameObject infoGO = new GameObject("info");
                thisGod.info = infoGO;
                infoGO.transform.parent = imgGO.transform;
                Text info = infoGO.AddComponent<Text>();
                info.text = godInfo[godCoord1D];
                info.font = font;
                info.color = new Color32(200, 255, 255, 255);
                info.rectTransform.anchorMin = Vector2.zero;
                info.rectTransform.anchorMax = Vector2.one;
                info.rectTransform.offsetMax = Vector2.zero;
                info.rectTransform.offsetMin = new Vector2(0f, 25);
                info.resizeTextForBestFit = true;
                info.resizeTextMinSize = 10;
                info.resizeTextMaxSize = 30;
                infoGO.SetActive(false);

            }
        }

        // add two players for keyboard mode if no controllers are connected
        string[] connectedJoysticks = Input.GetJoystickNames();
        if (connectedJoysticks.Length == 0 || (connectedJoysticks.Length == 1 && connectedJoysticks[0] == "")) {
            usingKeyboard = true;
            players.Add(new Player(1, godGameObjects[0].go.transform, playerFont, playerSprite));
            players.Add(new Player(2, godGameObjects[0].go.transform, playerFont, playerSprite));

            players[0].calculateAnchors(1, 2);
            players[0].refreshAnchors();
            players[1].calculateAnchors(2, 2);
            players[1].refreshAnchors();
            moveCooldown = .15f;
        }
    }

    // Update is called once per frame
    void Update() {
        // check for newly connected joysticks
        string[] connectedJoysticks = Input.GetJoystickNames();

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
                    Player p = new Player(i + 1, godGameObjects[0].go.transform, playerFont, playerSprite);
                    //p.refreshAnchors(currentNumberOfPlayers);
                    players.Add(p);
                    playerNumChanged = true;
                }
            }
        }

        // if true it means a controller has disconnected
        // now we need to figure out which controller
        // dont boot player until all other players have moved
        if (players.Count > connectedJoysticks.Length && !usingKeyboard) {
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
        if (playerNumChanged && !usingKeyboard) {
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
            int curRowLength = gods[p.y].Length;
            int regRowLength = gods[0].Length;

            if (usingKeyboard && Input.GetButtonDown("Fire" + p.id)) {
                p.setSelected(p.chosen == "");
            } else {
                if (Input.GetButtonDown("Submit" + p.id)) {
                    p.setSelected(true);
                    p.hasMovedRecently = true;
                }
                if (Input.GetButtonDown("Cancel" + p.id)) {
                    p.setSelected(false);
                    p.hasMovedRecently = true;
                }
            }

            if (Input.GetButton("Y" + p.id) || Input.GetKey(KeyCode.Y)) {
                godGameObjects[p.x + p.y * regRowLength].checkingInfo = true;
            }

            // check to see if player is allowed to move again
            if (p.inputCooldown < Time.time && p.chosen == "") {
                float x = Input.GetAxis("Horizontal" + (usingKeyboard ? "" : "_360_") + p.id);
                float y = Input.GetAxis("Vertical" + (usingKeyboard ? "" : "_360_") + p.id);
                bool moved = true;
                if (x > minMag) {
                    p.x++;
                    if (p.x >= curRowLength) {
                        p.x = 0;
                    }
                } else if (x < -minMag) {
                    p.x--;
                    if (p.x < 0) {
                        p.x = curRowLength - 1;
                    }
                } else if (y > minMag) {
                    p.y--;
                    if (p.y < 0) {
                        p.y = (len - 1 - p.x) / regRowLength;
                    }
                } else if (y < -minMag) {
                    p.y++;
                    if (p.y > (len - 1 - p.x) / regRowLength) {
                        p.y = 0;
                    }
                } else {
                    moved = false;
                }
                if (moved) {    // if successfully moved then set your parent and reset anchors
                    p.hasMovedRecently = true;
                    // change parent of player selector
                    if (p.parentName != gods[p.y][p.x]) {
                        p.setParent(godGameObjects[p.y * regRowLength + p.x].go.transform);
                        p.refreshAnchors();
                    }

                    p.inputCooldown = Time.time + moveCooldown;
                }
            }

            if (p.chosen == "") {
                allPlayersDecided = false;
            }
        }

        // update info checking
        for (int i = 0; i < godGameObjects.Length; i++) {
            godGameObjects[i].update();
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
                PlayerPrefs.DeleteAll();
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

    // should have extended event system probably, but this works good enough
    public float inputCooldown;

    public int id;
    public bool hasMovedRecently;

    public Image img;
    public Text txt;
    public string parentName;

    private Vector2 relativeAnchor;

    // should probably use a nicer color palette lol
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
        //txt.color = colors[id - 1];
        txt.alignment = TextAnchor.MiddleCenter;
        txt.resizeTextForBestFit = true;
        txt.resizeTextMinSize = 10;
        txt.resizeTextMaxSize = 50;

        setSelected(false);
        setParent(parent);
    }

    public void setSelected(bool b) {
        img.color = new Color(img.color.r, img.color.g, img.color.b, b ? 1 : .3f);
        img.fillCenter = b;
        //txt.color = b ? Color.white : colors[id - 1];
        chosen = b ? parentName : "";
    }

    public void setParent(Transform parent) {
        img.transform.SetParent(parent);
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
    }
}

// small class to hold and save some variables to avoid GetComponent calls
class GodGameObject {
    public GameObject go;
    public Image image;
    public GameObject info;
    public bool checkingInfo = false;

    public void update() {
        if (checkingInfo) {
            image.color = new Color(.3f, .3f, .3f, 1f);
            info.SetActive(true);
        } else {
            image.color = Color.white;
            info.SetActive(false);
        }

        checkingInfo = false;
    }

}
