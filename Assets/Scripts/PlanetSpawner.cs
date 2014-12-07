using UnityEngine;
using System.Collections;

public class PlanetSpawner : MonoBehaviour {

    public GameObject planet;
    static public int planetNum = 0;
    public int maxPlanets = 15;

    // Use this for initialization
    void Start() {
        InvokeRepeating("SpawnPlanet", 1f, 1f);
    }

    public void SpawnPlanet() {
        if (planetNum < maxPlanets) {

            float x = 0f;
            float y = 0f;

            // with Random.Range for integers the max is exclusive for some reason
            switch (Random.Range(1, 5)) {
                case 1:
                    x = -0.1f;
                    y = Random.Range(0f, 1f);
                    break;

                case 2:
                    x = 1.1f;
                    y = Random.Range(0f, 1f);
                    break;

                case 3:
                    y = -0.1f;
                    x = Random.Range(0f, 1f);
                    break;

                case 4:
                    y = 1.1f;
                    x = Random.Range(0f, 1f);
                    break;

            }

            Vector3 p = Camera.main.ViewportToWorldPoint(new Vector3(x, y, 10f));
            GameObject newPlanet = (GameObject)Instantiate(planet, p, Quaternion.identity);
            newPlanet.transform.parent = gameObject.transform;
			newPlanet.GetComponent<CircleCollider2D>().isTrigger = true;
            ++planetNum;
        }
    }
}
