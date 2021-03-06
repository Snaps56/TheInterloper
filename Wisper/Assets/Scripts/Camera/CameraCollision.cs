﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour
{

    public float minDistance;
    public float maxDistance;
    public float smooth;
    public float zoomRatio;

    Vector3 cameraDirection;
    float distance;

    // Use this for initialization
    void Awake()
    {
        cameraDirection = transform.localPosition.normalized;
        distance = transform.localPosition.magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 desiredCameraPosition = transform.TransformPoint(cameraDirection * maxDistance);

        RaycastHit hit;

        if (Physics.Linecast(transform.position, desiredCameraPosition, out hit))
        {
            if (!hit.collider.isTrigger && (hit.collider.CompareTag("Terrain") || hit.collider.CompareTag("LargeObject")))
            {
                distance = Mathf.Clamp(hit.distance * zoomRatio, minDistance, maxDistance);
            }
        }
        else
        {
            distance = maxDistance;
        }
        transform.localPosition = Vector3.Lerp(transform.localPosition, cameraDirection * distance, Time.deltaTime * smooth);
    }
}
