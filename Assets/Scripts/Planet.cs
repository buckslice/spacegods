using UnityEngine;
using System.Collections;

public class Planet : MonoBehaviour {
	
	public GameObject muhThang;

	// Use this for initialization
	void Start () {
		Instantiate (muhThang, new Vector3(0, 0, 0), Quaternion.identity);
	}
	
	// Update is called once per frame
	void Update () {

	}
}
