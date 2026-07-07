using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;

public class FaceDebugChecker : MonoBehaviour
{
    public ARFaceManager faceManager;

    void Awake()
    {
        if (faceManager == null)
            faceManager = FindObjectOfType<ARFaceManager>();
    }

    void OnEnable()
    {
        if (faceManager != null)
            faceManager.facesChanged += OnFacesChanged;
    }

    void OnDisable()
    {
        if (faceManager != null)
            faceManager.facesChanged -= OnFacesChanged;
    }

    void OnFacesChanged(ARFacesChangedEventArgs args)
    {
        Debug.Log("Faces Added: " + args.added.Count);
        Debug.Log("Faces Updated: " + args.updated.Count);
        Debug.Log("Faces Removed: " + args.removed.Count);

        foreach (var face in args.updated)
        {
            Vector3 euler = face.transform.rotation.eulerAngles;

            float pitch = NormalizeAngle(euler.x);
            float yaw = NormalizeAngle(euler.y);
            float roll = NormalizeAngle(euler.z);

            Debug.Log("FACE TRACKED | Pitch: " + pitch + " | Yaw: " + yaw + " | Roll: " + roll);
        }
    }

    float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;

        return angle;
    }
}