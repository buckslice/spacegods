using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 
{
    public float targSize = 5f;
    public float minSize = 5f;
    public float jumpSize = 8f;
    public float backgroundScale = 1f / 25f;

    private Camera mainCam;
    private GameObject background;

    // use this for initialization
    void Start() 
	{
        mainCam = Camera.main;
        background = GameObject.Find("Scrolling Background");
    }

    // update is called once per frame
    void Update() 
	{
		// calculate bounding box of all the gods in match
		Bounds bounds = new Bounds ();
		for (int i = 0; i < Game.instance.players.Count; ++i) 
		{
			GodController player = Game.instance.players [i];
			Bounds b = player.getCameraBounds ();

			// if you don't want to include origin
			// if (i == 0) 
			// {
			//   bounds = new Bounds(b.center, b.size);
			// }

			bounds.Encapsulate (b);
		}

		// set center of camera to center of the bounding box
		Vector3 newPos = new Vector3 (bounds.center.x, bounds.center.y, -10f);
		mainCam.transform.position = newPos;

		// calculate minimum required height
		float reqHeight = bounds.extents.x / mainCam.aspect;

		// set camera size
		// never goes smaller than minSize and has to be at least the reqHeight
		float targSize = Mathf.Max (minSize, (bounds.extents.y < reqHeight) ? reqHeight : bounds.extents.y);
		targSize = ((int)(targSize / jumpSize) + 1) * jumpSize;

		// lerp quickly when camera is expanding and slowly when shrinking
		float rate = targSize > mainCam.orthographicSize ? Time.deltaTime * 2f : Time.deltaTime;
		mainCam.orthographicSize = Mathf.Lerp (mainCam.orthographicSize, targSize, rate);

		// find and set height for size of background quad
		float bgHeight = mainCam.orthographicSize * 2f;
		float bgWidth = bgHeight * mainCam.aspect;
		background.transform.localScale = new Vector3 (bgWidth, bgHeight, 1);

		// set tiling of background texture
		float tileX = bgWidth * backgroundScale;
		float tileY = bgHeight * backgroundScale;
		background.renderer.material.SetTextureScale ("_MainTex", new Vector2 (tileX, tileY));

		// set offset of texture; assumes camera starts at origin
        // have to subtract off half the tiling rate to make the texture grow outward from the center
        float offsetX = newPos.x * backgroundScale - tileX / 2f;
        float offsetY = newPos.y * backgroundScale - tileY / 2f;
        background.renderer.material.SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY));
    }
}
