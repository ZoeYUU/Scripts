﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour {
    public OVRInput.Controller controller;

    public string buttonName;

    private GameObject grabbedObject;
    private bool grabbing;

    public float grabRadius;
    public LayerMask grabMask;

    private Quaternion lastRotation, currentRotation;

    void GrabObject()
    {
        grabbing = true;

        RaycastHit[] hits;

        hits = Physics.SphereCastAll(transform.position, grabRadius, transform.forward, 0f, grabMask);

        if (hits.Length > 0)
        {
            int closestHit = 0;
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].distance < hits[closestHit].distance)
                    closestHit = i;
            }
            grabbedObject = hits[closestHit].transform.gameObject;
            grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
            
           
            //grabbedObject.transform.position = transform.position; //if this is on, always grab the same position
            grabbedObject.transform.parent = transform;
        }
    }

    void DropObject()
    {
        grabbing = false;

        if (grabbedObject != null)
        {
            grabbedObject.transform.parent = null;
            grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
            grabbedObject.GetComponent<Rigidbody>().velocity = OVRInput.GetLocalControllerVelocity(controller);
            grabbedObject.GetComponent<Rigidbody>().velocity += new Vector3(100f, 100f, 100f); //upscale thrown-obj's velocity
            grabbedObject.GetComponent<Rigidbody>().angularVelocity = GetAngularVelocity(); //thrown obj's rotation

            Debug.Log(OVRInput.GetLocalControllerVelocity(controller));

            //grabbedObject = null;
        }
    }

    Vector3 GetAngularVelocity()
    {
        Quaternion deltaRotation = currentRotation * Quaternion.Inverse(lastRotation); //matrix-change: changed angle
        return new Vector3(Mathf.DeltaAngle(0, deltaRotation.eulerAngles.x), Mathf.DeltaAngle(0, deltaRotation.eulerAngles.y), Mathf.DeltaAngle(0, deltaRotation.eulerAngles.z));
    }


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (grabbedObject != null)
        {
            lastRotation = currentRotation;
            currentRotation = grabbedObject.transform.rotation;
        }

        if (!grabbing && Input.GetAxis(buttonName) == 1)
            GrabObject();
        if (grabbing && Input.GetAxis(buttonName) < 1)
            DropObject();

    }
}
