using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SafeZoneMenuController : MonoBehaviour
{
    private GiuliaAIConfig giuliaAIConfig;
    private void Start()
    {
        giuliaAIConfig = Object.FindFirstObjectByType<GiuliaAIConfig>();

        giuliaAIConfig.GUISafeZoneToggle.onValueChanged.AddListener(delegate { ToggleSafeZoneDropdown(giuliaAIConfig.GUISafeZoneToggle.isOn); });
        giuliaAIConfig.GUISafeZoneGuideLineToggle.onValueChanged.AddListener(delegate { ToggleGuideLine(giuliaAIConfig.GUISafeZoneGuideLineToggle.isOn); });
        giuliaAIConfig.GUISafeZoneDropdown.onValueChanged.AddListener(delegate { SelectSafeZoneOption(giuliaAIConfig.GUISafeZoneDropdown.value); });

        ToggleSafeZoneDropdown(giuliaAIConfig.GUISafeZoneToggle.isOn);
        ToggleGuideLine(giuliaAIConfig.GUISafeZoneGuideLineToggle.isOn);
        SelectSafeZoneOption(giuliaAIConfig.GUISafeZoneDropdown.value);
    }

    public void ToggleSafeZoneDropdown(bool isOn)
    {
        giuliaAIConfig.GUISafeZoneDropdown.interactable = isOn;
        
        if (!isOn)
        {
            foreach (var obj in giuliaAIConfig.GUISafeZoneObjects)
            {
                obj.SetActive(false);
            }
        }
        else
        {
            SelectSafeZoneOption(giuliaAIConfig.GUISafeZoneDropdown.value);
        }
    }

    public void SelectSafeZoneOption(int optionIndex)
    {
        foreach (var obj in giuliaAIConfig.GUISafeZoneObjects)
        {
            obj.SetActive(false);
        }

        if (giuliaAIConfig.GUISafeZoneToggle.isOn && optionIndex >= 0 && optionIndex < giuliaAIConfig.GUISafeZoneObjects.Length)
        {
            giuliaAIConfig.GUISafeZoneObjects[optionIndex].SetActive(true);
        }
    }

    public void ToggleGuideLine(bool isOn)
    {
        giuliaAIConfig.GUISafeZoneGuideLineObject.SetActive(isOn);
    }
}
