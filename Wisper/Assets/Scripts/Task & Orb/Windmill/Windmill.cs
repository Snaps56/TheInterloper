﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Windmill : MonoBehaviour {

    public GameObject abilitiesCollider;
    public float torqueMultiplier = 5f;
    public float baseSpeed = 10f;
    public float dangerSpeed = 6f;
    public float correctSpeed = 2f;
    public GameObject[] windmillParts;

    public GameObject fixedWing3;
    public GameObject fixedWing5;

    public GameObject brokenWing3;
    public GameObject brokenWing5;

    public GameObject testLocation;

    private int attachCount = 0;
    private readonly int totalToFix = 2;

    private Rigidbody rb;

    private bool isThrowing;
    private bool isLifting;

    private float torque;
    private bool isWithinRange = false;
    private bool hasSpawnedOrbs = false;
    private float currentVelocity;
    private float maxVelocity = 1.5f;
    private PersistantStateData persistantStateData;


    // Use this for initialization
    void Start () {
        persistantStateData = PersistantStateData.persistantStateData;
        rb = GetComponent<Rigidbody>();
        hasSpawnedOrbs = (bool)persistantStateData.stateConditions["WindmillSpawnedOrbs"];

    }
	
	// Update is called once per frame
	void Update () {
        isLifting = abilitiesCollider.GetComponent<ObjectLift>().GetIsLiftingObjects();
        isThrowing = abilitiesCollider.GetComponent<ObjectThrow>().GetIsThrowingObjects();

        //Debug.Log("current velocity: " + currentVelocity + ", danger speed: " + dangerSpeed);

        //If the windmill is fixed
        if (attachCount == totalToFix)
        {
            rb.isKinematic = false;
            Debug.Log("Windmill fixed. Need to push!");

            //If the PSD Variable WindmillFixed isn't set to true, set it to true
            if ((bool)persistantStateData.stateConditions["WindmillFixed"] == false)
            {
                persistantStateData.ChangeStateConditions("WindmillFixed", true);
            }
            //Check if the player is trying to move the windmill
            if ((isLifting || isThrowing) && isWithinRange)
            {
                Debug.Log("Player is attempting to push fixed windmill!");
                //Set the torque to move the windmill
                torque = abilitiesCollider.GetComponent<ObjectThrow>().GetThrowForce();
            }
            //If the player isn't trying to move the windmill and the task isn't done, set torque to 0
            else if ((bool)persistantStateData.stateConditions["WindmillTaskDone"] == false)
            {
                Debug.Log("Player gave up pushing fixed windmill!");
                torque = 0;
            }
            currentVelocity = rb.angularVelocity.y;

            //Break the windmill if the player reached max orbs
            if ((bool)persistantStateData.stateConditions["HasReachedMax"] == true && Input.GetKeyDown(KeyCode.K))
            {
                //rb.isKinematic = true;
                attachCount = 0;
                persistantStateData.ChangeStateConditions("WindmillFixed", false);
                persistantStateData.ChangeStateConditions("WindmillTaskDone", false);
                fixedWing3.SetActive(false);
                fixedWing5.SetActive(false);
                brokenWing3.SetActive(true);
                brokenWing5.SetActive(true);

                //TODO:
                //RESET ATTACHED STATES OF BROKEN WINGS

                Instantiate(windmillParts[0], testLocation.transform.position, Quaternion.identity);
                Instantiate(windmillParts[1], testLocation.transform.position, Quaternion.identity);
                Debug.Log("Player pushed windmill too fast and broke it");
                for (int i = 0; i < windmillParts.Length; i++)
                {
                    //Vector3 currentShellsterVelocity = windmillParts[i].GetComponent<Rigidbody>().velocity;
                    windmillParts[i].GetComponent<Rigidbody>().AddExplosionForce(150, windmillParts[i].transform.position, 5f);
                    Debug.Log("detached " + windmillParts[i]);
                }
            }
            //Windmill task is done! 
            else if (currentVelocity >= correctSpeed && (bool)persistantStateData.stateConditions["HasReachedMax"] == false)
            {
                if (!hasSpawnedOrbs)
                {
                    persistantStateData.ChangeStateConditions("WindmillSpawnedOrbs", true);
                    GetComponent<SpawnOrbs>().DropOrbs();
                    hasSpawnedOrbs = true;
                }
                Debug.Log("Player pushed windmill at correct speed and finished task.");
                persistantStateData.ChangeStateConditions("WindmillTaskDone",true);
                //Debug.Log("WindmillTaskDone: " + (bool)persistantStateData.stateConditions["WindmillTaskDone"]);
            }
        }
        else
        {
            torque = 0;
        }
    }
    private void FixedUpdate()
    {
        //If the taks isn't done, allow the user to move the windmill
        if ((bool)persistantStateData.stateConditions["WindmillTaskDone"] == false)
        {
            Debug.Log("Turning Windmill.");
            rb.AddTorque(-torque * torqueMultiplier, 0, 0);
        }
        //If task is done, add constant passive force
        else
        {
            Debug.Log("Windmill task is done. Add passive rotation to it.");
            if (currentVelocity < maxVelocity)
            {
                rb.AddTorque(-baseSpeed, 0, 0);

            }
            //Debug.Log("current velocity: " + currentVelocity);

        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Abilities Collider")
        {
            isWithinRange = true;
            //Debug.Log("Within Range");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Abilities Collider")
        {
            isWithinRange = false;
            //Debug.Log("Exited Range");
        }
    }

    public void IncrementAttachCounter()
    {
        attachCount++;
    }
}
