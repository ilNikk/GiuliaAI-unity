using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Bootstrap : MonoBehaviour
{
    public string mainSceneName;  // Inserisci il nome della scena principale qui
    public Slider progressBar;    // Assegna uno Slider di Unity qui

    void Start()
    {
        if (!string.IsNullOrEmpty(mainSceneName))
        {
            StartCoroutine(LoadSceneAsync(mainSceneName));
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        // Disattiva la navigazione automatica della scena
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            // Aggiorna la barra di avanzamento
            progressBar.value = asyncOperation.progress;

            // Se il caricamento è quasi finito, attiva la scena
            if (asyncOperation.progress >= 0.9f)
            {
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
