using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class CraftingZone : MonoBehaviour
{
    public List<string> requiredItems; // Liste des tags des objets requis pour le craft
    private List<GameObject> itemsInZone = new List<GameObject>(); // Liste des objets dans la zone de craft

    private void OnTriggerEnter(Collider other)
    {
        // Vérifier si l'objet entrant est un objet requis
        if (requiredItems.Contains(other.gameObject.tag))
        {
            // Ajouter l'objet à la liste des objets dans la zone
            itemsInZone.Add(other.gameObject);
            Debug.Log(other.gameObject.name + " ajouté à la zone de craft.");

            // Vérifier si tous les objets requis sont présents pour lancer le craft
            CheckCraftingItems();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Retirer l'objet de la liste s'il quitte la zone
        if (itemsInZone.Contains(other.gameObject))
        {
            itemsInZone.Remove(other.gameObject);
            Debug.Log(other.gameObject.name + " retiré de la zone de craft.");
        }
    }

    private void CheckCraftingItems()
    {
        // Vérifier si tous les objets requis sont présents dans la zone
        foreach (string requiredItem in requiredItems)
        {
            bool itemFound = false;
            foreach (GameObject obj in itemsInZone)
            {
                if (obj.tag == requiredItem)
                {
                    itemFound = true;
                    break;
                }
            }
            if (!itemFound)
            {
                Debug.Log("Il manque des objets pour le craft.");
                return;
            }
        }

        // Si tous les objets sont présents, lancer le craft
        Craft();
    }

    private void Craft()
    {
        // Lancer le craft ici (par exemple, créer une hache)
        Debug.Log("Tous les objets sont présents ! Craft réussi !");

        foreach (GameObject item in itemsInZone)
        {
            if (requiredItems.Contains(item.gameObject.tag))
            {
                Destroy(item);
            }
        }
    }
}
