using UnityEngine;
using System.Collections;

public class Planet : MonoBehaviour
{
	int health, sizeSelection;
	float size, mass;

	void Awake() 
	{
		health = 3;
		sizeSelection = Random.Range (0, 3);
		switch (sizeSelection) {
		case 0:
			size = .76f;
			mass = .6f;
			break;
		case 1:
			size = 1.0f;
			mass = 1.0f;
			break;
		case 2:
			size = 1.5f;
			mass = 1.4f;
			break;
		default:
			Debug.LogError("Size for planet out of range");
			break;
				}
	}

	void Update()
	{
		if (health == 0)
			Destroy (this);
		}

	public void damage()
	{
		health--;
		}
	public int getHealth()
	{
		return health;
		}

	public float getSize()
	{
		return size;
		}
	public float getMass()
	{
		return mass;
		}
}
