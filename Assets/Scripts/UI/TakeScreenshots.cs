using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Diagnostics;
using TMPro;

// TODO:
// Un modo stupido per fare screenshot, ma non va bene perche' ruota la camera per fare il portrait
//
public class ScreenshotController : MonoBehaviour
{
    private GiuliaAIConfig giuliaAIConfig;
    private void Start()
    {
        giuliaAIConfig = Object.FindFirstObjectByType<GiuliaAIConfig>();

        giuliaAIConfig.GUIScreenshotButton.onClick.AddListener(TakeScreenshot);
        giuliaAIConfig.GUIScreenshotFolderButton.onClick.AddListener(OpenScreenshotsFolder);
    }
    public void TakeScreenshot()
    {
        int width = giuliaAIConfig.MainCameraCanvasImage.texture.width;
        int height = giuliaAIConfig.MainCameraCanvasImage.texture.height;

        if (giuliaAIConfig.GUIScreenshotResolutionDropdown.value == 1)
        {
            Quaternion originalRotation = giuliaAIConfig.MainCamera.transform.rotation;
            giuliaAIConfig.MainCamera.transform.rotation = Quaternion.Euler(giuliaAIConfig.MainCamera.transform.eulerAngles.x, giuliaAIConfig.MainCamera.transform.eulerAngles.y, 90);
            giuliaAIConfig.MainCamera.Render();
            DoScreenshot(width, height);
            giuliaAIConfig.MainCamera.transform.rotation = originalRotation;
        }
        else
        {
            DoScreenshot(width, height);
        }
    }

    void DoScreenshot(int width, int height)
    {
        Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, false);
        RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
        Graphics.Blit(giuliaAIConfig.MainCameraCanvasImage.texture, renderTexture); //verificare se va bene giuliaAIConfig.MainCameraCanvasImage.texture
        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture2D.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(renderTexture);
        byte[] byteArray = texture2D.EncodeToPNG();

        string directoryPath = Application.dataPath + "/Screenshots";
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string fileName = "Screenshot-" + giuliaAIConfig.MainCameraCanvasImage.name + "-" + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + (width > height ? "_16_9.png" : "_9_16.png");
        File.WriteAllBytes(Path.Combine(directoryPath, fileName), byteArray);

        //se resolutionDropdown.value == 1 mettiamo la camera asse Z a 0

        Destroy(texture2D);
}

    public void OpenScreenshotsFolder()
    {
        string folderPath = Path.Combine(Application.dataPath, "Screenshots");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        Process.Start(folderPath);
    }
}
