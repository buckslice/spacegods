using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour 
{
	public GUIText gameOverText;
	private GameObject overlay;
    private Text text;
    private float timer = 4.99f;
    private int soundTimer = 4;
    private int frames = 0;
    private bool songStart = false;
    private bool gameStart = false;
    private bool introFinished = false;
    private AudioSource song;
    private string winner;
	private bool isPaused = false;

    // lazy singleton class
    public static Game instance { get; private set; }
    void Awake() 
	{
        instance = this;
    }

    // list of all gods in current game
    public List<GodController> players;

	// use this for initialization
	void Start () 
	{
		gameOverText.text = "";
        winner = "";
        text = GameObject.Find("CountdownText").GetComponent<Text>();
        song = Camera.main.GetComponent<AudioSource>();
		overlay = GameObject.Find ("PauseOverlay");
		overlay.SetActive (false);
		overlay.GetComponent<Renderer>().enabled = true;
	}
	
	// update is called once per frame
	void Update () 
	{
        if (!introFinished) 
		{   
			// code for cool intro countdown and music
            int t = (int)timer;
            if (t <= 3) 
			{
                if (!songStart) 
				{   
					// start song up at right time
                    song.time = 19.5f;
                    song.volume = 0f;
                    song.Play();
                    songStart = true;
                }
                song.volume += .25f * Time.deltaTime;   // fade in volume
                text.text = (t <= 0) ? "GO!" : t + "";
                text.fontSize = 200 + (int)((timer - (int)timer) * 100f);
                if (t != soundTimer && t > 0) 
				{ 
					// noises for the countdown numbers
                    AudioManager.instance.playSound("Collision", Vector3.zero, 1f);
                    soundTimer = t;
                }
                if (t <= 0 && !gameStart) 
				{   
					// game starts here
                    AudioManager.instance.playSound("Explosion1", Vector3.zero, .5f);
                    text.color = Color.white;
                    GameObject.Find("SCRIPTS").GetComponent<PlanetSpawner>().Begin();
                    foreach (GodController gc in players) 
					{
                        gc.unlockInput();
                    }
                    gameStart = true;
                }
                if (t <= 0 && frames > 5) 
				{   
					// flicker text between yellow and white
                    text.color = (text.color == Color.white) ? Color.yellow : Color.white;
                    frames = 0;
                }
                frames++;

                if (t <= -1) 
				{  
					// intro is over
                    text.text = "";
                    introFinished = true;
                }
            }
            timer -= Time.deltaTime / 1.25f;
        }

		if (gameIsOver ()) 
		{
			if (winner == "" && players.Count > 0) 
			{
				winner = players [0].name;
			}
			gameOverText.text = "Game Over! " + winner + " wins!\nPress R to Restart\nPress Q to Quit";
			if (Input.GetKeyDown (KeyCode.R)) 
			{
				Application.LoadLevel ("Main");
			}
			if (Input.GetKeyDown (KeyCode.Q)) 
			{
				Application.LoadLevel (0);
			}
		}
		if (Input.GetKeyDown (KeyCode.P) && !isPaused)
		{
			Time.timeScale = 0;
			isPaused = true;
			gameOverText.text = "Game Paused";
			overlay.SetActive(true);
		}
		else if (isPaused)
		{
			if (Input.GetKeyDown (KeyCode.R)) 
			{
				Time.timeScale = 1;
				isPaused = false;
				Application.LoadLevel ("Main");
			}
			if (Input.GetKeyDown (KeyCode.Q)) 
			{
				Time.timeScale = 1;
				isPaused = false;
				Application.LoadLevel (0);
			}
			if (Input.GetKeyDown(KeyCode.P))
			{
				Time.timeScale = 1;
				isPaused = false;
				overlay.SetActive(false);
				gameOverText.text = "";
			}
		}

	}

    public void addPlayer(GodController player) 
	{
        players.Add(player);
    }

    public void removePlayer(GodController player) 
	{
        players.Remove(player);
    }

	public bool gameIsOver() 
	{   
		// if there is only one god left
		return players.Count <= 1;
	}
}
