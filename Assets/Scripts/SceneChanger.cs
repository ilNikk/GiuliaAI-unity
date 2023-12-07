using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SceneChanger : MonoBehaviour
{
    public string[] sceneNames;

    void Update()
    {
        var keyboard = Keyboard.current;

        if (keyboard.digit1Key.wasPressedThisFrame && sceneNames.Length > 0)
        {
            DisableCurrentSceneObjects();
            SceneManager.LoadScene(sceneNames[0]);
        }
        if (keyboard.digit2Key.wasPressedThisFrame && sceneNames.Length > 1)
        {
            DisableCurrentSceneObjects();
            SceneManager.LoadScene(sceneNames[1]);
        }
        if (keyboard.digit3Key.wasPressedThisFrame && sceneNames.Length > 2)
        {
            DisableCurrentSceneObjects();
            SceneManager.LoadScene(sceneNames[2]);
        }
    }

    private void DisableCurrentSceneObjects()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.scene == SceneManager.GetActiveScene())
            {
                obj.SetActive(false);
            }
        }
    }
}
