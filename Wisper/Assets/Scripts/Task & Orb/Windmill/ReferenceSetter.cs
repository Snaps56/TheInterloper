﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceSetter : MonoBehaviour {

    /*****Purpose of this script *****/
    // This script was created to reduce the amount of checks the destroyed windmill pieces
    // need to make to obtain the proper references it needs.

    private GameObject destroyedPart;
    // Use this for initialization
    void Start () {
        //Debug.Log("The name of the part: " + this.gameObject.name);
        if(this.gameObject.name.Contains("3"))
        {
            Debug.Log("Attaching the wing");
            destroyedPart = GameObject.Find("broken_wing_3");
            destroyedPart.GetComponent<AttachToWindmill>().brokenPart = this.gameObject;
        }
        else if (this.gameObject.name.Contains("5"))
        {
            destroyedPart = GameObject.Find("broken_wing_5");
            destroyedPart.GetComponent<AttachToWindmill>().brokenPart = this.gameObject;
        }
    }
}
