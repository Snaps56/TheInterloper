﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCMovement : MonoBehaviour
{

    public GameObject player;
    public GameObject hat;
    public GameObject npc;
    public GameObject[] waypoints;
    int currentWP = 0;
    public float rotSpeed = 3.0f;
    public float speed = 0.5f;
    public float accuracyWP = 1.0f;
    public float detection = 5.0f;
    public float stopLength = 5.0f;

    public bool move = true;
    Vector3 npcPos;
    Vector3 hatPos;
    Animator animator;
    //NavMeshAgent agent;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player" && (Input.GetButton("PC_Key_Interact") || Input.GetButton("XBOX_Button_X")))
        {
            //move = false;
            //animator.SetBool("Idle", true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
       // move = true;
       if(move == true)
        {
            animator.SetBool("Idle", false);
        }
        
    }
    // Update is called once per frame
    void Update()
    {     
        //sets the last waypoint to the hat
        //waypoints[5].transform.position = hat.transform.position;
        Vector3 direction = player.transform.position - this.transform.position;
        if (hat != null)
        {
            Vector3 distanceFromHat = npc.transform.position - hat.transform.position;
        }
        //Movement loop
        if (waypoints.Length > 0)
        {
            if (Vector3.Distance(waypoints[currentWP].transform.position, transform.position) < accuracyWP)
            {
                currentWP++;
				if (currentWP >= waypoints.Length) {
					currentWP = 0;
                }
                //When he detects the hat
               if ((hat != null && Vector3.Distance(transform.position, hat.transform.position) < detection))
                {
                    //Go to last
                    //currentWP = waypoints.Length - 1;

                    Debug.Log("OBTAINED HAT");

                    //Stop in place
                    GetComponent<Searching>().enabled = false;
                    move = false;
                    Debug.Log(move);
                    if(move == false)
                    {
                        animator.SetBool("Idle", true);
                        Debug.Log("Stop!!");
                        GetComponent<SpawnOrbs>().DropOrbs();
                    }                   
                                        
                }
            }
            //if (/*currentWP == 5 && */ distanceFromHat.magnitude <= stopLength)
            //{
            //    move = false;
            //    direction = waypoints[currentWP].transform.position - transform.position;
            //    this.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotSpeed * Time.deltaTime);
            //}
        }

        //player movement
        if (move == true)
        {
            direction = waypoints[currentWP].transform.position - transform.position;
            this.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotSpeed * Time.deltaTime);
            this.transform.Translate(0, 0, Time.deltaTime * speed);
        }
	}


}