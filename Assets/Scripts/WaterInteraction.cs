using System.Collections;
using UnityEngine;

public class WaterZone : MonoBehaviour
{
    public float underwaterSpeedFactor = 0.5f; // Réduction de vitesse dans l'eau
    public float energyDrainRate = 10f; // Taux de perte d'énergie par seconde
    public float energyRegenRate = 5f;

    private float initialSpeedFactor;
    private float initialEnergy;
    private float currentEnergy;
    private PlayerController playerController;
    private bool isInWater = false;
    private bool isOutOfWater = false;
    private float maxEnergy;
    private float drainTimer = 0f;

    void Start()
    {
        maxEnergy = EventManager.instance.GetMaxEnergy();
    }

    private void Update()
    {
        currentEnergy = EventManager.instance.GetCurrentEnergy();

        if (isInWater)
        {
            drainTimer += Time.deltaTime;
            if (drainTimer >= .1f) // Appliquer la perte d'énergie chaque seconde
            {
                EventManager.instance.UpdateEnergy(currentEnergy - energyDrainRate, maxEnergy);
                drainTimer = 0f;
            }
        }

        if (isOutOfWater)
        {
            EventManager.instance.UpdateEnergy(currentEnergy + 100, maxEnergy);

        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController = other.GetComponent<PlayerController>();

            if (playerController != null)
            {
                isInWater = true;
                EventManager.instance.SetWaterState(true);
                initialSpeedFactor = playerController.speedFactor;
                initialEnergy = EventManager.instance.GetCurrentEnergy();
                isOutOfWater = false;

                playerController.speedFactor = underwaterSpeedFactor; // Réduction de la vitesse
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isInWater)
        {
            isInWater = false;
            EventManager.instance.SetWaterState(false);
            playerController.speedFactor = initialSpeedFactor; // Rétablit la vitesse initiale
            isOutOfWater = true;
        }
    }
}
