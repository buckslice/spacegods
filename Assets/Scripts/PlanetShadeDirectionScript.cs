using UnityEngine;
using System.Collections;

public class PlanetShadeDirectionScript : MonoBehaviour 
{
	void Start () 
	{
	
	}

	float ToAngle(float x,float y)
	{
		if(x == 0)
			return y >= 0 ? Mathf.PI/2 : Mathf.PI*3/2;
		
		if(y == 0)
			return x < 0 ? Mathf.PI : 0;
		
		float r = Mathf.Atan(y/x);
		return x < 0 ? Mathf.PI+r : r;
	}

	// update is called once per frame
	void Update () 
	{
		float a = ToAngle(transform.position.x,transform.position.y);
		transform.rotation = Quaternion.Euler(0,0,a*180f/3.1415f+225);
	}
}
