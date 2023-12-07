using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleScript : MonoBehaviour
{
    public MonoBehaviour sleepingScript;

    void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            sleepingScript.enabled = !sleepingScript.enabled;
        }
    }
}
