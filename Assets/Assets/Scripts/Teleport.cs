using UnityEngine;

public class Teleport : MonoBehaviour
{
    public GameObject parent;
    public GameObject destinationPrefab;

    public OVRInput.Button button;
    public OVRInput.Controller controller;

    private RayVisual rayVisual;

    private bool isActive = false;
    private bool isValid = false;

    private GameObject destination;

    private void Awake()
    {
        rayVisual = transform.GetComponent<RayVisual>();
    }

    private void Start()
    {
        destination = Instantiate(destinationPrefab);
    }

    void Update()
    {
        if (OVRInput.GetDown(button, controller) && !rayVisual.IsShowing)
        {
            isActive = true;
            isValid = true;
            rayVisual.IsShowing = true;
        }
        
        if (OVRInput.GetUp(button, controller) && isActive)
        {
            isActive = false;
            rayVisual.IsShowing = false;
            destination.SetActive(false);

            if (isValid)
                parent.transform.position = rayVisual.End;
        }

        if (isActive)
        {
            if (rayVisual.Info.transform == null || rayVisual.Info.normal.y < 0.9f)
            {
                isValid = false;
                rayVisual.Color = Color.red;
                destination.SetActive(false);
            }
            else
            {
                isValid = true;
                rayVisual.Color = Color.green;
                destination.SetActive(true);
                destination.transform.position = rayVisual.End;
            }
        }
    }
}
