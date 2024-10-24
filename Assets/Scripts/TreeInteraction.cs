using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeInteraction : MonoBehaviour
{
    public GameObject branchPrefab;      // Préfab pour les branches
    public GameObject logPrefab;         // Préfab pour les bûches
    public GameObject cutTreePrefab;     // Préfab pour l'arbre coupé
    public float dropRadius = 2f;        // Rayon autour de l'arbre pour faire tomber les objets
    public int hitsToCutDown = 3;        // Nombre de coups avant que l'arbre tombe

    private int hitCount = 0;            // Compteur de coups
    private PickUpDrop playerPickUpDrop; // Référence au script PickUpDrop du joueur

    private void Start()
    {
        // Chercher le script PickUpDrop sur le joueur
        playerPickUpDrop = FindObjectOfType<PickUpDrop>();
    }

    float interactionCooldown = 0.5f; // Délai entre les interactions
    private float lastInteractionTime = 0f;  // Temps de la dernière interaction

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Time.time >= lastInteractionTime + interactionCooldown) // Vérifier le délai
        {
            if (Input.GetKey(KeyCode.F))
            {
                lastInteractionTime = Time.time; // Mettre à jour le temps de la dernière interaction

                if (playerPickUpDrop != null && playerPickUpDrop.GetCarriedObject() != null &&
                    playerPickUpDrop.GetCarriedObject().CompareTag("Axe")) // Si le joueur porte une hache
                {
                    // Incrémenter le compteur de coups
                    hitCount++;

                    // Si l'arbre a reçu assez de coups, faire apparaître l'arbre coupé
                    if (hitCount >= hitsToCutDown)
                    {
                        // Faire apparaître l'arbre coupé à la place de l'arbre actuel
                        Instantiate(cutTreePrefab, transform.position, transform.rotation);

                        // Détruire cet arbre
                        Destroy(gameObject);
                    }
                    else
                    {
                        // Faire tomber des bûches
                        DropItems(logPrefab, Random.Range(1, 3));
                    }
                }
                else
                {
                    // Faire tomber des branches si le joueur ne porte pas de hache
                    DropItems(branchPrefab, Random.Range(1, 3));
                }
            }
        }
    }



    // Fonction pour instancier des objets avec une position aléatoire proche de l'arbre
    private void DropItems(GameObject itemPrefab, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 randomDropPosition = transform.position + new Vector3(
                Random.Range(-dropRadius, dropRadius),
                5,
                Random.Range(-dropRadius, dropRadius)
            );
            Instantiate(itemPrefab, randomDropPosition, Quaternion.identity);
        }
    }
}
