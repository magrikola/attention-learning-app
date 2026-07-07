using UnityEngine;
using TMPro;

public class AttentionDebugUI : MonoBehaviour
{
    public ARFaceHeadPoseInput faceHeadPoseInput;
    public TMP_Text debugText;

    void Start()
    {
        if (faceHeadPoseInput == null)
            faceHeadPoseInput = FindObjectOfType<ARFaceHeadPoseInput>();
    }

    void Update()
    {
        if (debugText == null)
            return;

        if (faceHeadPoseInput == null)
            faceHeadPoseInput = FindObjectOfType<ARFaceHeadPoseInput>();

        if (faceHeadPoseInput == null)
        {
            debugText.text = "Waiting for AR Face Input...";
            return;
        }

        string attentionText = "Unknown";

        if (AttentionSystemManager.Instance != null)
            attentionText = AttentionSystemManager.Instance.GetFocusText();
        else if (FakeAttentionManager.Instance != null)
            attentionText = FakeAttentionManager.Instance.GetFocusText();

        debugText.text =
            "Attention: " + attentionText +
            "\nCalibrated: " + faceHeadPoseInput.isCalibrated +
            "\nRaw Focus: " + faceHeadPoseInput.isFocused +
            "\nCurrent Yaw: " + faceHeadPoseInput.currentYaw.ToString("F1") +
            "\nCurrent Pitch: " + faceHeadPoseInput.currentPitch.ToString("F1") +
            "\nCurrent Roll: " + faceHeadPoseInput.currentRoll.ToString("F1") +
            "\nYaw Diff: " + faceHeadPoseInput.yawDifference.ToString("F1") +
            "\nPitch Diff: " + faceHeadPoseInput.pitchDifference.ToString("F1") +
            "\nRoll Diff: " + faceHeadPoseInput.rollDifference.ToString("F1");
    }
}