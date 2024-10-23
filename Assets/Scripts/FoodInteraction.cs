using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodInteraction : MonoBehaviour
{
    private bool isPlayerNearby = false; // Détection de la proximité du joueur
    private GameObject player; // Référence au joueur
    private PickUpDrop playerPickUpDrop; // Référence au script PickUpDrop du joueur

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerPickUpDrop = player.GetComponent<PickUpDrop>();
        }
    }

    private void Update()
    {
        if (isPlayerNearby && playerPickUpDrop != null && playerPickUpDrop.GetCarriedObject() == this.gameObject)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                ConsumeFood();
            }
        }
    }

    // Consommer la nourriture instantanément
    private void ConsumeFood()
    {
        // Détruire l'objet après consommation
        EventManager.TriggerEvent("OnEat");
        Destroy(gameObject);
    }

    // Détection du joueur entrant dans la zone de la nourriture
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    // Détection du joueur quittant la zone de la nourriture
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }
}