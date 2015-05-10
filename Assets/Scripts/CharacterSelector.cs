using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviour {

    //private string gods[]
    public Sprite[] godSprites;
    public AudioClip[] godSounds;
    private GodGameObject[] godGameObjects;

    public Sprite nameBackground;
    public Sprite playerSprite;
    public Font font;
    public Font playerFont;

    // all caps text cuz new font is kinda weird
    // it underlines lowercase vowels so try it out i dunno
    public static string[][] gods = new string[3][] {
        new string[] {"ZEUS","POSEIDON","ANUBIS","THOR","ODIN","ATHENA","MICHAEL JORDAN"},
        new string[] {"CTHULHU","HERMES", "SHIVA","SUN-WUKONG", "QUETZALCOATL","ARTEMIS & APOLLO", "JESUS"},
        new string[] {"NIKE","HADES", "BLANK", "BLANK", "BLANK", "BLANK", "RANDOM" },
    };

    private string[] godInfo = new string[]{
        "CHARGES PLANETS THE LONGER HE HOLDS THEM",
        "FREEZES PLAYERS BY THROWING ICE PLANETS",
        "CAN THROW PLANETS THROUGH THE SUN",
        "ATTACHES HIS HAMMER TO PLANETS AND CAUSES THEM TO BOOMERANG BACK TO HIM",
        "STRENGTH INCREASES AS HEALTH DECREASES",
        "HEALS HERSELF WHEN HEALTH IS LOW",
        "FIND THE BASKETBALLS AND SHOW THEM HOW TO SLAM",
        "CAN'T THROW BUT DAMAGES PLAYERS ON COLLISION",
        "VERY FAST BUT FRAGILE AND WEAK",
        "GAINS HEALTH WHEN A PLANET DIES IN HIS HANDS",
        "CAN THROW INVISIBLE PLANETS",
        "POISONS GODS WHEN THROWING TROPICAL PLANETS",
        "HELD PLANETS ARE SPLIT AND THROWN IN OPPOSITE DIRECTIONS",
        "TURNS WATER INTO WINE AND MAKES PLAYERS DRUNK WHEN THROWING WATER PLANETS",
        "STRONGER THE CLOSER SHE IS TO VICTORY",
        "COMES BACK FROM DEATH",
        "BLANK",
        "BLANK",
        "BLANK",
        "BLANK",
        "RNG WILL DETERMINE YOUR FATE"
    };

    // percentage of screen width travelled per second
    public float moveSpeed = .3f;
    //The volume of the sound of selecting a character
    public float CharSoundVolume;

    private List<Player> players = new List<Player>();
    private float gameStartCountdown;
    private bool usingKeyboard;
    private Text countDown;
    private Image overLay;
    private AudioSource menuMusic;

    private Vector2 minAnch;
    private Vector2 maxAnch;
    private float padX;
    private float padY;
    private float imgW;
    private float imgH;

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
        int len = 0;
        for (int i = 0; i < gods.Length; i++) {
            for (int j = 0; j < gods[i].Length; j++) {
                len++;
            }
        }

        RectTransform mainPanel = GetComponent<Image>().rectTransform;
        minAnch = mainPanel.anchorMin;
        maxAnch = mainPanel.anchorMax;

        // make sure it starts enabled in the inspector (can't Find disabled objects (thanks unity (thanks obama)))
        countDown = GameObject.Find("CountDown").GetComponent<Text>();
        overLay = GameObject.Find("Overlay").GetComponent<Image>();
        countDown.enabled = false;
        overLay.gameObject.SetActive(false);

        godGameObjects = new GodGameObject[len];
        int xL = gods.Length;
        int yL = gods[0].Length;
        padX = .008f;   // percentage of main panel padding
        padY = .015f;   // eyeballed these but whatever
        imgW = (1f - padX * (yL + 1)) / yL;
        imgH = (1f - padY * (xL + 1)) / xL;
        for (int y = 0; y < xL; y++) {
            for (int x = 0; x < yL; x++) {
                if (x >= gods[y].Length) {
                    break;
                }
                int godCoord1D = y * yL + x;

                GodGameObject thisGod = new GodGameObject();
                godGameObjects[godCoord1D] = thisGod;
                // main rectTransform anchoring for each god
                GameObject mainGO = new GameObject(gods[y][x]);
                thisGod.name = mainGO.name;
                mainGO.transform.parent = gameObject.transform;
                RectTransform mainRT = mainGO.AddComponent<RectTransform>();
                mainRT.anchorMin = new Vector2(x * imgW + padX * (x + 1), 1f - (y + 1) * imgH - padY * (y + 1));
                mainRT.anchorMax = new Vector2((x + 1) * imgW + padX * (x + 1), 1f - y * imgH - padY * (y + 1));
                mainRT.offsetMin = Vector2.zero;
                mainRT.offsetMax = Vector2.zero;

                // adds gods sound
                if (godSounds != null && godSounds.Length > godCoord1D)
                {
                    thisGod.sound = godSounds[godCoord1D];
                }

                
                // add gods image
                GameObject imgGO = new GameObject("image");
                imgGO.transform.parent = mainGO.transform;
                Image img = imgGO.AddComponent<Image>();
                thisGod.image = img;
                if (godSprites != null && godSprites.Length > godCoord1D) {
                    img.sprite = godSprites[godCoord1D];
                }
                //img.preserveAspect = true;
                img.rectTransform.anchorMin = new Vector2(0f, .125f);
                img.rectTransform.anchorMax = new Vector2(1f, 1f);
                img.rectTransform.offsetMin = Vector2.zero;
                img.rectTransform.offsetMax = Vector2.zero;

                // panel for gods name text
                GameObject panelGO = new GameObject("name panel");
                panelGO.transform.parent = mainGO.transform;
                Image txtPanel = panelGO.AddComponent<Image>();
                txtPanel.sprite = nameBackground;
                txtPanel.rectTransform.anchorMin = new Vector2(0f, 0f);
                txtPanel.rectTransform.anchorMax = new Vector2(1f, .25f);
                txtPanel.rectTransform.offsetMin = Vector2.zero;
                txtPanel.rectTransform.offsetMax = Vector2.zero;

                // gods name text
                GameObject txtGO = new GameObject("name");
                txtGO.transform.parent = panelGO.transform;
                Text txt = txtGO.AddComponent<Text>();
                txt.text = gods[y][x];
                txt.font = font;
                txt.color = new Color(1f, 1f, .6f);
                txt.alignment = TextAnchor.MiddleCenter;
                txt.resizeTextForBestFit = true;
                txt.resizeTextMinSize = 10;
                txt.resizeTextMaxSize = 100;
                txt.rectTransform.anchorMin = Vector2.zero;
                txt.rectTransform.anchorMax = Vector2.one;
                txt.rectTransform.offsetMin = Vector2.zero;
                txt.rectTransform.offsetMax = Vector2.zero;

                // gods info text
                GameObject infoGO = new GameObject("god info");
                thisGod.info = infoGO;
                infoGO.transform.parent = mainGO.transform;
                Text info = infoGO.AddComponent<Text>();
                info.text = godInfo[godCoord1D];
                info.font = font;
                info.color = new Color32(200, 255, 255, 255);
                info.rectTransform.anchorMin = new Vector2(0f, .25f);
                info.rectTransform.anchorMax = Vector2.one;
                info.rectTransform.offsetMin = Vector2.zero;
                info.rectTransform.offsetMax = Vector2.zero;
                info.resizeTextForBestFit = true;
                info.resizeTextMinSize = 10;
                info.resizeTextMaxSize = 30;
                infoGO.SetActive(false);
            }
        }

        padX *= (maxAnch.x - minAnch.x);
        padY *= (maxAnch.y - minAnch.y);

        imgW *= (maxAnch.x - minAnch.x);
        imgH *= (maxAnch.y - minAnch.y);

        // add two players for keyboard mode if no controllers are connected
        string[] connectedJoysticks = Input.GetJoystickNames();
        bool anyJoysticksConnected = false;
        for (int i = 0; i < connectedJoysticks.Length; i++) {
            if (connectedJoysticks[i] != "") {
                anyJoysticksConnected = true;
            }
        }
        if (!anyJoysticksConnected) {
            usingKeyboard = true;
            players.Add(new Player(1, playerFont, playerSprite));
            players.Add(new Player(2, playerFont, playerSprite));
        }
    }

    // Update is called once per frame
    void Update() {
        // check for newly connected joysticks
        if (!usingKeyboard) {
            string[] connectedJoysticks = Input.GetJoystickNames();
            // NORMAL JOYSTICK MODE
            bool playerNumChanged = false;
            for (int i = 0; i < connectedJoysticks.Length; i++) {
                if (connectedJoysticks[i] != "") {
                    // add new player if new unused joystick is found
                    if (players.Find(p => p.id == i + 1) == null) {
                        players.Add(new Player(i + 1, playerFont, playerSprite));
                        playerNumChanged = true;
                    }
                } else {    // remove player with this id if found
                    Player toRemove = players.Find(p => p.id == i + 1);
                    if (players.Remove(toRemove)) {
                        Destroy(toRemove.img.gameObject);
                        playerNumChanged = true;
                    }
                }
            }

            // if player number changed then resort player list based on id
            // then set anchors based on which players are present
            if (playerNumChanged) {
                players.Sort((p1, p2) => p1.id.CompareTo(p2.id));
            }
        }

        // let first player cancel back to menu
        if (players.Count > 0) {
            if (Input.GetButtonDown("Cancel" + players[0].id) && players[0].chosen == "") {
                Application.LoadLevel("Menu");
            }
        }

        // need at least 2 players to start game
        bool allPlayersDecided = players.Count > 1;
        // process input for joysticks and move each player
        foreach (Player p in players) {

            float speed = moveSpeed * Screen.width * Time.deltaTime;
            float x = Input.GetAxis("Horizontal" + (usingKeyboard ? "" : "_360_") + p.id);
            float y = Input.GetAxis("Vertical" + (usingKeyboard ? "" : "_360_") + p.id);

            if (usingKeyboard && Mathf.Abs(x) + Mathf.Abs(y) > 1.5f) {
                x *= .7071f;
                y *= .7071f;
            }

            x *= speed;
            y *= speed;

            if (p.chosen == "") {
                Color c = p.img.color;
                p.img.color = new Color(c.r, c.g, c.b, .3f);
                p.x = Mathf.Clamp(p.x + x, -Screen.width / 2f, Screen.width / 2f);
                p.y = Mathf.Clamp(p.y + y, -Screen.height / 2f, Screen.height / 2f);
            } else {
                Color c = p.img.color;
                p.img.color = new Color(c.r, c.g, c.b, 1f);
            }

            p.img.rectTransform.anchoredPosition = new Vector2(p.x, p.y);

            int godHover = getGodAtScreenPoint((p.x + Screen.width / 2f) / Screen.width, (p.y + Screen.height / 2f) / Screen.height);
            if (godHover >= 0) {
                string godName = godGameObjects[godHover].name;
                if (usingKeyboard && Input.GetButtonDown("Fire" + p.id)) {
                    p.chosen = p.chosen == "" ? godName : "";
                    //Play the Characters sound on selection
                    if (p.chosen != "" && godGameObjects[godHover].sound != null)
                    {
                        AudioSource.PlayClipAtPoint(godGameObjects[godHover].sound, Vector3.zero, CharSoundVolume);
                    }
                } else {
                    if (Input.GetButtonDown("Submit" + p.id)) {
                        p.chosen = godName;
                    }
                    if (Input.GetButtonDown("Cancel" + p.id)) {
                        p.chosen = "";
                        //need to remove sound on cancel
                    }
                }

                while (p.chosen == "RANDOM" || p.chosen == "BLANK") {
                    p.chosen = godGameObjects[Random.Range(0, godGameObjects.Length - 1)].name;
                }

                if (Input.GetButton("Y" + p.id) || Input.GetKey(KeyCode.Y)) {
                    godGameObjects[godHover].checkingInfo = true;
                }
            }

            if (p.chosen == "") {
                allPlayersDecided = false;
            }

            // incase screen is resized during play (probably unnecessary)
            p.updatePointerSize();
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
            overLay.gameObject.SetActive(true);
            Color c = overLay.color;
            overLay.color = new Color(c.r, c.b, c.g, 1f - gameStartCountdown / 3f);
            menuMusic.volume = gameStartCountdown / 3f;

            if (gameStartCountdown < 0f) {
                menuMusic.Stop();
                Destroy(menuMusic.gameObject);
                // save all the players choices
                PlayerPrefs.DeleteAll();
                PlayerPrefs.SetInt("Number of players", players.Count);
                for (int i = 0; i < players.Count; i++) {
                    Player p = players[i];
                    PlayerPrefs.SetInt("Player" + i + " ", p.id);
                    PlayerPrefs.SetString("Player" + p.id, p.chosen);
                }
                Application.LoadLevel("Main");
            }
        } else {
            // stop countdown
            countDown.enabled = false;
            overLay.gameObject.SetActive(false);
            gameStartCountdown = 3f;
            menuMusic.volume = 1f;
        }
    }

    public int getGodAtScreenPoint(float x, float y) {
        int xg = -1;
        int yg = -1;

        int cols = gods[0].Length;
        int rows = gods.Length;

        float xp = minAnch.x;
        for (int i = 0; i < cols; i++) {
            xp += padX;
            if (x >= xp && x <= xp + imgW) {
                xg = i;
                break;
            }
            xp += imgW;
        }

        float yp = maxAnch.y;
        for (int i = 0; i < rows; i++) {
            yp -= padY;
            if (y <= yp && y >= yp - imgH) {
                yg = i;
                break;
            }
            yp -= imgH;
        }

        if (xg < 0 || yg < 0) {
            return -1;
        }

        return yg * cols + xg;
    }
}

