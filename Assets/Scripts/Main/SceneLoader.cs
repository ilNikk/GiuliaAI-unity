using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private Object[] scenesToLoad;

    IEnumerator Start()
    {
        foreach (Object scene in scenesToLoad)
        {
            if (scene != null)
            {
                yield return StartCoroutine(LoadSceneAsync(scene.name));
            }
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
