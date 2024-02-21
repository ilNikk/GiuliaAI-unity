using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GiuliaAIConfig : MonoBehaviour
{
    [Header("Animator Controller")]
    public Animator AnimatorController;

    [Space(10)]
    [Header("Audio Source")]
    public AudioSource DanceAudioSource;

    [Space(10)]
    [Header("Main Camera")]
    public Camera MainCamera;
    public RenderTexture MainCameraRenderTexture4k;
    public RenderTexture MainCameraRenderTexture1080p;
    public RawImage MainCameraCanvasImage;
    [Tooltip("0: 4K, 1: 1080")]
    public int MainCameraOutputResolution = 1; // 0: 4K, 1: 1080
    public Vector3 MainCameraDefaultPosition = new Vector3(0f, 0.7f, 0f);
    public Vector3 MainCameraDefaultRotation = Vector3.zero;
    public float MainCameraDefaultFOV = 60f;
    public float MainCameraXZSpeed = 5.0f;
    public float MainCameraXYSpeed = 2.0f;
    public float MainCameraRotationSpeed = 100.0f;

    [Space(10)]
    [Header("GiuliaAI Prefeab configuration")]
    public GameObject mainPrefab;
    public GameObject bodyPrefab;
    public GameObject facePrefab;
    public GameObject rootPrefab;
    public Transform headTransform;
  
    [Space(10)]
    [Header("API Server")]
    public string APIServerDomain = "localhost";
    public int APIServerPort = 4988;

    [Space(10)]
    [Header("Emote Populator")]
    public List<string> EmoteTriggers;
    
    [Space(10)]
    [Header("Dance Populator")]
    public List<string> DanceTriggers;
    public List<AudioClip> DanceClips;

    [Space(10)]
    [Header("OSU Controller")]
    public GameObject OSUMouseTracker;
    public string OSUControllerFilePath = @"C:\Users\nicol\Desktop\git\GiuliaAI\game\osu\GiuliaAI_OsuControllerBool.txt";
    public float OSUControllerCheckInterval = 2f;
    public float OSUControllerMaxHorizontalAngle = 10f;
    public float OSUControllerMaxVerticalAngle = 25f;
    public float OSUControllerSmoothFactor = 5f;
    public bool OSUControllerLogDebug = false;

    [Space(10)]
    [Header("GUI - Emote and Dance")]
    public TMP_Dropdown GUIEmoteDropdown;
    public Button GUIEmotePlayButton;
    public Button GUIDanceStopButton;
    public TMP_Dropdown GUIDanceDropdown;
    public Button GUIDancePlayDanceButton;

    [Space(2)]
    [Header("GUI - Camera")]
    public Toggle GUICameraToggle;
    public Slider GUICameraFOVSlider;
    public Button GUICameraResetButton;

    [Space(2)]
    [Header("GUI - NDI Config")]
    public GameObject GUINDISender;
    public Toggle GUINDIOutputToggle;
    public TMP_InputField GUINDINameInputField;
    public Button GUINDISaveButton;
    
    [Space(2)]
    [Header("GUI - Audio output selector")]
    public TMP_Dropdown GUIAudioOutputDropdown;

    [Space(2)]
    [Header("GUI - FPS Counter")]
    public TextMeshProUGUI GUIFPSCounter;
    
    [Space(2)]
    [Header("GUI - Safe Zone")]
    public Toggle GUISafeZoneToggle;
    public TMP_Dropdown GUISafeZoneDropdown;
    public Toggle GUISafeZoneGuideLineToggle;
    public GameObject[] GUISafeZoneObjects;
    public GameObject GUISafeZoneGuideLineObject;

    [Space(2)]
    [Header("GUI - Output Resolution")]
    public TMP_Dropdown GUIOutputResolutionDropdown;
    
    [Space(2)]
    [Header("GUI - Screenshots")]
    public TMP_Dropdown GUIScreenshotResolutionDropdown;
    public Button GUIScreenshotButton;
    public Button GUIScreenshotFolderButton;

    [Space(10)]
    [Header("Debug")]
    public bool EnableDebugMode = false;
}