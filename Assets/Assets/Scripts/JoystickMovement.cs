using UnityEngine;

public class JoystickMovement : MonoBehaviour
{
    public OVRInput.Controller moveController;

    public GameObject parent;
    private GameObject centerEyeAnchor;

    public float moveSpeed = 1.0f;
    // public float lookSpeed = 120.0f;

    private void Start()
    {
        centerEyeAnchor = GameObject.Find("CenterEyeAnchor");
    }

    private void FixedUpdate()
    {
        Vector2 moveInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, moveController);
        float moveMagnitude = moveInput.magnitude;
        Vector3 moveRawDir = centerEyeAnchor.transform.rotation * new Vector3(moveInput.x, 0, moveInput.y);
        moveRawDir.y = 0;
        Vector3 moveDir = moveMagnitude * moveMagnitude * moveRawDir.normalized;
        parent.transform.Translate(moveSpeed * moveDir * Time.fixedDeltaTime, Space.Self);

    }
}
