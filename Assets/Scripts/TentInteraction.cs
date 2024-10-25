using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentInteraction : MonoBehaviour
{
    public GameObject fabricPrefab;
    public Transform spawnPoint;
    private PickUpDrop playerPickUpDrop;
    private Outline outline;  // Référence au script Outline
    private bool isOutlineActive = false; // État actuel de l'outline
    private float interactionCooldown = 0.5f;
    private float lastInteractionTime = 0f;

    private void Start()
    {
        // Chercher le script PickUpDrop sur le joueur
        playerPickUpDrop = FindObjectOfType<PickUpDrop>();
        outline = GetComponentInChildren<Outline>(); 

        // S'assurer que l'outline est désactivé au départ
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    private void Update()
    {
        if (playerPickUpDrop != null)
        {
            GameObject carriedObject = playerPickUpDrop.GetCarriedObject();

            // Activer ou désactiver l'outline selon l'objet porté par le joueur
            if (carriedObject != null && carriedObject.CompareTag("Knife") && !isOutlineActive)
            {
                ActivateOutline(true);
            }
            else if ((carriedObject == null || !carriedObject.CompareTag("Knife")) && isOutlineActive)
            {
                ActivateOutline(false);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Time.time >= lastInteractionTime + interactionCooldown) // Vérifier le délai
        {
            if (Input.GetKey(KeyCode.F))
            {
                lastInteractionTime = Time.time; // Mettre à jour le temps de la dernière interaction

                if (playerPickUpDrop != null && playerPickUpDrop.GetCarriedObject() != null &&
                    playerPickUpDrop.GetCarriedObject().CompareTag("Knife")) 
                {
                    Loot();
                }
            }
        }
    }

    private void Loot()
    {
        Instantiate(fabricPrefab, spawnPoint.position, Quaternion.identity);
        Destroy(gameObject);
    }

    // Activer ou désactiver le script Outline
    private void ActivateOutline(bool activate)
    {
        if (outline != null)
        {
            outline.enabled = activate;
            isOutlineActive = activate;
        }
    }
}
