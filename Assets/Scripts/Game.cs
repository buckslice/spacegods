using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {
    private GameObject overlay;
    private Text countDown;
    private Text gameOverText;
    private float timer = 4.99f;
    private int soundTimer = 4;
    private int frames = 0;
    private bool songStart = false;
    private bool gameStart = false;
    private bool introFinished = false;
    private AudioSource song;
    private int winner = 0;
    private string winnerName;
    private bool isPaused = false;

    // lazy singleton class
    public static Game instance { get; private set; }
    void Awake() {
        instance = this;
    }

    // list of all gods in current game
    public List<GodController> players;

    // use this for initialization
    void Start() {
		initializeVariables ();
    }

    // update is called once per frame
    void Update() {
		handleIntro();
		handleWinner ();
		handlePause();
    }

    public void addPlayer(GodController player) {
        players.Add(player);
    }

    public void removePlayer(GodController player) {
        players.Remove(player);
    }
	public bool hasBegun(){
		return introFinished;
	}
    public bool gameIsOver() {
        // if there is only one god left
        if (Time.timeSinceLevelLoad > 5f) {
            return players.Count <= 1;
        }
        return false;
    }

	private void initializeVariables(){
		countDown = GameObject.Find("CountdownText").GetComponent<Text>();
		gameOverText = GameObject.Find("GameOverText").GetComponent<Text>();
		song = Camera.main.GetComponent<AudioSource>();
		overlay = GameObject.Find("PauseOverlay");
		overlay.SetActive(false);
		overlay.GetComponent<Renderer>().enabled = true;
		
		// spawn players with data from player prefs
		int numPlayers = PlayerPrefs.GetInt("Number of players");
		for (int i = 0; i < numPlayers; i++) {
			int player = PlayerPrefs.GetInt("Player" + i + " ");
			string choice = PlayerPrefs.GetString("Player" + player);
			//Debug.Log(player + " " + choice);
			Object loadedGod = Resources.Load(choice);
			if (!loadedGod) {
				choice = "Zeus";    // classic
				loadedGod = Resources.Load(choice);
			}
			Vector3 spawnLocation = new Vector3(-20, 0, 0);
			spawnLocation = Quaternion.Euler(0, 0, i * (360f / numPlayers)) * spawnLocation;
			
			GameObject godPrefab = (GameObject)Instantiate(loadedGod, spawnLocation, Quaternion.identity);
			godPrefab.name = choice;
			godPrefab.GetComponent<GodController>().player = player;
		}
	}

	private void handleIntro(){
		if (!introFinished) {
			// code for cool intro countdown and music
			int t = (int)timer;
			if (t <= 3) {
				if (!songStart) {
					// start song up at right time
					song.time = 19.5f;
					song.volume = 0f;
					song.Play();
					songStart = true;
				}
				song.volume += .25f * Time.deltaTime;   // fade in volume
				countDown.text = (t <= 0) ? "GO!" : t + "";
				countDown.fontSize = 200 + (int)((timer - (int)timer) * 100f);
				if (t != soundTimer && t > 0) {
					// noises for the countdown numbers
					AudioManager.instance.playSound("Collision", Vector3.zero, 1f);
					soundTimer = t;
				}
				if (t <= 0 && !gameStart) {
					// game starts here
					AudioManager.instance.playSound("Explosion1", Vector3.zero, .5f);
					countDown.color = Color.white;
					foreach (GodController gc in players) {
						gc.freezeInputs = false;
					}
					gameStart = true;
				}
				if (t <= 0 && frames > 5) {
					// flicker text between yellow and white
					countDown.color = (countDown.color == Color.white) ? Color.yellow : Color.white;
					frames = 0;
				}
				frames++;
				
				if (t <= -1) {
					// intro is over
					countDown.text = "";
					introFinished = true;
				}
			}
			timer -= Time.deltaTime / 1.25f;
		}
	}
	private void handleWinner(){
		if (gameIsOver ()) {
			if (winner == 0 && players.Count > 0) {
					winnerName = players [0].name;
					winner = players [0].player;
			}
			if (winner != 0) {
					//gameOverText.text = "Game Over!\nPlayer " + winner + " (" + winnerName + ") wins!\nWinner Press A to Restart\nor Press B to Quit";
					gameOverText.text = "Game Over!\n" + winnerName + " (P" + winner + ") wins!\nWinner Press A to Restart\nor Press B to Quit";
					if (Input.GetKeyDown (KeyCode.R) || Input.GetButtonDown ("Submit" + winner)) {
							Application.LoadLevel ("Main");
					}
					if (Input.GetKeyDown (KeyCode.Q) || Input.GetButtonDown ("Cancel" + winner)) {
							Application.LoadLevel (0);
					}
			}
		}
	}

	private void handlePause(){
		if (Input.GetKeyDown(KeyCode.P) && !isPaused) {
			Time.timeScale = 0;
			isPaused = true;
			gameOverText.text = "Game Paused";
			overlay.SetActive(true);
		} else if (isPaused) {
			if (Input.GetKeyDown(KeyCode.R)) {
				Time.timeScale = 1;
				isPaused = false;
				Application.LoadLevel("Main");
			}
			if (Input.GetKeyDown(KeyCode.Q)) {
				Time.timeScale = 1;
				isPaused = false;
				Application.LoadLevel(0);
			}
			if (Input.GetKeyDown(KeyCode.P)) {
				Time.timeScale = 1;
				isPaused = false;
				overlay.SetActive(false);
				gameOverText.text = "";
			}
		}
	}
}
