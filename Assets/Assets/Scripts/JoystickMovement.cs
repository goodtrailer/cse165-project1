using UnityEngine;

public class JoystickMovement : MonoBehaviour
{
    public OVRInput.Controller moveController;
    public OVRInput.Controller lookController;

    public GameObject anchor;

    public float moveSpeed = 1.0f;
    // public float lookSpeed = 120.0f;

    private void FixedUpdate()
    {
        Vector2 moveInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, moveController);
        float moveMagnitude = moveInput.magnitude;
        Vector3 moveRawDir = anchor.transform.rotation * new Vector3(moveInput.x, 0, moveInput.y);
        moveRawDir.y = 0;
        Vector3 moveDir = moveMagnitude * moveMagnitude * moveRawDir.normalized;
        transform.Translate(moveSpeed * moveDir * Time.fixedDeltaTime, Space.Self);

        // float lookDir = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, lookController).x;
        // transform.Rotate(Vector3.up * lookSpeed * lookDir * Time.fixedDeltaTime, Space.Self);
    }
}
