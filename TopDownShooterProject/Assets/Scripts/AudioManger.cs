using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManger : MonoBehaviour
{
    public new AudioSource audio;
    public List<SoundClip> soundClips; 
    public static AudioManger instance;

    public void Awake()
    {
        instance = this;
    }

    public void Play(string name)
    {
        foreach (SoundClip clip in soundClips)
        {
            if (clip.clip.name == name)
            {
                StartCoroutine(audioPlayer(clip.clip));
                break;
            }
        }

        Debug.Log(name + " audio clip not found in audio libary");
    }

    IEnumerator audioPlayer(AudioClip clip)
    {
        audio.PlayOneShot(clip);
        yield return new WaitForSeconds(audio.clip.length);
    }
}

[System.Serializable]
public class SoundClip
{
    public AudioClip clip;    
}