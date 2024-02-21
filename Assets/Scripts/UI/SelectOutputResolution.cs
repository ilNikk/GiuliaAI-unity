using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class OutputResulution : MonoBehaviour
{
    private GiuliaAIConfig giuliaAIConfig;
    void Start()
    {
        giuliaAIConfig = Object.FindFirstObjectByType<GiuliaAIConfig>();

        giuliaAIConfig.GUIOutputResolutionDropdown.onValueChanged.AddListener(ChangeCameraOutputTexture);
        giuliaAIConfig.GUIOutputResolutionDropdown.value = giuliaAIConfig.MainCameraOutputResolution;
        ChangeCameraOutputTexture(giuliaAIConfig.GUIOutputResolutionDropdown.value);
    }

    private void ChangeCameraOutputTexture(int value)
    {
        switch (value)
        {
            case 0:
                giuliaAIConfig.MainCamera.targetTexture = giuliaAIConfig.MainCameraRenderTexture4k;
                giuliaAIConfig.MainCameraCanvasImage.texture = giuliaAIConfig.MainCameraRenderTexture4k;
                break;
            case 1:
                giuliaAIConfig.MainCamera.targetTexture = giuliaAIConfig.MainCameraRenderTexture1080p;
                giuliaAIConfig.MainCameraCanvasImage.texture = giuliaAIConfig.MainCameraRenderTexture1080p;

                break;
            default:
                Debug.LogError("Selezione risoluzione non supportata.");
                break;
        }
    }
}
