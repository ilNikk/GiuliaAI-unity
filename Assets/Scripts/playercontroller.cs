/*using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // Punto attorno al quale la telecamera ruoterà
    public float rotationSpeed = 10.0f; // Velocità di rotazione
    public float smoothFactor = 0.125f;
    public float zoomSpeed = 10.0f; // Velocità di zoom
    public float minFOV = 10.0f; // Campo visivo minimo (zoom in massimo)
    public float maxFOV = 50.0f; // Campo visivo massimo (zoom out massimo)

    private Camera cam;
    private Vector3 currentRotation;

    private void Start()
    {
        cam = GetComponent<Camera>();
        currentRotation = transform.eulerAngles;
    }

    void Update()
    {
        // Ottieni input di rotazione
        float horizontal = Input.GetAxis("RightStickHorizontal");
        float vertical = Input.GetAxis("RightStickVertical");

        // Calcola l'angolo target
        currentRotation.y += horizontal * rotationSpeed;
        currentRotation.x -= vertical * rotationSpeed;

        // Interpolazione lineare per ottenere una rotazione fluida
        Vector3 smoothedRotation = Vector3.Lerp(transform.eulerAngles, currentRotation, smoothFactor);
        transform.eulerAngles = smoothedRotation;

        // Posiziona la camera attorno al target
        transform.position = target.position;
        transform.Translate(Vector3.back);

        // Gestione dello zoom con i trigger LT e RT
        float triggerValue = Input.GetAxis("Triggers");
        
        if (triggerValue > 0)
        {
            cam.fieldOfView -= triggerValue * zoomSpeed * Time.deltaTime;
        }
        else if (triggerValue < 0)
        {
            cam.fieldOfView += -triggerValue * zoomSpeed * Time.deltaTime;
        }
        
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minFOV, maxFOV);
    }
}*/