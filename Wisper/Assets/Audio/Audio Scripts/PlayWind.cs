﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayWind: MonoBehaviour
{
    public AudioClip soundEffect;

    public AudioSource MusicSource;

    // Start is called before the first frame update
    void Start()
    {
        MusicSource.clip = soundEffect;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
            MusicSource.Play();
        if (Input.GetKeyUp(KeyCode.LeftShift))
            MusicSource.Stop();
    }
}
