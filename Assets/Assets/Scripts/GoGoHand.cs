using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoGoHand : MonoBehaviour
{
    public OVRInput.Controller gogoController;
    public OVRInput.Controller rotateController;

    public float distanceThreshold = 1f;
    public float fastSlope = 50f;

    public float grabForce = 10f;

    public float rotateSpeed = 90f;

    private GameObject centerEyeAnchor;
    private GameObject controllerAnchor;
    private GameObject secondaryAnchor;
    private GameObject controllerVisual;
    private GameObject secondaryVisual;

    private GoGoHandCollider controllerCollider;

    private Rigidbody currentRigidbody;
    private Vector3 currentRigidbodyOffset;

    private Outline controllerOutline;

    private bool controllerHasLoaded = false;

    void Start()
    {
        centerEyeAnchor = GameObject.Find("CenterEyeAnchor");
        controllerAnchor = GameObject.Find(gogoController == OVRInput.Controller.LTouch ? "LeftHandAnchor" : "RightHandAnchor");
        secondaryAnchor = GameObject.Find(gogoController == OVRInput.Controller.LTouch ? "RightHandAnchor" : "LeftHandAnchor");
        controllerVisual = GameObject.Find(gogoController == OVRInput.Controller.LTouch ? "LeftOVRRuntimeController" : "RightOVRRuntimeController");
        secondaryVisual = GameObject.Find(gogoController == OVRInput.Controller.LTouch ? "RightOVRRuntimeController" : "LeftOVRRuntimeController");
        controllerCollider = controllerVisual.GetComponent<GoGoHandCollider>();
    }

    
    void Update()
    {
        if (!controllerHasLoaded)
        {
            if (controllerVisual.GetComponentInChildren<Renderer>() is Renderer renderer)
            {
                controllerOutline = renderer.gameObject.AddComponent<Outline>();
                controllerOutline.OutlineMode = Outline.Mode.OutlineAndSilhouette;
                controllerOutline.OutlineColor = Color.yellow;
                controllerOutline.OutlineWidth = 4f;
                controllerOutline.enabled = false;
                controllerHasLoaded = true;
            }
        }

        Vector3 localOffset = controllerAnchor.transform.localPosition - centerEyeAnchor.transform.localPosition;
        Vector3 localSecondaryOffset = secondaryAnchor.transform.localPosition - centerEyeAnchor.transform.localPosition;
        float distance = localOffset.magnitude;

        if (distance > distanceThreshold)
        {
            localOffset += localOffset.normalized * fastSlope * (distance - distanceThreshold);

            if (controllerOutline != null)
                controllerOutline.enabled = true;
        }
        else
        {
            if (controllerOutline != null)
                controllerOutline.enabled = false;
        }

        controllerVisual.transform.localPosition = centerEyeAnchor.transform.localPosition + localOffset;
        controllerVisual.transform.localRotation = controllerAnchor.transform.localRotation;
        controllerVisual.transform.Rotate(-60f * Vector3.right, Space.Self);

        secondaryVisual.transform.localPosition = centerEyeAnchor.transform.localPosition + localSecondaryOffset;
        secondaryVisual.transform.localRotation = secondaryAnchor.transform.localRotation;
        secondaryVisual.transform.Rotate(-60f * Vector3.right, Space.Self);

        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, gogoController))
        {
            currentRigidbody = controllerCollider.ClosestIntersection?.GetComponent<Rigidbody>();
            if (currentRigidbody != null)
            {
                currentRigidbody.freezeRotation = true;
                currentRigidbodyOffset = currentRigidbody.transform.position - controllerCollider.transform.position;
            }
        }

        if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, gogoController) && currentRigidbody != null)
        {
            currentRigidbody.freezeRotation = false;
            currentRigidbody = null;
        }
    }

    void FixedUpdate()
    {
        if (currentRigidbody != null)
        {
            Vector3 target = controllerVisual.transform.position + currentRigidbodyOffset;
            Vector3 dir = target - currentRigidbody.transform.position;
            if (false && dir.magnitude > 0.2f)
            {
                currentRigidbody.AddForce(-2f * currentRigidbody.velocity);
                currentRigidbody.AddForce(grabForce * dir.normalized);
            }
            else
            {
                currentRigidbody.MovePosition(target);
            }


            // rotation

            Vector2 rotateInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, rotateController);
            Quaternion rotation;
            rotateInput *= Time.fixedDeltaTime * rotateSpeed;
            if (Mathf.Abs(rotateInput.x) > Mathf.Abs(rotateInput.y))
            {
                rotation = Quaternion.AngleAxis(-rotateInput.x, Vector3.up);
            }
            else
            {
                rotation = Quaternion.AngleAxis(rotateInput.y,
                                                centerEyeAnchor.transform.localToWorldMatrix * Vector3.right);
            }
            currentRigidbody.MoveRotation(rotation * currentRigidbody.rotation);

        }
    }
}