class Player {
    public float x = 0;
    public float y = 0;
    public bool locked = false;

    public string chosen = "";
    public int id;

    public Image img;
    public Text txt;
    public Text btxt;

    private Vector2 relativeAnchor;

    // should probably use a nicer color palette lol
    public static Color[] colors = new Color[] {
        Color.red, Color.yellow, Color.green, Color.blue, Color.magenta, Color.cyan, Color.grey, Color.black };

    public Player(int id, Font f, Sprite sprite) {
        this.id = id;

        // sprite image
        img = new GameObject("Player " + id).AddComponent<Image>();
        img.sprite = sprite;
        img.color = colors[id - 1];
        img.type = Image.Type.Simple;

        // text that indicates player number
        txt = new GameObject("Player " + id + " text").AddComponent<Text>();
        txt.font = f;
        txt.text = "" + id;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.resizeTextForBestFit = true;
        txt.resizeTextMinSize = 10;

        // black background text
        btxt = new GameObject("Player " + id + " btext").AddComponent<Text>();
        btxt.font = f;
        btxt.text = "" + id;
        btxt.color = Color.black;
        btxt.alignment = TextAnchor.MiddleCenter;
        btxt.resizeTextForBestFit = true;
        btxt.resizeTextMinSize = 10;
        btxt.rectTransform.anchoredPosition3D = new Vector3(2, -3, 0);

        img.transform.SetParent(GameObject.Find("Canvas").transform);
        img.transform.SetSiblingIndex(id + 2);
        btxt.transform.SetParent(img.transform);
        txt.transform.SetParent(img.transform);

        updatePointerSize();
    }

    // set player pointer size based on percentage of screen width
    public void updatePointerSize() {
        float size = .05f * Screen.width;

        img.rectTransform.sizeDelta = new Vector2(size, size);
        txt.resizeTextMaxSize = (int)size;
        btxt.resizeTextMaxSize = (int)size;
    }
}

// small class to hold and save some variables to avoid GetComponent calls
class GodGameObject {
    public string name;
    public Image image;
    public AudioClip sound;
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
