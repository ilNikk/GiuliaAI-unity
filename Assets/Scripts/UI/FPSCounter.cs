using UnityEngine;
using TMPro;
public class FPSCounter : MonoBehaviour
{
    private GiuliaAIConfig giuliaAIConfig;
    private float deltaTime = 0.0f;
    void Start()
    {
        giuliaAIConfig = Object.FindFirstObjectByType<GiuliaAIConfig>();
        if (giuliaAIConfig == null)
        {
            Debug.LogError("Impossibile trovare un'istanza di GiuliaAIConfig nell'ambiente!");
        }
    }
    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        giuliaAIConfig.GUIFPSCounter.text = string.Format("{0:0.} biscotti al secondo", fps);
    }
}
