﻿using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour 
{
    private God god;
    private Vector3 screenPoint;
    private Texture2D texture;
    private GUIStyle style;

    // use this for initialization
    void Start() 
	{
        god = GetComponent<God>();
        texture = new Texture2D(1, 1);
        style = new GUIStyle();
    }

    // update is called once per frame
    void Update() 
	{
        screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        screenPoint.y = Screen.height - screenPoint.y;
    }

    void OnGUI() 
	{
        // avoids editor errors
        if (god == null) 
		{
            return;
        }

        if (god.health > 50)
            texture.SetPixel(0, 0, Color.green);
        else if (god.health <= 50 && god.health > 20)
            texture.SetPixel(0, 0, Color.yellow);
        else
            texture.SetPixel(0, 0, Color.red);

        texture.Apply();

        style.normal.background = texture;

        float yOff = 700f / Camera.main.orthographicSize;

        GUI.Box(new Rect(screenPoint.x - 50, screenPoint.y + yOff, god.health, 5), GUIContent.none, style);

        // add gray bar to see how far off from maxHealth you are
        texture.SetPixel(0, 0, Color.gray);
        texture.Apply();
        GUI.Box(new Rect(screenPoint.x -50 + god.health, screenPoint.y + yOff, god.maxHealth - god.health, 5), GUIContent.none, style);
    }
}
