using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour 
{
    // lazy singleton class
    public static AudioManager instance { get; private set; }
    public AudioClip[] sounds;
    private Dictionary<string, AudioClip> clips;

    void Awake() 
	{
        instance = this;
        // make dictionary of audio clips and their names (name is exact filename string)
        clips = new Dictionary<string, AudioClip>();
        for (int i = 0; i < sounds.Length; ++i) 
		{
            clips.Add(sounds[i].name, sounds[i]);
        }
    }

    // creates a new audio source that plays sound clip at specified position and volume
    public void playSound(string name, Vector3 pos, float vol) 
	{
        AudioSource.PlayClipAtPoint(clips[name], pos, vol);
    }
}
