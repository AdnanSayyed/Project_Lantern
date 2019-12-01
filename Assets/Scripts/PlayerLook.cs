using UnityEngine;

public class PlayerLook : MonoBehaviour {

    public GameObject cam;
    public GameObject hallCam;

    private float mouseY;
    private float mouseSensitivity;
    private float camLookAngle;
    private float camRotation = 0f;

    private float camLookHandAngleX = 25f;
    private float camLookHandAngleY = 25f;

    //For hand cam in hallway
    private float camRotationX = 0f;
    private float camRotationY = 0f;

    public void SetSensitivity(float _sensitivity)
    {
        mouseSensitivity = _sensitivity;
    }

    public void SetLookAngle(float _angle)
    {
        camLookAngle = _angle;
    }

    public void SetRotation(float _mouseY)
    {
        mouseY = -_mouseY;
    }

    public void PerformHandRotation(float _mouseY, float _mouseX)
    {
        camRotationX += (_mouseX * mouseSensitivity);
        camRotationY += (_mouseY * mouseSensitivity);

        camRotationX = Mathf.Clamp(camRotationX, -camLookHandAngleX, camLookHandAngleX);
        camRotationY = Mathf.Clamp(camRotationY, -camLookHandAngleY, camLookHandAngleY);
        hallCam.transform.localRotation = Quaternion.Euler(-camRotationX, camRotationY, 0);
    }


    private void Update()
    {
        PerformRotation();
    }

    void PerformRotation()
    {
        camRotation += (mouseY * mouseSensitivity);
        camRotation = Mathf.Clamp(camRotation, -camLookAngle, camLookAngle);
        cam.transform.localRotation = Quaternion.Euler(camRotation, 0, 0);
    }

    public void ResetCamera()
    {
        cam.transform.localRotation = Quaternion.identity;
    }

}
