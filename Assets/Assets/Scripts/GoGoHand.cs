using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoGoHand : MonoBehaviour
{
    public OVRInput.Controller selectionController;
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
    private Outline controllerOutline;
    private bool controllerHasLoaded = false;

    private Rigidbody currentRigidbody;
    private Outline currentOutline;

    private RayVisual rayVisual;
    private bool isUsingRay;
    private Vector3 initialControllerPosition;
    private Vector3 initialRigidbodyPosition;

    // scale
    private Vector3 initialAnchorPosition;
    private Vector3 initialSecondaryAnchorPosition;
    private bool isScaling;
    private Vector3 initialScale;

    private Vector3 initialCenterEyePosition;

    void Start()
    {
        centerEyeAnchor = GameObject.Find("CenterEyeAnchor");
        controllerAnchor = GameObject.Find(selectionController == OVRInput.Controller.LTouch ? "LeftHandAnchor" : "RightHandAnchor");
        secondaryAnchor = GameObject.Find(selectionController == OVRInput.Controller.LTouch ? "RightHandAnchor" : "LeftHandAnchor");
        controllerVisual = GameObject.Find(selectionController == OVRInput.Controller.LTouch ? "LeftOVRRuntimeController" : "RightOVRRuntimeController");
        secondaryVisual = GameObject.Find(selectionController == OVRInput.Controller.LTouch ? "RightOVRRuntimeController" : "LeftOVRRuntimeController");
        controllerCollider = controllerVisual.GetComponent<GoGoHandCollider>();

        rayVisual = GetComponent<RayVisual>();
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

        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, selectionController))
        {
            rayVisual.IsShowing = true;
            rayVisual.Color = Color.yellow;
            isUsingRay = true;
        }

        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, selectionController))
        {
            rayVisual.IsShowing = false;
            isUsingRay = false;
        }

        Vector3 localOffset = controllerAnchor.transform.localPosition - centerEyeAnchor.transform.localPosition;
        Vector3 localSecondaryOffset = secondaryAnchor.transform.localPosition - centerEyeAnchor.transform.localPosition;
        float distance = localOffset.magnitude;

        if (!isUsingRay)
        {
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
        }

        controllerVisual.transform.localPosition = centerEyeAnchor.transform.localPosition + localOffset;
        controllerVisual.transform.localRotation = controllerAnchor.transform.localRotation;
        controllerVisual.transform.Rotate(-60f * Vector3.right, Space.Self);

        secondaryVisual.transform.localPosition = centerEyeAnchor.transform.localPosition + localSecondaryOffset;
        secondaryVisual.transform.localRotation = secondaryAnchor.transform.localRotation;
        secondaryVisual.transform.Rotate(-60f * Vector3.right, Space.Self);

        // Selection

        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, selectionController))
        {

            initialControllerPosition = controllerVisual.transform.position;
            initialCenterEyePosition = centerEyeAnchor.transform.position;

            if (isUsingRay)
            {
                currentRigidbody = rayVisual.Info.collider?.GetComponent<Rigidbody>();

                if (currentRigidbody != null)
                    rayVisual.IsShowing = false;
            }
            else
            {
                currentRigidbody = controllerCollider.ClosestIntersection?.GetComponent<Rigidbody>();
            }

            if (currentRigidbody != null)
            {
                currentRigidbody.freezeRotation = true;
                initialRigidbodyPosition = currentRigidbody.position;

                // Outline on select

                if (currentOutline != null)
                    currentOutline.enabled = false;
                currentOutline = null;
                if (currentRigidbody.GetComponent<Outline>() is Outline outline)
                {
                    outline.enabled = true;
                    outline.OutlineColor = Color.yellow;
                }
            }
        }

        if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, selectionController) && currentRigidbody != null)
        {
            // Un-outline on deselect

            if (currentRigidbody?.GetComponent<Outline>() is Outline outline)
                outline.enabled = false;

            if (isUsingRay)
                rayVisual.IsShowing = true;

            currentRigidbody.freezeRotation = false;
            currentRigidbody = null;

        }

        // scale stuff

        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, rotateController) && currentRigidbody != null)
        {
            initialAnchorPosition = controllerAnchor.transform.position;
            initialSecondaryAnchorPosition = secondaryAnchor.transform.position;
            isScaling = true;

            initialScale = currentRigidbody.transform.localScale;
        }

        if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, rotateController))
        {
            isScaling = false;
        }


        // Outline on hover

        if (currentRigidbody == null)
        {
            if (currentOutline != null)
                currentOutline.enabled = false;

            currentOutline = isUsingRay
                ? rayVisual.Info.collider?.GetComponent<Outline>()
                : controllerCollider.ClosestIntersection?.GetComponent<Outline>();

            if (currentOutline != null)
            {
                currentOutline.enabled = true;
                currentOutline.OutlineColor = new Color(0f, 0.8703723f, 1f, 1f);
                currentOutline.UpdateMaterialProperties();
            }
        }
    }

    void FixedUpdate()
    {
        if (currentRigidbody != null)
        {

            Vector3 target = isUsingRay
                ? 10 * (controllerVisual.transform.position - initialControllerPosition - centerEyeAnchor.transform.position + initialCenterEyePosition) + centerEyeAnchor.transform.position - initialCenterEyePosition + initialRigidbodyPosition
                : controllerVisual.transform.position - initialControllerPosition + initialRigidbodyPosition;
            currentRigidbody.MovePosition(target);

            // scale
            if (isScaling)
            {
                float initDist = (initialAnchorPosition - initialSecondaryAnchorPosition).magnitude;
                float newDist = (controllerAnchor.transform.position - secondaryAnchor.transform.position).magnitude;
                float scaleMag = newDist / initDist;

                currentRigidbody.transform.localScale = scaleMag * initialScale;
            }



            // rotation



            //Vector2 rotateInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, rotateController);
            //Quaternion rotation;
            //rotateInput *= Time.fixedDeltaTime * rotateSpeed;
            //if (Mathf.Abs(rotateInput.x) > Mathf.Abs(rotateInput.y))
            //{
            //    rotation = Quaternion.AngleAxis(-rotateInput.x, Vector3.up);
            //}
            //else
            //{
            //    rotation = Quaternion.AngleAxis(rotateInput.y,
            //                                    centerEyeAnchor.transform.localToWorldMatrix * Vector3.right);
            //}
            //currentRigidbody.MoveRotation(rotation * currentRigidbody.rotation);
        }
    }
}
