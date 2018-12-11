﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Searching : MonoBehaviour {

    private float RandomNum;
    public NPCMovement NPCMovements;
    private bool playedAnimation;
    private Animator animator;
    private int count;
    private void Start()
    {
        animator = GetComponent<Animator>();
        playedAnimation = false;

    }
    // Update is called once per frame

    void OnTriggerEnter(Collider other)
    {
        //when touches the waypoints, roll a dice between 0 and 100
        if(other.gameObject.tag == "WayPoint")
        {
            RandomNum = Random.Range(0, 100);
           // Debug.Log("random: " + RandomNum);
            if (RandomNum > 50)
            {
                if (playedAnimation == false)
                {
                    NPCMovements.move = false;
                    animator.SetBool("Searching", true);
                    playedAnimation = true;
                    StartCoroutine(Search());
                }
            }
        }
        
    }

    IEnumerator Search()
    {
        //Debug.Log("Search");
        
        if(playedAnimation == true)
        {
            yield return new WaitForSeconds(6.0f);
            //Debug.Log("isSearching");
            animator.SetBool("Searching", false);
            NPCMovements.move = true;
            playedAnimation = false;
            Debug.Log(NPCMovements.move);
        }
    }
}
