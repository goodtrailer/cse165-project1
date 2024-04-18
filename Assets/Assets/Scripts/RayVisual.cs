using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RayVisual : MonoBehaviour
{
    public OVRInput.Controller controller;

    private LineRenderer lineRenderer;

    public bool IsShowing { get; set; } = false;

    public RaycastHit Info { get; private set; }

    public Vector3 Origin { get; private set; }

    public Vector3 Direction { get; private set; }

    public Vector3 End => Origin + Direction * (Info.transform == null ? 1000f : Info.distance);

    public Color Color
    {
        get => lineRenderer.material.GetColor("_EmissionColor");
        set
        {
            lineRenderer.material.SetColor("_Color", value.WithAlpha(0.3f));
            lineRenderer.material.SetColor("_EmissionColor", value);
        }
    }

    private void Start()
    {
        lineRenderer = transform.GetComponent<LineRenderer>();
        IsShowing = false;
    }

    void FixedUpdate()
    {
        if (lineRenderer == null)
            return;

        Vector3 localPosition = OVRInput.GetLocalControllerPosition(controller);
        Quaternion localRotation = OVRInput.GetLocalControllerRotation(controller);

        Origin = transform.TransformPoint(localPosition);
        Direction = localRotation * Vector3.forward;

        RaycastHit info;
        Physics.Raycast(Origin, Direction, out info);
        Info = info;

        Vector3 localEnd = transform.InverseTransformPoint(End);

        if (IsShowing)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPositions(new[] {
                localPosition,
                localEnd,
            });
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
    }
}
