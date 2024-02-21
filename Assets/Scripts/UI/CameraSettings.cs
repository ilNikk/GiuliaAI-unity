using UnityEngine;
using UnityEngine.UI; 
public class CameraController : MonoBehaviour
{
    private GiuliaAIConfig giuliaAIConfig; 
    private bool isMoveCameraActive = false;
    void Start()
    { 
        giuliaAIConfig = Object.FindFirstObjectByType<GiuliaAIConfig>();
        if (giuliaAIConfig == null)
        {
            Debug.LogError("Impossibile trovare un'istanza di GiuliaAIConfig nell'ambiente!");
        }
        giuliaAIConfig.GUICameraToggle.onValueChanged.AddListener(ToggleFOVSlider);
        giuliaAIConfig.GUICameraToggle.onValueChanged.AddListener(ToggleCameraMovement);
        giuliaAIConfig.GUICameraResetButton.onClick.AddListener(ResetCamera);
    }
    void Update()
    {
        if (giuliaAIConfig.GUICameraToggle.isOn != isMoveCameraActive)
        {
            isMoveCameraActive = giuliaAIConfig.GUICameraToggle.isOn;
        }

        if (isMoveCameraActive)
        {
            float x = Input.GetAxis("Horizontal") * giuliaAIConfig.MainCameraXZSpeed * Time.deltaTime;
            float z = Input.GetAxis("Vertical") * giuliaAIConfig.MainCameraXZSpeed * Time.deltaTime;
            float y = 0;

            if (Input.GetKey(KeyCode.Space))
            {
                y = giuliaAIConfig.MainCameraXYSpeed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                y = -giuliaAIConfig.MainCameraXYSpeed * Time.deltaTime;
            }

            giuliaAIConfig.MainCamera.transform.Translate(x, y, z);

            if (Input.GetMouseButton(1))
            {
                float h = giuliaAIConfig.MainCameraRotationSpeed * Input.GetAxis("Mouse X") * Time.deltaTime;
                float v = giuliaAIConfig.MainCameraRotationSpeed * Input.GetAxis("Mouse Y") * Time.deltaTime;
                giuliaAIConfig.MainCamera.transform.Rotate(-v, h, 0);
                giuliaAIConfig.MainCamera.transform.eulerAngles = new Vector3(giuliaAIConfig.MainCamera.transform.eulerAngles.x, giuliaAIConfig.MainCamera.transform.eulerAngles.y, 0);
            }
        }

        giuliaAIConfig.MainCamera.fieldOfView = giuliaAIConfig.GUICameraFOVSlider.value;

    }
    public void ResetCamera()
    {
        giuliaAIConfig.MainCamera.transform.position = giuliaAIConfig.MainCameraDefaultPosition;
        giuliaAIConfig.MainCamera.transform.eulerAngles = giuliaAIConfig.MainCameraDefaultRotation;
        giuliaAIConfig.MainCamera.fieldOfView = giuliaAIConfig.MainCameraDefaultFOV;
        giuliaAIConfig.GUICameraToggle.isOn = false;
        giuliaAIConfig.GUICameraFOVSlider.value = giuliaAIConfig.MainCameraDefaultFOV;
    }
    public void ToggleFOVSlider(bool toggle)
    {
        giuliaAIConfig.GUICameraFOVSlider.interactable = toggle;
    }
    void ToggleCameraMovement(bool toggle)
    {
        isMoveCameraActive = toggle;
    }
}