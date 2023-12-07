using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioDetection : MonoBehaviour
{
    private AudioSource audioSource;
    private Animator animator;
    private float timer = 0.0f;
    public MonoBehaviour queueProcessor;
    public float silenceThreshold = 0.01f;
    public float silenceDuration = 60.0f;
    private bool previousSleepingState = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        previousSleepingState = animator.GetBool("sleeping");
    }

    private void Update()
    {
        if (audioSource.isPlaying)
        {
            float volume = RMSValue();
            if (volume > silenceThreshold)
            {
                timer = 0.0f;
                animator.SetBool("sleeping", false);
            }
            else
            {
                timer += Time.deltaTime;
                if (timer >= silenceDuration)
                {
                    animator.SetBool("sleeping", true);
                }
            }

            bool currentSleepingState = animator.GetBool("sleeping");
            if (currentSleepingState != previousSleepingState)
            {
                queueProcessor.enabled = !currentSleepingState;
                previousSleepingState = currentSleepingState;
            }
        }
    }

    float RMSValue()
    {
        float[] data = new float[1024];
        audioSource.GetOutputData(data, 0);
        float squaredSum = 0.0f;
        for (int i = 0; i < data.Length; i++)
        {
            squaredSum += Mathf.Pow(data[i], 2);
        }
        return Mathf.Sqrt(squaredSum / data.Length);
    }
}
