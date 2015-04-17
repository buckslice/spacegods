﻿using UnityEngine;
using System.Collections;

public class PlanetSpawner : MonoBehaviour 
{
    private GameObject planet;
    static public int planetNum;
    public int maxPlanets = 25;

    void Awake() 
	{
        planetNum = 0;
    }

    // use this for initialization
    void Start() 
	{
        // InvokeRepeating("SpawnPlanets", 1f, 1f);
    }

    public void Begin() 
	{
        InvokeRepeating("SpawnPlanets", 1f, 0.5f);
    }

    public void SpawnPlanets() 
	{
        if (planetNum < maxPlanets && !Game.instance.gameIsOver()) 
		{
            float x = 0f;
            float y = 0f;

            // with Random.Range for integers the max is exclusive for some reason
            switch (Random.Range(1, 5)) 
			{
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
			switch (Random.Range(1,9)) 
			{ 
				// spawn planets with different prefab, add more cases for each prefab
				case 1:
					planet = (GameObject)Instantiate(Resources.Load("BasketballPlanet"), p, Quaternion.identity);
					break;
				case 2:
					planet = (GameObject)Instantiate(Resources.Load("IcyPlanet"), p, Quaternion.identity);
					break;
				case 3:
					planet = (GameObject)Instantiate(Resources.Load("IcyPlanet"), p, Quaternion.identity);
					break;
				case 4:
					planet = (GameObject)Instantiate(Resources.Load("GoldPlanet"), p, Quaternion.identity);
					break;
				case 5:
					planet = (GameObject)Instantiate(Resources.Load("LavaPlanet"), p, Quaternion.identity);
					break;
				case 6:
					planet = (GameObject)Instantiate(Resources.Load("MetalPlanet"), p, Quaternion.identity);
					break;
				case 7:
					planet = (GameObject)Instantiate(Resources.Load("RockyPlanet"), p, Quaternion.identity);
					break;
				case 8:
					planet = (GameObject)Instantiate(Resources.Load("TropicalPlanet"), p, Quaternion.identity);
					break;
			}
			// GameObject newPlanet = (GameObject)Instantiate(planet, p, Quaternion.identity);
			// newPlanet.transform.parent = gameObject.transform;
			planet.transform.parent = gameObject.transform;
            // newPlanet.GetComponent<CircleCollider2D>().isTrigger = true;
            ++planetNum;
        }
    }
}
