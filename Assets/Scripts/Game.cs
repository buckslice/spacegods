using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {

    // lazy singleton class
    public static Game instance { get; private set; }
    void Awake() {
        instance = this;
    }

    // list of all gods in current game
    public List<God> gods;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void addGodToList(God god) {
        gods.Add(god);
    }
}
