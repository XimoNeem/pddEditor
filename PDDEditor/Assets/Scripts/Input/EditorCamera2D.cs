using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorCamera2D : MonoBehaviour
{
    public List<Camera> m_2Dcameras;
    private Vector3 m_CameraPosition;

    public float moveSens = 1;
    public float zoomSens = 1;

    public float maxZoom;
    public float minZoom;

    void Start()
    {
        m_CameraPosition.y = 50;
    }

    void Update()
    {
        if(Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            m_CameraPosition.x -= Input.GetAxis("Mouse X") * moveSens;
            m_CameraPosition.z -= Input.GetAxis("Mouse Y") * moveSens;
        }
        foreach (Camera item in m_2Dcameras)
        {
            item.gameObject.transform.position = m_CameraPosition;
            item.orthographicSize = Mathf.Clamp(item.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * zoomSens, minZoom, maxZoom);
        }
    }
}
