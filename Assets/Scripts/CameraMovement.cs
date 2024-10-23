using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private const int MAX_Y = 269;
    private const int MAX_X = 479;

    public float dragSpeed = 2f;

    private Camera cam;
    private Vector3 dragOrigin;
    private bool isDragging;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(0) || !Input.GetKey(KeyCode.LeftControl)) isDragging = false;

        if (isDragging)
        {
            var difference = cam.ScreenToWorldPoint(Input.mousePosition) -
                             cam.ScreenToWorldPoint(dragOrigin);
            difference.z = 0f;

            var newPosition = transform.position - difference;

            var camHeight = cam.orthographicSize;
            var camWidth = camHeight * cam.aspect;

            newPosition.x = Mathf.Clamp(newPosition.x, 0 + camWidth, MAX_X - camWidth);
            newPosition.y = Mathf.Clamp(newPosition.y, 0 + camHeight, MAX_Y - camHeight);
            transform.position = newPosition;

            dragOrigin = Input.mousePosition;
        }
    }
}