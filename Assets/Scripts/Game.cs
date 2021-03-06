﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Game : MonoBehaviour {
    // list of all gods in current game
    public List<GodController> players;
    public AudioClip[] songs;
    public int numPlayers;
    public bool usingKeyboard = false;
    private GameObject overlay;
    private Text countDown;
    private Text gameOverText;
    private Text gameOverTextBack;
    private float timer;
    private int soundTimer;
    private int frames;
    private bool songStart = false;
    private bool gameStart = false;
    private bool introFinished = false;
    private AudioSource song;
    private int winner;
    private string winnerName;
    private bool isPaused = false;

    // lazy singleton class
    public static Game instance { get; private set; }
    void Awake() {
        instance = this;
        //Application.targetFrameRate = 60;
    }

    // use this for initialization
    void Start() {
        timer = 4.99f;
        soundTimer = 4;
        frames = 0;
        winner = 0;
        countDown = GameObject.Find("CountdownText").GetComponent<Text>();
        gameOverText = GameObject.Find("GameOverText").GetComponent<Text>();
        gameOverTextBack = GameObject.Find("GameOverTextBack").GetComponent<Text>();
        song = Camera.main.GetComponent<AudioSource>();
        overlay = GameObject.Find("PauseOverlay");
        overlay.SetActive(false);
        overlay.GetComponent<Renderer>().enabled = true;

        // spawn players with data from player prefs
        numPlayers = PlayerPrefs.GetInt("Number of players");
        usingKeyboard = PlayerPrefs.GetInt("Keyboard") == 1;
        for (int i = 0; i < numPlayers; i++) {
            int player = PlayerPrefs.GetInt("Player" + i + " ");
            string choice = PlayerPrefs.GetString("Player" + player);
            //Debug.Log(player + " " + choice);
            Object loadedGod = Resources.Load("Gods/" + choice);
            if (!loadedGod) {
                choice = "ZEUS";    // classic
                loadedGod = Resources.Load("Gods/" + choice);
            }
            Vector3 spawnLocation = new Vector3(-20, 0, 0);
            spawnLocation = Quaternion.Euler(0, 0, i * (360f / numPlayers)) * spawnLocation;

            GameObject godPrefab = (GameObject)Instantiate(loadedGod, spawnLocation, Quaternion.identity);
            godPrefab.name = choice;
            godPrefab.GetComponent<GodController>().id = player;
            godPrefab.transform.Find("Range Indicator").GetComponent<SpriteRenderer>().color = Player.colors[player - 1];
        }

        // generate boundary pased on number of players
        GameObject.Find("Boundary").GetComponent<BoundaryGenerator>().generate(numPlayers);
    }

    // update is called once per frame
    void Update() {
        runIntro();
        checkWinner();
        handlePause();
        playMusic();
    }

    public void playMusic() {
        if (song.time >= 209.68f) {
            song.time = 40.96f;
        }
    }

    public void DamageAllEnemies() {
        for (int i = 0; i < players.Count; i++) {
            if (players[i].god.type != GodType.ANUBIS) {
                players[i].god.changeHealth(-10);
            }
        }
    }

    public void addPlayer(GodController player) {
        players.Add(player);
    }

    public void removePlayer(GodController player) {
        players.Remove(player);
    }

    public bool hasBegun() {
        return introFinished;
    }

    public bool gameIsOver() {
        // if there is only one god left
        if (Time.timeSinceLevelLoad > 5f) {
            return players.Count <= 1;
        }
        return false;
    }

    private void checkWinner() {
        if (gameIsOver()) {
            if (winner == 0 && players.Count > 0) {
                winnerName = players[0].name;
                winner = players[0].id;

                string winPlurality = winnerName == "ARTEMIS & APOLLO" ? "WIN" : "WINS";
                string restartLetter = usingKeyboard ? "R" : "A";
                string quitLetter = usingKeyboard ? "Q" : "B";
                gameOverText.text = "GAME OVER!\n" + winnerName + " (P" + winner + ") " + winPlurality + "!\n P" + winner + ": " + restartLetter + " TO CHANGE GODS\nY TO PLAY AGAIN\n" + quitLetter + " TO QUIT";
                gameOverTextBack.text = gameOverText.text;
            }
            if (winner != 0) {
                if (Input.GetKeyDown(KeyCode.R) || Input.GetButtonDown("Submit" + winner)) {
                    Application.LoadLevel("Character Selection");
                }
                if (Input.GetKeyDown(KeyCode.Q) || Input.GetButtonDown("Cancel" + winner)) {
                    Application.LoadLevel("Menu");
                }
                if (Input.GetKeyDown(KeyCode.Y) || Input.GetButton("Y" + winner)) {
                    Application.LoadLevel("Main");
                }
            }
        }
    }

    private void runIntro() {
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
                if (t != soundTimer && t > 0) {
                    // noises for the countdown numbers
                    AudioManager.instance.playSound("Collision", Vector3.zero, 1f);
                    soundTimer = t;
                    countDown.text = t.ToString();
                }
                float maxFontSize = 400f;
                if (t <= 0) {
                    countDown.fontSize = (int)Mathf.Lerp(0f, maxFontSize, (timer + 1f) / 2f);
                    if (!gameStart) {   // start game here
                        countDown.text = "GO!";
                        AudioManager.instance.playSound("Explosion1", Vector3.zero, .5f);
                        countDown.color = Color.white;
                        for (int i = 0; i < players.Count; i++) {
                            players[i].freezeInputs = false;
                        }
                        gameStart = true;
                    }
                    if (frames > 5) {  // flicker text between yellow and white
                        countDown.color = (countDown.color == Color.white) ? Color.yellow : Color.white;
                        frames = 0;
                    }
                    frames++;
                } else {
                    countDown.fontSize = (int)Mathf.Lerp(maxFontSize / 2f, maxFontSize, timer - t);
                }

                if (t <= -1) {
                    // intro is over
                    countDown.text = "";
                    introFinished = true;
                }
            }
            timer -= Time.deltaTime / 1.25f;    // slower than normal
        }
    }

    private void handlePause() {
        if (Input.GetKeyDown(KeyCode.P) && !isPaused && !gameIsOver() && introFinished) {
            Time.timeScale = 0;
            isPaused = true;
            gameOverText.text = "PAUSED";
            gameOverTextBack.text = gameOverText.text;
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
                gameOverTextBack.text = gameOverText.text;
            }
        }
    }
}
