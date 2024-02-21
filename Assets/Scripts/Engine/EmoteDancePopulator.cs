using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EmoteDancePopulator : MonoBehaviour
{
    private GiuliaAIConfig giuliaAIConfig; 
    void Start()
    {
        giuliaAIConfig = UnityEngine.Object.FindFirstObjectByType<GiuliaAIConfig>();

        PopulateDropdown(giuliaAIConfig.GUIEmoteDropdown, giuliaAIConfig.EmoteTriggers);
        PopulateDropdown(giuliaAIConfig.GUIDanceDropdown, giuliaAIConfig.DanceTriggers);

        giuliaAIConfig.GUIEmotePlayButton.onClick.AddListener(() => TriggerAnimation(giuliaAIConfig.GUIEmoteDropdown, null));
        giuliaAIConfig.GUIDancePlayDanceButton.onClick.AddListener(() => TriggerAnimation(giuliaAIConfig.GUIDanceDropdown, giuliaAIConfig.DanceClips));
        giuliaAIConfig.GUIDanceStopButton.onClick.AddListener(StopDance);
    }

    void PopulateDropdown(TMP_Dropdown dropdown, List<string> triggers)
    {
        dropdown.ClearOptions();
        foreach (var trigger in triggers)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData() { text = trigger });
        }
        dropdown.RefreshShownValue();
    }

    public void TriggerAnimation(TMP_Dropdown dropdown, List<AudioClip> clips)
    {
        string selectedTrigger = dropdown.options[dropdown.value].text;
        giuliaAIConfig.AnimatorController.SetTrigger(selectedTrigger);
        if(giuliaAIConfig.EnableDebugMode)
        {
            Debug.Log("(TriggerAnimation) Triggered animation: " + selectedTrigger);
        }
        if (clips != null && clips.Count > dropdown.value)
        {
            giuliaAIConfig.DanceAudioSource.clip = clips[dropdown.value];
            giuliaAIConfig.DanceAudioSource.Play();
            if (giuliaAIConfig.EnableDebugMode)
            {
                Debug.Log("(TriggerAnimation) Triggered audio: " + clips[dropdown.value].name);
            }
        }
    }
    public void APITriggerAnimation(string animationName, List<AudioClip> clips)
    {
        if (!string.IsNullOrEmpty(animationName))
        {
            giuliaAIConfig.AnimatorController.SetTrigger(animationName);

            if (clips != null && clips.Count > 0)
            {
                int clipIndex = Math.Min(giuliaAIConfig.DanceTriggers.IndexOf(animationName), clips.Count - 1);

                giuliaAIConfig.DanceAudioSource.clip = clips[clipIndex];
                giuliaAIConfig.DanceAudioSource.Play();

                if (giuliaAIConfig.EnableDebugMode)
                {
                    Debug.Log("(APITriggerAnimation) Triggered animation: " + animationName + " with audio: " + clips[clipIndex].name);
                }
            }
            else if (giuliaAIConfig.EnableDebugMode)
            {
                Debug.Log("(APITriggerAnimation) Triggered animation: " + animationName + " without audio");
            }
        }
    }





    public void StopDance()
    {
        giuliaAIConfig.DanceAudioSource.Stop();
        giuliaAIConfig.DanceAudioSource.clip = null;

        giuliaAIConfig.AnimatorController.SetTrigger("dance_exit");
        if (giuliaAIConfig.EnableDebugMode)
        {
            Debug.Log("Stopped dance");
        }
    }

}
