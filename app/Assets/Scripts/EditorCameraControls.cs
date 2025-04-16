#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.SpatialTracking;

public class EditorCameraControls : MonoBehaviour
{
    public float moveSpeed = 2.0f;
    public float lookSpeed = 2.0f;

    private float yaw;
    private float pitch;

    private void Awake()
    {
        // Disable TrackedPoseDriver if present (Editor only)
        var poseDriver = GetComponent<TrackedPoseDriver>();
        if (poseDriver != null)
        {
            poseDriver.enabled = false;
        }
    }

    void Update()
    {
        // Mouse look with right mouse button
        if (Input.GetMouseButton(1))
        {
            yaw += lookSpeed * Input.GetAxis("Mouse X");
            pitch -= lookSpeed * Input.GetAxis("Mouse Y");
            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }

        // WASD + QE movement
        Vector3 dir = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) dir += transform.forward;
        if (Input.GetKey(KeyCode.S)) dir -= transform.forward;
        if (Input.GetKey(KeyCode.A)) dir -= transform.right;
        if (Input.GetKey(KeyCode.D)) dir += transform.right;
        if (Input.GetKey(KeyCode.Q)) dir -= transform.up;
        if (Input.GetKey(KeyCode.E)) dir += transform.up;

        transform.position += dir * moveSpeed * Time.deltaTime;
    }
}
#endif
