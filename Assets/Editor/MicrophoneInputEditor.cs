using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MicrophoneInput))]
public class MicrophoneInputEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MicrophoneInput micInput = (MicrophoneInput)target;

        if (micInput.availableMicrophones.Length > 0)
        {
            int selectedIndex = EditorGUILayout.Popup("Select Microphone", micInput.microphoneIndex, micInput.availableMicrophones);

            if (selectedIndex != micInput.microphoneIndex)
            {
                micInput.microphoneIndex = selectedIndex;
                micInput.SetMicrophoneDevice(selectedIndex);
            }
        }
        else
        {
            EditorGUILayout.LabelField("No microphones available.");
        }
    }
}
