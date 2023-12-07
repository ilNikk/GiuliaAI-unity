using UnityEngine;
using System.Collections.Generic;

public class AudioController : MonoBehaviour
{
    public Animator animator;
    public AudioSource audioSource;  
    public List<string> triggerNames;  
    public List<AudioClip> audioClips;  
    private Dictionary<string, AudioClip> triggerToClipMap;  

    void Start()
    {
        triggerToClipMap = new Dictionary<string, AudioClip>();
        for (int i = 0; i < Mathf.Min(triggerNames.Count, audioClips.Count); i++)
        {
            triggerToClipMap[triggerNames[i]] = audioClips[i];
        }
        
        foreach (AudioClip clip in audioClips)
        {
            if (clip != null)
            {
                clip.LoadAudioData();
            }
        }
    }

    public void PlayAudioForTrigger(string triggerName)
    {
        if (triggerToClipMap.ContainsKey(triggerName))
        {
            audioSource.clip = triggerToClipMap[triggerName];
            audioSource.Play();
            //ResetFirstTrigger();
            //StartCoroutine(ResetFirstTriggerAfterDelay(1f)); 
        }
    }
    /*IEnumerator ResetFirstTriggerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);  // attendi per il ritardo specificato
        ResetFirstTrigger();
    }
    void ResetFirstTrigger()
    {
        if (animator != null && triggerNames.Count > 0)
        {
            animator.ResetTrigger(triggerNames[0]);
        }
    }*/
}


