using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpDrop : MonoBehaviour
{
    public Transform handPosition;    // Position de la main
    public GameObject initialObject;  // Objet que le personnage tiendra au début du jeu
    public float pickUpRange = 1.0f;  // Distance pour ramasser
    public Vector3 plantedRotation = new Vector3(0, 0, 0); // Rotation verticale
    public float plantHeightOffset = 0.1f;  // Hauteur de l’objet posé

    private GameObject carriedObject = null; // Objet ramassé

    void Start()
    {
        if (initialObject != null)
        {
            // Si un objet initial est défini, le ramasser au début du jeu
            PickUpObject(initialObject);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (carriedObject != null)
            {
                // Si un objet est porté, on le plante
                PlantObject();
            }
            else
            {
                // Sinon, essayer de ramasser un objet
                TryPickUpObject();
            }
        }
    }

    void TryPickUpObject()
    {
        // Chercher les objets autour du personnage
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickUpRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.layer == LayerMask.NameToLayer("GrabObjects"))
            {
                PickUpObject(hitCollider.gameObject);
                break;
            }
        }
    }
    
    void PickUpObject(GameObject obj)
    {
        // Désactiver la physique pour ramasser l'objet
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Empêcher la physique tant qu'il est porté
            rb.useGravity = false; // Désactiver la gravité
        }

        // Positionner et attacher l'objet à la main
        obj.transform.position = handPosition.position;
        obj.transform.rotation = handPosition.rotation;
        obj.transform.parent = handPosition;

        carriedObject = obj;
    }

    void PlantObject()
    {
        // Détacher l'objet de la main
        carriedObject.transform.parent = null;

        // Positionner l'objet au sol avec une orientation verticale
        // Vector3 plantPosition = new Vector3(handPosition.position.x, 0 + plantHeightOffset, handPosition.position.z);
        // carriedObject.transform.position = plantPosition;
        // carriedObject.transform.eulerAngles = plantedRotation;

        // Rendre l'objet immobile mais laisser les collisions actives
        Rigidbody rb = carriedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;  // Garder les collisions actives pour pouvoir le reprendre
        }

        // Laisser le collider actif pour bloquer le personnage
        Collider collider = carriedObject.GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = true;  // Le collider reste actif pour bloquer le joueur
        }

        carriedObject = null;  // Réinitialiser la référence
    }
}