using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public float minSize = 5f;
    private Camera mainCam;

    // Use this for initialization
    void Start() {
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update() {

        // calculate bounding box of all the gods in match
        Bounds bounds = new Bounds();
        for (int i = 0; i < Game.instance.players.Count; ++i) {
            GodController player = Game.instance.players[i];
            Bounds b = player.getCameraBounds();

            // if you don't want to include origin
            if (i == 0) {
                //bounds = new Bounds(b.center, b.size);
            }

            bounds.Encapsulate(b);
        }

        // set center of camera to center of the bounding box
        mainCam.transform.position = new Vector3(bounds.center.x, bounds.center.y, -10f);

        // calculate minimum required height
        float reqHeight = bounds.extents.x / mainCam.aspect;

        // set camera size
        // never goes smaller than minSize and has to be at least the reqHeight
        // should probably add some smoothing to this later (by lerping)
        mainCam.orthographicSize = Mathf.Max(minSize, (bounds.extents.y < reqHeight) ? reqHeight : bounds.extents.y);

		
    }

}
