using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public float targSize = 5f;
    public float minSize = 5f;
    public float jumpSize = 8f;
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
            //if (i == 0) {
            //  bounds = new Bounds(b.center, b.size);
            //}

            bounds.Encapsulate(b);
        }

        // set center of camera to center of the bounding box
        mainCam.transform.position = new Vector3(bounds.center.x, bounds.center.y, -10f);

        // calculate minimum required height
        float reqHeight = bounds.extents.x / mainCam.aspect;

        // set camera size
        // never goes smaller than minSize and has to be at least the reqHeight
        float targSize = Mathf.Max(minSize, (bounds.extents.y < reqHeight) ? reqHeight : bounds.extents.y);
        targSize = ((int)(targSize / jumpSize) + 1) * jumpSize;

        // lerp quickly when camera is expanding and slowly when shrinking
        float rate = targSize > mainCam.orthographicSize ? Time.deltaTime * 2f : Time.deltaTime;
        mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize, targSize, rate);


    }

}
