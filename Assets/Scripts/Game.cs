using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {
	public GUIText gameOverText;
    // lazy singleton class
    public static Game instance { get; private set; }
    void Awake() {
        instance = this;
    }

    // list of all gods in current game
    public List<GodController> players;

	// Use this for initialization
	void Start () {
		gameOverText.text = "";
	}
	
	// Update is called once per frame
	void Update () {
		if (gameIsOver()) {
			gameOverText.text = "Game Over! " + players[0].name + " wins!\nPress R to Restart\nPress Q to Quit";

			if (Input.GetKeyDown (KeyCode.R))
			{
				Application.LoadLevel (Application.loadedLevel);
			}
			if (Input.GetKeyDown (KeyCode.Q))
			{
				Application.LoadLevel (0);
			}
		}
	}

    public void addPlayer(GodController player) {
        players.Add(player);
    }

    public void removePlayer(GodController player) {
        players.Remove(player);
    }

	public bool gameIsOver() {//if there is only one god left
		return players.Count == 1;
	}
}
