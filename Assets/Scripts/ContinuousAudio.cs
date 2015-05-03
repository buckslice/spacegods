using UnityEngine;
using System.Collections;

public class ContinuousAudio : MonoBehaviour 
{
    private static ContinuousAudio instance = null;
    public static ContinuousAudio Instance 
	{
        get { return Instance; }
    }

	// use this for initialization
    void Awake() 
	{
        if (instance != null && instance != this) 
		{
            Destroy(this.gameObject);
            return;
        } 
		else 
		{
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
}

