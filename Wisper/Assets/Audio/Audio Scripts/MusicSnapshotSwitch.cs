﻿using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class MusicSnapshotSwitch : MonoBehaviour {

	public AudioMixerSnapshot mySnapshot;
    public AudioMixerSnapshot mySnapshot2;
	public float fadeTime = 3.0f;
    public float delayTime = 0.0f;
    private AudioSource myAudioSource;
    public AudioSource L2AudioSource;
    private bool needsToBeFaded = false;
    private float timeToStop = 0.0f;

	// Use this for initialization
	void Start () {
        myAudioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        if(myAudioSource.isPlaying && needsToBeFaded){
            if(Time.time > timeToStop){
                myAudioSource.Stop();
                needsToBeFaded = false;
                //Debug.Log("audio Stop clip: " + myAudioSource.clip.name);

            }

        }
	
	}

	void OnTriggerEnter () {
        myAudioSource.Play();
        L2AudioSource.Play();
        //MusicSnapshotSwitchL2.L2AudioScript.myL2AudioSource 
		mySnapshot.TransitionTo(fadeTime);
	}

    void OnTriggerExit()
    {
        mySnapshot2.TransitionTo(fadeTime);
        timeToStop = Time.time + fadeTime + 1.0f;
        needsToBeFaded = true;
        //Debug.Log("audio OnTriggerExit clip: " + myAudioSource.clip.name);
    }
}
