using UnityEngine;
using UnityEngine.SceneManagement;

public class ARHeadPoseToAttentionInput : MonoBehaviour
{
    [Header("References")]
    public MLAttentionProvider mlAttentionProvider;

    [Tooltip("Camera transform used as head/device pose source.")]
    public Transform headTransform;

    [Header("Calibration")]
    public bool autoCalibrateOnStart = true;
    public bool recalibrateOnSceneChange = true;
    public KeyCode recalibrateKey = KeyCode.C;

    private Quaternion calibrationRotation;
    private bool calibrated = false;

    [Header("Live Values")]
    public float yaw;
    public float pitch;
    public float gazeX;
    public float gazeY;

    [Header("Settings")]
    public bool printDebugLogs = false;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        FindReferences();

        if (autoCalibrateOnStart)
            Calibrate();
    }

    void Update()
    {
        if (Input.GetKeyDown(recalibrateKey))
            Calibrate();

        if (mlAttentionProvider == null || headTransform == null)
            FindReferences();

        if (mlAttentionProvider == null || headTransform == null)
            return;

        if (!calibrated)
            Calibrate();

        Quaternion relativeRotation = Quaternion.Inverse(calibrationRotation) * headTransform.rotation;
        Vector3 euler = NormalizeEuler(relativeRotation.eulerAngles);

        yaw = euler.y;
        pitch = euler.x;

        gazeX = yaw;
        gazeY = pitch;

        mlAttentionProvider.SetInputFeatures(yaw, pitch, gazeX, gazeY);

        if (printDebugLogs)
        {
            Debug.Log(
                "[ARHeadPoseInput] Yaw: " + yaw.ToString("F1") +
                " | Pitch: " + pitch.ToString("F1") +
                " | State: " + mlAttentionProvider.GetFocusText()
            );
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindReferences();

        if (recalibrateOnSceneChange)
        {
            calibrated = false;
            Calibrate();
        }
    }

    private void FindReferences()
    {
        if (mlAttentionProvider == null)
            mlAttentionProvider = MLAttentionProvider.Instance;

        if (headTransform == null && Camera.main != null)
            headTransform = Camera.main.transform;
    }

    public void Calibrate()
    {
        FindReferences();

        if (headTransform == null)
        {
            Debug.LogWarning("[ARHeadPoseInput] Head Transform is missing. No camera found.");
            return;
        }

        calibrationRotation = headTransform.rotation;
        calibrated = true;

        Debug.Log("[ARHeadPoseInput] Calibrated current direction as Focused center.");
    }

    private Vector3 NormalizeEuler(Vector3 euler)
    {
        euler.x = NormalizeAngle(euler.x);
        euler.y = NormalizeAngle(euler.y);
        euler.z = NormalizeAngle(euler.z);
        return euler;
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;

        return angle;
    }
}