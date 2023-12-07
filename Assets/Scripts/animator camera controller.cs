using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Animator animator;
    public string fovParameterName = "FOV";
    public string rotationXParameterName = "RotationX";
    public string rotationYParameterName = "RotationY";
    public string rotationZParameterName = "RotationZ";

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (animator)
        {
            float fov = animator.GetFloat(fovParameterName);
            float rotationX = animator.GetFloat(rotationXParameterName);
            float rotationY = animator.GetFloat(rotationYParameterName);
            float rotationZ = animator.GetFloat(rotationZParameterName);

            // Applica il FOV
            if(cam.fieldOfView != fov)
                cam.fieldOfView = fov;

            // Applica la rotazione
            Vector3 newRotation = new Vector3(rotationX, rotationY, rotationZ);
            if(transform.eulerAngles != newRotation)
                transform.eulerAngles = newRotation;
        }
    }
}
