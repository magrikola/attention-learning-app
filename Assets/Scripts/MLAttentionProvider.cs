using System;
using System.IO;
using UnityEngine;

public class MLAttentionProvider : MonoBehaviour, IAttentionProvider
{
    public static MLAttentionProvider Instance;

    [Header("Provider Settings")]
    public bool registerAsActiveProvider = true;

    [Header("Model Files")]
    public string tfliteModelFileName = "attention_model.tflite";
    public string scalerFileName = "scaler_values.json";

    [Header("Input Features")]
    public float yaw;
    public float pitch;
    public float gazeX;
    public float gazeY;

    [Header("Prediction Output")]
    public bool isFocused = true;
    [Range(0f, 1f)] public float confidence = 1f;

    [Header("Debug")]
    public bool useDebugValues = true;
    public bool printDebugLogs = true;

    private ScalerData scalerData;
    private bool lastFocusState;

    [Serializable]
    public class ScalerData
    {
        public string[] features;
        public float[] mean;
        public float[] scale;
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        lastFocusState = isFocused;

        LoadScalerValues();
        CheckTFLiteFile();
    }

    void Start()
    {
        RegisterProvider();
    }

    void Update()
    {
        if (useDebugValues)
        {
            PredictAttention(yaw, pitch, gazeX, gazeY);
        }

       
        if (registerAsActiveProvider &&
            AttentionSystemManager.Instance != null &&
            AttentionSystemManager.Instance.GetFocusState() != isFocused)
        {
            AttentionSystemManager.Instance.SetProvider(this);
        }
    }

    private void RegisterProvider()
    {
        if (!registerAsActiveProvider)
            return;

        if (AttentionSystemManager.Instance != null)
        {
            AttentionSystemManager.Instance.SetProvider(this);
            Debug.Log("[MLAttentionProvider] Registered as active attention provider.");
        }
        else
        {
            Debug.LogWarning("[MLAttentionProvider] AttentionSystemManager not found yet.");
        }
    }

    public bool GetFocusState()
    {
        return isFocused;
    }

    public bool PredictAttention(float inputYaw, float inputPitch, float inputGazeX, float inputGazeY)
    {
        float[] rawInput = new float[]
        {
            inputYaw,
            inputPitch,
            inputGazeX,
            inputGazeY
        };

        float[] scaledInput = NormalizeInput(rawInput);

       

        bool prediction =
            Mathf.Abs(inputYaw) <= 15f &&
            Mathf.Abs(inputGazeX) <= 5f &&
            Mathf.Abs(inputGazeY) <= 10f;

        isFocused = prediction;

        float deviation =
            Mathf.Abs(inputYaw) +
            Mathf.Abs(inputPitch) +
            Mathf.Abs(inputGazeX) +
            Mathf.Abs(inputGazeY);

        confidence = Mathf.Clamp01(1f - deviation / 80f);

       
        if (FakeAttentionManager.Instance != null)
        {
            FakeAttentionManager.Instance.SetFocus(isFocused);
        }

        if (lastFocusState != isFocused)
        {
            Debug.Log("[MLAttentionProvider] Focus changed to: " + GetFocusText());
            lastFocusState = isFocused;
        }

        if (printDebugLogs)
        {
            Debug.Log(
                "ML Attention Prediction: " +
                GetFocusText() +
                " | Confidence: " + confidence.ToString("F2") +
                " | Raw: [" + string.Join(", ", rawInput) + "]" +
                " | Scaled: [" + string.Join(", ", scaledInput) + "]"
            );
        }

        return isFocused;
    }

    private float[] NormalizeInput(float[] rawInput)
    {
        float[] scaled = new float[rawInput.Length];

        if (scalerData == null || scalerData.mean == null || scalerData.scale == null)
        {
            return rawInput;
        }

        for (int i = 0; i < rawInput.Length; i++)
        {
            if (i < scalerData.mean.Length && i < scalerData.scale.Length && scalerData.scale[i] != 0)
            {
                scaled[i] = (rawInput[i] - scalerData.mean[i]) / scalerData.scale[i];
            }
            else
            {
                scaled[i] = rawInput[i];
            }
        }

        return scaled;
    }

    private void LoadScalerValues()
    {
        string scalerPath = Path.Combine(Application.streamingAssetsPath, scalerFileName);

        if (!File.Exists(scalerPath))
        {
            Debug.LogWarning("[MLAttentionProvider] Scaler file not found: " + scalerPath);
            return;
        }

        string json = File.ReadAllText(scalerPath);
        scalerData = JsonUtility.FromJson<ScalerData>(json);

        Debug.Log("[MLAttentionProvider] Scaler values loaded from: " + scalerPath);
    }

    private void CheckTFLiteFile()
    {
        string modelPath = Path.Combine(Application.streamingAssetsPath, tfliteModelFileName);

        if (File.Exists(modelPath))
        {
            Debug.Log("[MLAttentionProvider] TFLite model found: " + modelPath);
        }
        else
        {
            Debug.LogWarning("[MLAttentionProvider] TFLite model not found: " + modelPath);
        }
    }

    public string GetFocusText()
    {
        return isFocused ? "Focused" : "Not Focused";
    }
    
    public void SetInputFeatures(float inputYaw, float inputPitch, float inputGazeX, float inputGazeY)
    {
        yaw = inputYaw;
        pitch = inputPitch;
        gazeX = inputGazeX;
        gazeY = inputGazeY;

        PredictAttention(yaw, pitch, gazeX, gazeY);
    }

    public bool GetFocus()
    {
        return isFocused;
    }
}