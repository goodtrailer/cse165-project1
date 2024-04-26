using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Spawn : MonoBehaviour
{
    public OVRInput.Controller controller;

    public List<GameObject> prefabs;

    public float hologramDistance = 5f;
    public float hologramGap = 5f;

    private List<GameObject> holograms = new List<GameObject>();
    private Dictionary<GameObject, EventHandler> unhologramHandlers = new Dictionary<GameObject, EventHandler>();

    private GameObject centerEyeAnchor;

    private bool isShowingHolograms = false;

    void Start()
    {
        centerEyeAnchor = GameObject.Find("CenterEyeAnchor");
    }

    void Unhologram(GameObject toUnhologram)
    {
        // update outlined
        if (toUnhologram.GetComponent<Outlined>() is Outlined outlined)
        {
            outlined.OutlineEnabled = false;
            outlined.OutlineMode = Outline.Mode.OutlineAndSilhouette;
        }

        if (toUnhologram.GetComponent<Manipulatable>() is Manipulatable manipulatable)
        {
            manipulatable.OnSelected -= unhologramHandlers[toUnhologram];
            unhologramHandlers.Remove(toUnhologram);
        }

        // give physics
        if (toUnhologram.GetComponent<Rigidbody>() is Rigidbody body)
        {
            body.useGravity = true;
        }
        if (toUnhologram.GetComponent<Collider>() is Collider collider)
        {
            collider.isTrigger = false;
        }

        // destroy 
        holograms.Remove(toUnhologram);
        foreach (GameObject hologram in holograms)
            Destroy(hologram);
        holograms.Clear();

        isShowingHolograms = false;
    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One, controller))
        {
            if (!isShowingHolograms)
            {
                for (int i = 0; i < prefabs.Count; i++)
                {
                    Vector3 forward = centerEyeAnchor.transform.forward;
                    forward.y = 0f;
                    forward = forward.normalized * hologramDistance;

                    Vector3 right = centerEyeAnchor.transform.right;
                    right.y = 0f;
                    right = right.normalized * hologramGap;

                    Vector3 position = centerEyeAnchor.transform.position + forward + (i + 0.5f - prefabs.Count * 0.5f) * right;

                    GameObject hologram = Instantiate(prefabs[i], position, Quaternion.identity);

                    if (hologram.GetComponent<Rigidbody>() is Rigidbody rigidbody)
                        rigidbody.useGravity = false;

                    if (hologram.GetComponent<Collider>() is Collider collider)
                        collider.isTrigger = true;

                    if (hologram.GetComponent<Outlined>() is Outlined outlined)
                    {
                        outlined.OutlineEnabled = true;
                        outlined.OutlineColor = Color.white;
                        outlined.OutlineMode = Outline.Mode.SilhouetteAlways;
                        outlined.UpdateOutline();
                    }

                    if (hologram.GetComponent<Manipulatable>() is Manipulatable manipulatable)
                    {
                        EventHandler handler = (_, _) => Unhologram(hologram);
                        manipulatable.OnSelected += handler;
                        unhologramHandlers.Add(hologram, handler);
                    }

                    holograms.Add(hologram);
                }

                isShowingHolograms = true;
            }
            else
            {
                foreach (GameObject hologram in holograms)
                    Destroy(hologram);

                holograms.Clear();
                isShowingHolograms = false;
            }
        }
    }
}
