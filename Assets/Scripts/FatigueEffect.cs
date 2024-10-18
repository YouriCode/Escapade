using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class FatigueEffect : MonoBehaviour
{
    public PostProcessVolume postProcessVolume; // Référence au volume de post-processing
    private ColorGrading colorGrading;
    private Vignette vignette;

    private void Start()
    {
        if (postProcessVolume != null)
        {
            postProcessVolume.profile.TryGetSettings(out colorGrading);
            postProcessVolume.profile.TryGetSettings(out vignette);
        }
        else
        {
            Debug.LogError("PostProcessVolume is not assigned!");
        }
    }

    private void Update()
    {
        // Récupérer l'énergie actuelle depuis l'EventManager
        float currentEnergy = EventManager.instance.GetCurrentEnergy();
        float maxEnergy = EventManager.instance.GetMaxEnergy();

        // Ajuster les effets visuels en fonction de l'énergie
        float fatigueFactor = 1 - (currentEnergy / maxEnergy);
        colorGrading.saturation.value = Mathf.Lerp(0, -100, fatigueFactor);
        vignette.intensity.value = Mathf.Lerp(0.2f, 0.6f, fatigueFactor);
    }
}
