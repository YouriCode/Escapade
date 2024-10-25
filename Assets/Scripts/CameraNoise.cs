using UnityEngine;

public class MenuCameraNoise : MonoBehaviour
{
    public float noiseIntensity = 0.1f;  // Intensité du mouvement (peut être ajusté dans l'Inspector)
    public float noiseSpeed = 1.0f;      // Vitesse du mouvement du noise (ajuster selon le besoin)

    private Vector3 originalPosition;    // Position d'origine de la caméra
    private Vector3 originalRotation;    // Rotation d'origine de la caméra

    void Start()
    {
        // Stocker la position et la rotation d'origine de la caméra
        originalPosition = transform.position;
        originalRotation = transform.eulerAngles;
    }

    void Update()
    {
        // Créer un léger mouvement de "noise" dans la position de la caméra
        float noiseX = Mathf.PerlinNoise(Time.time * noiseSpeed, 0.0f) * 2 - 1;
        float noiseY = Mathf.PerlinNoise(0.0f, Time.time * noiseSpeed) * 2 - 1;

        // Appliquer le noise à la position ou à la rotation de la caméra
        transform.position = originalPosition + new Vector3(noiseX, noiseY, 0) * noiseIntensity;

        // Si tu veux également un léger tremblement dans la rotation :
        float rotationNoiseX = Mathf.PerlinNoise(Time.time * noiseSpeed, 1.0f) * 2 - 1;
        float rotationNoiseY = Mathf.PerlinNoise(1.0f, Time.time * noiseSpeed) * 2 - 1;
        transform.eulerAngles = originalRotation + new Vector3(rotationNoiseX, rotationNoiseY, 0) * noiseIntensity;
    }
}
