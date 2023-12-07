using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem; 

public class AnimationToggle : MonoBehaviour
{
    private Animator _animator;

    [Header("Settings")]
    public AudioSource selectedAudioSource;
    public AudioSource bgmAudioSource;
    public MonoBehaviour sleepingScript;
    private float originalBGMVolume;

    public string boolParameterName1 = "Animation1";
    public string boolParameterName2 = "Animation2";
    public string boolParameterName3 = "Animation3";
    public string boolParameterName4 = "Animation4";

    public AudioClip audioClip1;
    public AudioClip audioClip2;
    public AudioClip audioClip3;
    public AudioClip audioClip4;

    private bool isPlaying = false;

    private void Start()
    {
        //preload
        selectedAudioSource.clip = audioClip1;
        selectedAudioSource.Play();
        selectedAudioSource.Stop();
        selectedAudioSource.clip = audioClip2;
        selectedAudioSource.Play();
        selectedAudioSource.Stop();
        selectedAudioSource.clip = audioClip3;
        selectedAudioSource.Play();
        selectedAudioSource.Stop();
        selectedAudioSource.clip = audioClip4;
        selectedAudioSource.Play();
        selectedAudioSource.Stop();

        _animator = GetComponent<Animator>();
        if (selectedAudioSource == null)
        {
            selectedAudioSource = GetComponent<AudioSource>();
        }

        originalBGMVolume = bgmAudioSource.volume;
        if (!bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Play();
        }
    }

    private void ResetAllAnimations()
    {
        _animator.SetBool("sleeping", false);
        _animator.SetBool(boolParameterName1, false);
        _animator.SetBool(boolParameterName2, false);
        _animator.SetBool(boolParameterName3, false);
        _animator.SetBool(boolParameterName4, false);
        selectedAudioSource.Stop();

        // Unmute il bgmAudioSource con fade
        StartCoroutine(FadeVolume(bgmAudioSource, originalBGMVolume, 1f));

        isPlaying = false;
    }

    private void StartAnimation(Key key, string parameterName, AudioClip clip) // Modificato KeyCode in Key
    {
        if (Keyboard.current[key].wasPressedThisFrame) // Modificato Input.GetKeyDown
        {
            ResetAllAnimations();
            sleepingScript.enabled = false;
            _animator.SetBool(parameterName, true);

            // Mute il bgmAudioSource con fade
            StartCoroutine(FadeVolume(bgmAudioSource, 0f, 1f));

            if (clip != null && selectedAudioSource != null)
            {
                selectedAudioSource.clip = clip;
                selectedAudioSource.Play();
                isPlaying = true;
            }
        }
    }

    private void Update()
    {
        if (!isPlaying)
        {
            StartAnimation(Key.A, boolParameterName1, audioClip1); 
            StartAnimation(Key.S, boolParameterName2, audioClip2);
            StartAnimation(Key.D, boolParameterName3, audioClip3);
            StartAnimation(Key.F, boolParameterName4, audioClip4);
        }

        if (isPlaying && selectedAudioSource && !selectedAudioSource.isPlaying)
        {
            ResetAllAnimations();
            sleepingScript.enabled = true;
        }

        if (Keyboard.current[Key.Escape].wasPressedThisFrame)
        {
            StopAllAnimationsAndAudio();
            sleepingScript.enabled = !sleepingScript.enabled;
        }
    }

    private void StopAllAnimationsAndAudio()
    {
        selectedAudioSource.Stop();

        _animator.SetBool("sleeping", false);
        _animator.SetBool(boolParameterName1, false);
        _animator.SetBool(boolParameterName2, false);
        _animator.SetBool(boolParameterName3, false);
        _animator.SetBool(boolParameterName4, false);
    }

    IEnumerator FadeVolume(AudioSource audioSource, float targetVolume, float duration)
    {
        float startVolume = audioSource.volume;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, t / duration);
            yield return null;
        }

        audioSource.volume = targetVolume;
    }
}
