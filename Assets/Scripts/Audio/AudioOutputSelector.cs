using System.Collections.Generic;
using TMPro;
using UnityEngine;
using NAudio.Wave;
 
public class AudioOutputSelector : MonoBehaviour
{
    private GiuliaAIConfig giuliaAIConfig;

    void Start()
    {
        giuliaAIConfig = Object.FindFirstObjectByType<GiuliaAIConfig>();


        PopulateAudioDevices();
    }

    void PopulateAudioDevices()
    {
        giuliaAIConfig.GUIAudioOutputDropdown.ClearOptions();
        List<string> deviceNames = new List<string>();
        for (int n = 0; n < WaveOut.DeviceCount; n++)
        {
            WaveOutCapabilities capabilities = WaveOut.GetCapabilities(n);
            deviceNames.Add(capabilities.ProductName);
        }
        giuliaAIConfig.GUIAudioOutputDropdown.AddOptions(deviceNames);
    }
    public void OnAudioDeviceSelected(int index)
    {
        // working on this
    }
}
