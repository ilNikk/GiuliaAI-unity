using System.Runtime.InteropServices;
using UnityEngine;
using System.IO;

public class MouseTracking : MonoBehaviour
{
    private GiuliaAIConfig giuliaAIConfig;
    private bool setActive = false;
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;
    }

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT pos);
    private Vector2 screenBounds;
    private float currentHorizontalAngle = 0f;
    private float currentVerticalAngle = 0f;

    void Start()
    {
        giuliaAIConfig = GameObject.FindFirstObjectByType<GiuliaAIConfig>();
        
        InvokeRepeating("CheckFileAndToggleObject", 0, giuliaAIConfig.OSUControllerCheckInterval);

        screenBounds = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
    }

    void LateUpdate()
    {
        if (setActive)
        {
            TrackMouse();
        }

    }

    float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
    {
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }

    void CheckFileAndToggleObject()
    {
        if (File.Exists(giuliaAIConfig.OSUControllerFilePath))
        {
            string content = File.ReadAllText(giuliaAIConfig.OSUControllerFilePath).Trim();

            if (content == "1")
            {
                setActive = true;
            }
            else if (content == "0")
            {
                setActive = false;
            }
        }
        else
        {
            Debug.LogWarning("GiuliaAI_OsuControllerBool not found at: " + giuliaAIConfig.OSUControllerFilePath);
        }
    }

    void TrackMouse()
    {
        if (giuliaAIConfig.headTransform == null) return;

        POINT mousePosition;
        if (GetCursorPos(out mousePosition))
        {
            Vector2 mousePos = new Vector2(mousePosition.x, mousePosition.y);
            Vector2 normalizedMousePos = new Vector2(mousePos.x / screenBounds.x, mousePos.y / screenBounds.y);

            if (giuliaAIConfig.OSUControllerLogDebug)
            {
                Debug.Log("Mouse Pos: " + mousePos + " Normalized Mouse Pos: " + normalizedMousePos);
            }

            float targetHorizontalAngle = Map(normalizedMousePos.x, 0, 1, giuliaAIConfig.OSUControllerMaxHorizontalAngle, -giuliaAIConfig.OSUControllerMaxHorizontalAngle);
            float targetVerticalAngle = Map(normalizedMousePos.y, 0, 1, -giuliaAIConfig.OSUControllerMaxVerticalAngle, giuliaAIConfig.OSUControllerMaxVerticalAngle);

            currentHorizontalAngle = Mathf.Lerp(currentHorizontalAngle, targetHorizontalAngle, Time.deltaTime * giuliaAIConfig.OSUControllerSmoothFactor);
            currentVerticalAngle = Mathf.Lerp(currentVerticalAngle, targetVerticalAngle, Time.deltaTime * giuliaAIConfig.OSUControllerSmoothFactor);

            giuliaAIConfig.headTransform.localRotation = Quaternion.Euler(currentVerticalAngle, currentHorizontalAngle, 0);

            SkinnedMeshRenderer skinnedMeshRenderer = giuliaAIConfig.facePrefab.GetComponentInChildren<SkinnedMeshRenderer>();
            int blendShapeIndex = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex("Fcl_ALL_Angry");
            if (blendShapeIndex != -1)
            {
                skinnedMeshRenderer.SetBlendShapeWeight(blendShapeIndex, 100f);
                //all other blendshapes to 0
                for (int i = 0; i < skinnedMeshRenderer.sharedMesh.blendShapeCount; i++)
                {
                    if (i != blendShapeIndex)
                    {
                        skinnedMeshRenderer.SetBlendShapeWeight(i, 0f);
                    }
                }
            }
        }
    }
}
