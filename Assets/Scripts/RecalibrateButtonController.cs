using UnityEngine;
using UnityEngine.UI;

public class RecalibrateButtonController : MonoBehaviour
{
    public Button button;
    public ARFaceHeadPoseInput faceHeadPoseInput;

    void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (faceHeadPoseInput == null)
            faceHeadPoseInput = FindObjectOfType<ARFaceHeadPoseInput>();
    }

    void Start()
    {
        if (button == null)
        {
            Debug.LogError("RecalibrateButtonController: Button component bulunamadı!");
            return;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnRecalibrateClicked);

        Debug.Log("Recalibrate button listener added.");
    }

    void OnRecalibrateClicked()
    {
        Debug.Log("RECALIBRATE BUTTON CLICKED");

        if (faceHeadPoseInput == null)
        {
            Debug.LogError("ARFaceHeadPoseInput bulunamadı!");
            return;
        }

        faceHeadPoseInput.StartCalibration();
    }
}