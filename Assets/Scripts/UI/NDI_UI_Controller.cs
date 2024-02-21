using UnityEngine;
using UnityEngine.UI; 
using TMPro;
using Klak.Ndi;

public class NdiMenuController : MonoBehaviour
{
    private GiuliaAIConfig giuliaAIConfig;
    private NdiSender ndiSender;

    private void Start()
    {
        giuliaAIConfig = Object.FindFirstObjectByType<GiuliaAIConfig>();

        ndiSender = giuliaAIConfig.GUINDISender.GetComponent<NdiSender>();

        if (ndiSender != null)
        {
            giuliaAIConfig.GUINDINameInputField.text = ndiSender.ndiName;
        }

        giuliaAIConfig.GUINDIOutputToggle.onValueChanged.AddListener(delegate { ToggleNdiOutput(giuliaAIConfig.GUINDIOutputToggle.isOn); });
        giuliaAIConfig.GUINDISaveButton.onClick.AddListener(SaveNdiName);
        
        ToggleNdiOutput(giuliaAIConfig.GUINDIOutputToggle.isOn);
    }

    private void ToggleNdiOutput(bool isOn)
    {
        giuliaAIConfig.GUINDISender.SetActive(isOn);
    }

    public void SaveNdiName()
    {
        if (ndiSender != null && !string.IsNullOrEmpty(giuliaAIConfig.GUINDINameInputField.text))
        {
            ndiSender.ndiName = giuliaAIConfig.GUINDINameInputField.text;
        }
        if (ndiSender.isActiveAndEnabled)
        {
        ndiSender.enabled = false;
        ndiSender.enabled = true;
        }
    }
}
