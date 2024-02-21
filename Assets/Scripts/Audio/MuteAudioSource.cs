using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MuteAudioSource : MonoBehaviour
{
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!audioSource.mute)
        {
            audioSource.mute = true;
        }
    }
}
