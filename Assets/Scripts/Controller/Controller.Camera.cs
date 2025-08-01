using UnityEngine;


public partial class Controller : MonoBehaviour
{
    private void HandleCameraRotation()
    {
        if (Input.GetMouseButtonDown(1))
        {
            bIsRightMouseHeld = true;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            bIsRightMouseHeld = false;
            Cursor.lockState = CursorLockMode.None;
        }

        if (bIsRightMouseHeld)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            yaw += mouseX;
            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, -89f, 89f);

            cameraTransform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }
    }

    private void HandleCameraMovement()
    {
        if (bIsRightMouseHeld)
        {
            float speed = cameraSpeed * (Input.GetKey(KeyCode.LeftShift) ? fastSpeedMultiplier : 1f);

            Vector3 direction = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) direction += cameraTransform.forward;
            if (Input.GetKey(KeyCode.S)) direction -= cameraTransform.forward;
            if (Input.GetKey(KeyCode.A)) direction -= cameraTransform.right;
            if (Input.GetKey(KeyCode.D)) direction += cameraTransform.right;
            if (Input.GetKey(KeyCode.E)) direction += cameraTransform.up;
            if (Input.GetKey(KeyCode.Q)) direction -= cameraTransform.up;

            cameraTransform.position += direction.normalized * (speed * Time.deltaTime);
        }
    }
}
