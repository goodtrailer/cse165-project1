using UnityEngine;

public class FollowController : MonoBehaviour
{
    [SerializeField]
    private OVRInput.Controller controller;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = OVRInput.GetLocalControllerPosition(controller);
        transform.localRotation = OVRInput.GetLocalControllerRotation(controller);
    }
}
