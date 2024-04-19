using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoGoHand : MonoBehaviour
{
    public OVRInput.Controller controller;
    public GameObject locationPrefab;

    public float distanceThreshold = 1f;
    public float fastSlope = 50f;

    public float grabForce = 10f;

    private GameObject centerEyeAnchor;
    private GameObject controllerAnchor;
    private GameObject location;
    private Renderer locationRenderer;
    private GoGoHandCollider locationCollider;

    private Rigidbody currentRigidbody;
    private Vector3 currentRigidbodyOffset;

    void Start()
    {
        controllerAnchor = GameObject.Find(controller == OVRInput.Controller.LTouch ? "LeftHandAnchor" : "RightHandAnchor");
        centerEyeAnchor = GameObject.Find("CenterEyeAnchor");
        location = Instantiate(locationPrefab, transform);
        locationRenderer = location.GetComponentInChildren<Renderer>();
        locationCollider = location.GetComponentInChildren<GoGoHandCollider>();
    }

    
    void Update()
    {
        Vector3 localOffset = controllerAnchor.transform.localPosition - centerEyeAnchor.transform.localPosition;
        float distance = localOffset.magnitude;

        if (distance > distanceThreshold)
        {
            locationRenderer.enabled = true;
            foreach (Renderer renderer in controllerAnchor.GetComponentsInChildren<Renderer>())
                renderer.enabled = false;

            localOffset += localOffset.normalized * fastSlope * (distance - distanceThreshold);
        }
        else
        {
            locationRenderer.enabled = false;
            foreach (Renderer renderer in controllerAnchor.GetComponentsInChildren<Renderer>())
                renderer.enabled = true;
        }

        location.transform.localPosition = centerEyeAnchor.transform.localPosition + localOffset;

        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, controller))
        {
            currentRigidbody = locationCollider.ClosestIntersection?.GetComponent<Rigidbody>();
            if (currentRigidbody != null)
                currentRigidbodyOffset = currentRigidbody.transform.position - locationCollider.transform.position;
        }

        if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, controller))
            currentRigidbody = null;
    }

    void FixedUpdate()
    {
        if (currentRigidbody != null)
        {
            Vector3 target = location.transform.position + currentRigidbodyOffset;
            Vector3 dir = target - currentRigidbody.transform.position;
            if (dir.magnitude > 0.2f)
            {
                currentRigidbody.AddForce(-2f * currentRigidbody.velocity);
                currentRigidbody.AddForce(grabForce * dir.normalized);
            }
            else
            {
                currentRigidbody.MovePosition(target);
            }
        }
    }
}
