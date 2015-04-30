using UnityEngine;
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

        if (god.getCurrentHealth() > god.maxHealth/2)
            texture.SetPixel(0, 0, Color.green);
		else if (god.getCurrentHealth() <= god.maxHealth/2 && god.getCurrentHealth() > god.maxHealth/4)
            texture.SetPixel(0, 0, Color.yellow);
        else
            texture.SetPixel(0, 0, Color.red);

        texture.Apply();

        style.normal.background = texture;

        float yOff = 700f / Camera.main.orthographicSize;

		GUI.Box(new Rect(screenPoint.x - god.maxHealth/2, screenPoint.y + yOff, god.getCurrentHealth(), 5), GUIContent.none, style);

        // add gray bar to see how far off from maxHealth you are
        texture.SetPixel(0, 0, Color.gray);
        texture.Apply();
		GUI.Box(new Rect(screenPoint.x - god.maxHealth/2 + god.getCurrentHealth(), screenPoint.y + yOff, god.maxHealth - god.getCurrentHealth(), 5), GUIContent.none, style);
    }
}
