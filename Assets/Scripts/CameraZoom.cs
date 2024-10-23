using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    private const float ZOOM_SPEED = 10f;
    private const float MIN_ZOOM = 2.0f;
    private const float MAX_ZOOM = 135.0f;

    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        var scroll = Input.GetAxis("Mouse ScrollWheel");

        cam.orthographicSize -= scroll * ZOOM_SPEED;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, MIN_ZOOM, MAX_ZOOM);
    }
}