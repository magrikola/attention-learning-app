using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class ARFaceHeadPoseInput : MonoBehaviour
{
    [Header("AR Face Tracking")]
    public ARFaceManager faceManager;

    [Header("Focus Thresholds")]
    public float maxYawDifference = 18f;
    public float maxPitchDifference = 15f;
    public float maxRollDifference = 25f;

    [Header("Calibration Settings")]
    public bool autoCalibrateOnStart = true;
    public float calibrationDuration = 2f;

    [Header("UI")]
    public GameObject calibrationPanel;
    public TMP_Text calibrationText;
    public TMP_Text debugText;

    [Header("Debug Values")]
    public float currentYaw;
    public float currentPitch;
    public float currentRoll;

    public float baselineYaw;
    public float baselinePitch;
    public float baselineRoll;

    public float yawDifference;
    public float pitchDifference;
    public float rollDifference;

    public bool isCalibrated = false;
    public bool isFocused = false;

    private ARFace currentFace;
    private float calibrationTimer = 0f;
    private bool isCalibrating = false;

    public bool IsCalibrating
    {
        get { return isCalibrating; }
    }

    public bool IsCalibrationBlockingGameplay
    {
        get { return isCalibrating || !isCalibrated; }
    }

    void Awake()
    {
        if (faceManager == null)
            faceManager = FindObjectOfType<ARFaceManager>();
    }

    void Start()
    {
        if (autoCalibrateOnStart)
            StartCalibration();
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

    void Update()
    {
        if (currentFace == null)
        {
            isFocused = false;
            SendFocusToAttentionSystem(false);
            UpdateDebugText(isCalibrating ? "Calibrating - no face detected" : "No face detected");
            return;
        }

        UpdateCurrentFaceAngles();

        if (isCalibrating)
        {
            RunCalibrationTimer();
            return;
        }

        if (!isCalibrated)
        {
            isFocused = false;
            SendFocusToAttentionSystem(false);
            UpdateDebugText("Not calibrated");
            return;
        }

        CalculateFocus();
        SendFocusToAttentionSystem(isFocused);
        UpdateDebugText(isFocused ? "Focused" : "Not Focused");
    }

    void OnFacesChanged(ARFacesChangedEventArgs args)
    {
        if (args.added.Count > 0)
        {
            currentFace = args.added[0];
            Debug.Log("Face detected.");
        }

        if (args.updated.Count > 0 && currentFace == null)
        {
            currentFace = args.updated[0];
        }

        if (args.removed.Count > 0)
        {
            currentFace = null;
            isFocused = false;
            SendFocusToAttentionSystem(false);
            Debug.Log("Face lost.");
        }
    }

    void UpdateCurrentFaceAngles()
    {
        Vector3 euler = currentFace.transform.rotation.eulerAngles;

        currentPitch = NormalizeAngle(euler.x);
        currentYaw = NormalizeAngle(euler.y);
        currentRoll = NormalizeAngle(euler.z);
    }

    public void StartCalibration()
    {
        calibrationTimer = 0f;
        isCalibrated = false;
        isCalibrating = true;

        isFocused = false;
        SendFocusToAttentionSystem(false);

        if (calibrationPanel != null)
            calibrationPanel.SetActive(true);

        if (calibrationText != null)
        {
            calibrationText.gameObject.SetActive(true);
            calibrationText.text = "Please look at the screen";
        }

        Debug.Log("Calibration started.");
    }

    void RunCalibrationTimer()
    {
        calibrationTimer += Time.deltaTime;

        float remainingTime = Mathf.Ceil(calibrationDuration - calibrationTimer);
        if (remainingTime < 0f)
            remainingTime = 0f;

        if (calibrationText != null)
            calibrationText.text = "Please look at the screen\n" + remainingTime.ToString("0");

        if (calibrationTimer >= calibrationDuration)
        {
            CalibrateCurrentPose();
        }
    }

    public void CalibrateCurrentPose()
    {
        if (currentFace == null)
        {
            Debug.LogWarning("Calibration failed. No face detected.");

            calibrationTimer = 0f;
            isCalibrating = true;
            isCalibrated = false;

            if (calibrationPanel != null)
                calibrationPanel.SetActive(true);

            if (calibrationText != null)
            {
                calibrationText.gameObject.SetActive(true);
                calibrationText.text = "No face detected.\nPlease look at the screen";
            }

            return;
        }

        UpdateCurrentFaceAngles();

        baselineYaw = currentYaw;
        baselinePitch = currentPitch;
        baselineRoll = currentRoll;

        yawDifference = 0f;
        pitchDifference = 0f;
        rollDifference = 0f;

        isCalibrated = true;
        isCalibrating = false;

        if (calibrationPanel != null)
            calibrationPanel.SetActive(false);

        if (calibrationText != null)
        {
            calibrationText.text = "";
            calibrationText.gameObject.SetActive(false);
        }

        Debug.Log("Calibration completed.");
        Debug.Log("Baseline Yaw: " + baselineYaw);
        Debug.Log("Baseline Pitch: " + baselinePitch);
        Debug.Log("Baseline Roll: " + baselineRoll);
    }

    void CalculateFocus()
    {
        yawDifference = Mathf.Abs(Mathf.DeltaAngle(baselineYaw, currentYaw));
        pitchDifference = Mathf.Abs(Mathf.DeltaAngle(baselinePitch, currentPitch));
        rollDifference = Mathf.Abs(Mathf.DeltaAngle(baselineRoll, currentRoll));

        isFocused =
            yawDifference <= maxYawDifference &&
            pitchDifference <= maxPitchDifference;
    }

    float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;

        return angle;
    }

    void SendFocusToAttentionSystem(bool focused)
    {
        if (AttentionSystemManager.Instance != null)
        {
            AttentionSystemManager.Instance.SetFocusState(focused);
        }
        else if (FakeAttentionManager.Instance != null)
        {
            FakeAttentionManager.Instance.SetFocus(focused);
        }
    }

    void UpdateDebugText(string state)
    {
        if (debugText == null)
            return;

        debugText.text =
            "State: " + state +
            "\nCalibrating: " + isCalibrating +
            "\nCalibrated: " + isCalibrated +
            "\nYaw Diff: " + yawDifference.ToString("0.0") + " / " + maxYawDifference +
            "\nPitch Diff: " + pitchDifference.ToString("0.0") + " / " + maxPitchDifference +
            "\nRoll Diff: " + rollDifference.ToString("0.0") + " / " + maxRollDifference;
    }
}