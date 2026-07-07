using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ForceFocusCamera : MonoBehaviour
{
    public ARCameraManager cameraManager;

    void Awake()
    {
        if (cameraManager == null)
            cameraManager = FindObjectOfType<ARCameraManager>();

        if (cameraManager != null)
        {
            cameraManager.requestedFacingDirection = CameraFacingDirection.User;
            Debug.Log("Requested Camera: USER / FRONT");
        }
        else
        {
            Debug.LogError("ARCameraManager bulunamadı!");
        }
    }

    void Update()
    {
        if (cameraManager != null)
        {
            Debug.Log("Current requested camera: " + cameraManager.requestedFacingDirection);
        }
    }
}