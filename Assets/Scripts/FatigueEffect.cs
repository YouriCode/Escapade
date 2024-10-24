using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FatigueEffect : MonoBehaviour
{
    public Volume volume; // Référence au volume de post-processing
    private ColorAdjustments colorAdjustments;
    private Vignette vignette;
    private DepthOfField depthOfField;
    public float vignetteMin = .2f;
    public float vignetteMax = .6f;
    public float desaturationMin = 0f;
    public float desaturationMax = -100f;
    public float focalLengthMin = 1f;
    public float focalLengthMax = 20f;

    private void Start()
    {
        if (volume.profile.TryGet<Vignette>(out vignette))
        {
            vignette.intensity.overrideState = true;
        }

        if (volume.profile.TryGet<ColorAdjustments>(out colorAdjustments))
        {
            colorAdjustments.saturation.overrideState = true;
        }
        if (volume.profile.TryGet<DepthOfField>(out depthOfField))
        {
            depthOfField.focalLength.overrideState = true;
        }
    }

    private void Update()
    {
        // Récupérer l'énergie actuelle depuis l'EventManager
        float currentEnergy = EventManager.instance.GetCurrentEnergy();
        float energyThreshold = EventManager.instance.GetMaxEnergy() / 2f;

        if (currentEnergy < energyThreshold)
        {
            float energyPercentage = (energyThreshold - currentEnergy) / energyThreshold;

            float vignetteIntensity = Mathf.Lerp(vignetteMin, vignetteMax, energyPercentage);
            vignette.intensity.value = vignetteIntensity;

            float desaturationValue = Mathf.Lerp(desaturationMin, desaturationMax, energyPercentage);
            colorAdjustments.saturation.value = desaturationValue;

            float focalLengthValue = Mathf.Lerp(focalLengthMin, focalLengthMax, energyPercentage);
            depthOfField.focalLength.value = focalLengthValue;
        }
        else
        {
            vignette.intensity.value = vignetteMin;
            colorAdjustments.saturation.value = desaturationMin;
            depthOfField.focalLength.value = focalLengthMin;
        }
    }
}
