using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderInteraction : MonoBehaviour
{
    public Transform topPosition; // Position en haut de l'échelle
    public Transform bottomPosition; // Position en bas de l'échelle
    public Transform player; // Référence au joueur

    private void Start()
    {
        // Trouve le joueur en utilisant un tag (assurez-vous que le joueur a le tag "Player")
        player = FindObjectOfType<PlayerController>().gameObject.transform;
    }

    float interactionCooldown = 0.5f; // Délai de 0.5 secondes
    private float lastInteractionTime = 0f;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Time.time >= lastInteractionTime + interactionCooldown)
        {
            if (Input.GetKey(KeyCode.F))
            {
                lastInteractionTime = Time.time; // Mettre à jour le temps de la dernière interaction
                float distanceToBottom = Vector3.Distance(player.position, bottomPosition.position);
                float distanceToTop = Vector3.Distance(player.position, topPosition.position);

                if (distanceToBottom + 1f < distanceToTop)
                {
                    Debug.Log("going up");
                    TeleportPlayer(topPosition.position);
                }
                else if (distanceToTop + 1f < distanceToBottom)
                {
                    Debug.Log("going down");
                    TeleportPlayer(bottomPosition.position);
                }
            }
        }
    }

    private void TeleportPlayer(Vector3 targetPosition)
    {
        Debug.Log("tp");
        player.position = targetPosition;
    }
}
