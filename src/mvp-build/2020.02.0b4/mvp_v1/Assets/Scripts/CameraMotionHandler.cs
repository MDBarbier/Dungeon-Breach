using UnityEngine;

public class CameraMotionHandler : MonoBehaviour
{
    public float cameraMoveSpeed;
    public float cameraMousePanSpeed;
    public float panDetect;
    public float rotationSpeed;
    public float rotationAmount;
    public float minCameraHeight;
    public float maxCameraHeight;

    private Quaternion initialRotation;
    private float initialHeight;

    // Start is called before the first frame update
    public void Start()
    {
        initialRotation = Camera.main.transform.rotation;
        initialHeight = Camera.main.transform.position.y;
    }

    // Update is called once per frame
    public void Update()
    {
        MoveCamera();
        RotateCamera();
        ResetCamera();
    }

    /// <summary>
    /// Handles panning movement both by the keyboard and the mouse being near the edge of the screen
    /// </summary>
    private void MoveCamera()
    {
        ScreenEdgePan();
        LateralKeyboardMovement();
        VerticalKeyboardMovement();
        MouseScrollWheelMovement();
        KeyboardHorizontalRotation();
        KeyboardVerticalRotation();
        KeyboardMovementInDirectionOfCamera();
    }

    private void KeyboardMovementInDirectionOfCamera()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Camera.main.transform.Translate(Vector3.forward * cameraMoveSpeed);
        }

        if (Input.GetKey(KeyCode.A))
        {
            Camera.main.transform.Translate(Vector3.left * cameraMoveSpeed);
        }

        if (Input.GetKey(KeyCode.S))
        {
            Camera.main.transform.Translate(Vector3.back * cameraMoveSpeed);
        }

        if (Input.GetKey(KeyCode.D))
        {
            Camera.main.transform.Translate(Vector3.right * cameraMoveSpeed);
        }
    }

    private void RotateCamera()
    {
        Vector3 origin = Camera.main.transform.eulerAngles;
        Vector3 destination = origin;

        if (Input.GetMouseButton(2))
        {
            destination.x -= Input.GetAxis("Mouse Y") * (rotationAmount);
            destination.y += Input.GetAxis("Mouse X") * (rotationAmount);
        }

        if (destination != origin)
        {
            Camera.main.transform.eulerAngles = Vector3.MoveTowards(origin, destination, Time.deltaTime * rotationSpeed);
        }
    }

    private void KeyboardVerticalRotation()
    {
        Vector3 origin = Camera.main.transform.eulerAngles;
        Vector3 destination = origin;

        if (Input.GetKey(KeyCode.Insert))
        {
            destination.x -= rotationAmount;
        }

        if (Input.GetKey(KeyCode.Delete))
        {
            destination.x += rotationAmount;
        }

        if (destination != origin)
        {
            Camera.main.transform.eulerAngles = Vector3.MoveTowards(origin, destination, Time.deltaTime * rotationSpeed);
        }
    }

    private void KeyboardHorizontalRotation()
    {
        Vector3 origin = Camera.main.transform.eulerAngles;
        Vector3 destination = origin;

        if (Input.GetKey(KeyCode.Home))
        {
            destination.y -= rotationAmount;
        }

        if (Input.GetKey(KeyCode.End))
        {
            destination.y += rotationAmount;
        }

        if (destination != origin)
        {
            Camera.main.transform.eulerAngles = Vector3.MoveTowards(origin, destination, Time.deltaTime * rotationSpeed);
        }
    }

    private void MouseScrollWheelMovement()
    {
        float moveY = Camera.main.transform.position.y;
        float scrollwheel = Input.GetAxis("Mouse ScrollWheel");
        moveY -= scrollwheel * (cameraMousePanSpeed * 20);
        moveY = Mathf.Clamp(moveY, minCameraHeight, maxCameraHeight);
        Vector3 newPos = new Vector3(Camera.main.transform.position.x, moveY, Camera.main.transform.position.z);
        Camera.main.transform.position = newPos;
    }

    private void VerticalKeyboardMovement()
    {
        if ((Input.GetKey(KeyCode.PageUp)) && transform.position.y < maxCameraHeight)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + cameraMoveSpeed, transform.position.z);
        }

        if (Input.GetKey(KeyCode.PageDown) && transform.position.y > minCameraHeight)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - cameraMoveSpeed, transform.position.z);
        }
    }

    private void LateralKeyboardMovement()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position = new Vector3(transform.position.x + cameraMoveSpeed, transform.position.y, transform.position.z);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position = new Vector3(transform.position.x - cameraMoveSpeed, transform.position.y, transform.position.z);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - cameraMoveSpeed);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + cameraMoveSpeed);
        }
    }

    private void ScreenEdgePan()
    {
        float moveX = Camera.main.transform.position.x;
        float moveZ = Camera.main.transform.position.z;
        float mouseX = Input.mousePosition.x;
        float mouseY = Input.mousePosition.y;
        float cameraHeight = transform.position.y;

        if (mouseX > 0 && mouseX < panDetect) //mouse pan left
        {
            moveX -= cameraMoveSpeed;
        }
        else if (mouseX < Screen.width && mouseX > Screen.width - panDetect) //mouse pan right
        {
            moveX += cameraMoveSpeed;
        }

        if (mouseY < Screen.height && mouseY > Screen.height - panDetect) //mouse pan up
        {
            moveZ += cameraMoveSpeed;
        }
        else if (mouseY > 0 && mouseY < panDetect) //mouse pan down
        {
            moveZ -= cameraMoveSpeed;
        }

        Vector3 newCameraPosition = new Vector3(moveX, cameraHeight, moveZ);

        Camera.main.transform.position = newCameraPosition;
    }

    private void ResetCamera()
    {
        if (Input.GetKeyDown(KeyCode.Home))
        {
            Camera.main.transform.rotation = initialRotation;
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, initialHeight, Camera.main.transform.position.z);
        }
    }
}
