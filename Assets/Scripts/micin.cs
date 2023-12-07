using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MicrophoneInput : MonoBehaviour
{
    [Header("Microphone Settings")]
    public int microphoneIndex = 0;

    [SerializeField]
    public string[] availableMicrophones;

    public string selectedDevice;
    private AudioSource _audioSource;
    private bool _isInitialized;

    public void Start()
    {
        availableMicrophones = Microphone.devices;

        if (availableMicrophones.Length > 0)
        {
            if (microphoneIndex >= 0 && microphoneIndex < availableMicrophones.Length)
            {
                selectedDevice = availableMicrophones[microphoneIndex];
            }
            else
            {
                Debug.LogWarning("L'indice del microfono selezionato non è valido. Si utilizza il microfono predefinito.");
                selectedDevice = availableMicrophones[0];
            }

            _audioSource = GetComponent<AudioSource>();
            InitMic();
            _isInitialized = true;
        }
        else
        {
            Debug.LogError("Nessun microfono rilevato!");
        }
    }

    public void Update()
    {
        if (_isInitialized)
        {
            // Se il microfono viene fermato e l'audio finisce, lo riavvia. 
            // Questo evita di avere una pausa nell'input del microfono
            if (!_audioSource.isPlaying)
            {
                StartMic();
            }
        }
    }

    private void InitMic()
    {
        StopMic();
        StartMic();
    }

    private void StartMic()
    {
        _audioSource.clip = Microphone.Start(selectedDevice, true, 999, 48000);
        _audioSource.loop = true;
        _audioSource.Play();
    }

    private void StopMic()
    {
        _audioSource.Stop();
        Microphone.End(selectedDevice);
    }

    private void OnApplicationQuit()
    {
        StopMic();
    }

    // Metodo per cambiare il microfono di input durante l'esecuzione
    public void SetMicrophoneDevice(int index)
    {
        if (availableMicrophones.Length > index)
        {
            selectedDevice = availableMicrophones[index];
            InitMic();
        }
    }
}
