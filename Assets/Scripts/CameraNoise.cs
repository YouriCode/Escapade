using UnityEngine;

public class MenuCameraNoise : MonoBehaviour
{
    public float noiseIntensity = 0.1f;  // Intensit� du mouvement (peut �tre ajust� dans l'Inspector)
    public float noiseSpeed = 1.0f;      // Vitesse du mouvement du noise (ajuster selon le besoin)

    private Vector3 originalPosition;    // Position d'origine de la cam�ra
    private Vector3 originalRotation;    // Rotation d'origine de la cam�ra

    void Start()
    {
        // Stocker la position et la rotation d'origine de la cam�ra
        originalPosition = transform.position;
        originalRotation = transform.eulerAngles;
    }

    void Update()
    {
        // Cr�er un l�ger mouvement de "noise" dans la position de la cam�ra
        float noiseX = Mathf.PerlinNoise(Time.time * noiseSpeed, 0.0f) * 2 - 1;
        float noiseY = Mathf.PerlinNoise(0.0f, Time.time * noiseSpeed) * 2 - 1;

        // Appliquer le noise � la position ou � la rotation de la cam�ra
        transform.position = originalPosition + new Vector3(noiseX, noiseY, 0) * noiseIntensity;

        // Si tu veux �galement un l�ger tremblement dans la rotation :
        float rotationNoiseX = Mathf.PerlinNoise(Time.time * noiseSpeed, 1.0f) * 2 - 1;
        float rotationNoiseY = Mathf.PerlinNoise(1.0f, Time.time * noiseSpeed) * 2 - 1;
        transform.eulerAngles = originalRotation + new Vector3(rotationNoiseX, rotationNoiseY, 0) * noiseIntensity;
    }
}
