using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlanetSpawner : MonoBehaviour {
    public static PlanetSpawner current;

    private int activePlanets = 0;
    public int maxPlanets;
    public float spawnSpeed = .25f;
    private float spawnTime;

    public Sprite[] planetSprites;

    private Stack<GameObject> pool;
    private Object basicPlanet;

    void Awake() {
        current = this;

        basicPlanet = Resources.Load("Planet");
        pool = new Stack<GameObject>();
        for (int i = 0; i < maxPlanets; i++) {
            returnPlanet((GameObject)Instantiate(basicPlanet));
            ++activePlanets; // to offset returnplanets
        }

    }

    public void returnPlanet(GameObject obj) {
        obj.transform.parent = transform;
        obj.SetActive(false);
        obj.name = "Pooled Planet";
        pool.Push(obj);
        --activePlanets;
    }

    private GameObject getPlanet() {
        if (pool.Count > 0) {
            return pool.Pop();
        }

        return (GameObject)Instantiate(basicPlanet);

    }

    void Update() {
        if (Game.instance.hasBegun() && !Game.instance.gameIsOver()) {
            if (activePlanets < maxPlanets && spawnTime < Time.timeSinceLevelLoad) {
                //Time.time doesn't work for restart
                spawnPlanet();
                spawnTime = Time.timeSinceLevelLoad + spawnSpeed;
            }

            // increase maxPlanets by 1 every 5 seconds
            // not sure if we want this (change maxPlanets back to float if so)
            //maxPlanets += Time.deltaTime/5f;
        }
    }

    private void spawnPlanet() {
        int numberOfPlanetTypes = System.Enum.GetValues(typeof(PlanetType)).Length;
        if (planetSprites.Length < numberOfPlanetTypes) {
            Debug.Log("Not enough planet sprites defined.");
            return;
        }

        Vector2 spawn = Vector2.zero;

        // with Random.Range for integers the max is exclusive for some reason
        switch (Random.Range(1, 5)) {
            case 1:
                spawn = new Vector2(-0.1f, Random.Range(0f,1f));
                break;

            case 2:
                spawn = new Vector2(1.1f, Random.Range(0f, 1f));
                break;

            case 3:
                spawn = new Vector2(Random.Range(0f, 1f), -0.1f);
                break;

            case 4:
                spawn = new Vector2(Random.Range(0f, 1f), 1.1f);
                break;
        }

        GameObject planet = getPlanet();
        planet.SetActive(true);
        planet.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(spawn.x, spawn.y, 10f)); ;
        planet.transform.rotation = Quaternion.identity;    //just in case

        Planet script = planet.GetComponent<Planet>();

        int i = Random.Range(0, numberOfPlanetTypes);
        script.type = (PlanetType)i;
        planet.name = script.type.ToString();
        script.sr.sprite = planetSprites[i];
        script.initializeVariables();

        //planet.transform.parent = gameObject.transform;
        ++activePlanets;

    }
}

