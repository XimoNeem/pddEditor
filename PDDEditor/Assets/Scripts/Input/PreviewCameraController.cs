using Unity.VisualScripting;
using UnityEngine;

public class PreviewCameraController : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField] private Camera _camera;

    [SerializeField]
    private float rotationSpeed = 5f;

    [SerializeField]
    private float zoomSpeed = 5f;

    [SerializeField]
    private float minDistance = 1f;

    [SerializeField]
    private float maxDistance = 10f;

    private float xRotation = 0f;
    private float yRotation = 0f;


    void LateUpdate()
    {
        if (!Input.GetMouseButton(1)) { return; }

        float zoomInput = -Input.GetAxis("Mouse ScrollWheel");
        if (zoomInput != 0f)
        {
            _camera.transform.localPosition = new Vector3(0, 0, -Mathf.Clamp((zoomInput * zoomSpeed) - _camera.transform.localPosition.z, minDistance, maxDistance));
        }

        xRotation += Input.GetAxis("Mouse X") * rotationSpeed;
        yRotation -= Input.GetAxis("Mouse Y") * rotationSpeed;
        yRotation = Mathf.Clamp(yRotation, -90f, 90f);

        Quaternion rotation = Quaternion.Euler(yRotation, xRotation, 0f);

        transform.rotation = rotation;
    }
}
