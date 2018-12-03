﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMovementCondition : MonoBehaviour {

    public GameObject player;
    // variables relating to the player's distance
    public float distanceTravelRequired;
    private float playerDistanceTraveled;

    private TutorialCondition tutorialCondition;

    // Use this for initialization
    void Start () {
        tutorialCondition = GetComponent<TutorialCondition>();
        playerDistanceTraveled = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
        // player distance travelled
        playerDistanceTraveled += player.GetComponent<Rigidbody>().velocity.magnitude * Time.deltaTime;

        // if player has traveled the required distance, update tutorial condition
        if (playerDistanceTraveled > distanceTravelRequired)
        {
            tutorialCondition.SetCondition(true);
        }
    }
}
