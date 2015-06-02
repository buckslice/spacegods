using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {
    public void LoadLevel(string action) {
        if (action == "Play") {
            Application.LoadLevel("Character Selection");
        } else if (action == "Instructions") {
            Application.LoadLevel("Instructions");
        } else if (action == "Menu") {
            Application.LoadLevel("Menu");
        } else if (action == "Credits") {
            Application.LoadLevel("Credits");
        } else if (action == "Exit") {
            Application.Quit();
        }
    }

    void Update() {
        if (Application.loadedLevel == 4) {
            if (Input.anyKeyDown) {
                Application.LoadLevel("Menu");

            }
        }
    }
}

