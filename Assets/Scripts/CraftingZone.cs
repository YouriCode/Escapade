using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingZone : MonoBehaviour
{
    // Liste des objets requis pour le craft
    public List<string> requiredItems = new List<string>() { "Wood", "Stone" };
    
    // Liste des objets que le joueur a déposés
    private List<string> depositedItems = new List<string>();

    void OnTriggerEnter(Collider other)
    {
        // Vérifie si l'objet est un objet de craft valide
        if (requiredItems.Contains(other.gameObject.tag))
        {
            depositedItems.Add(other.gameObject.tag);
            Debug.Log("Item déposé: " + other.gameObject.tag);
            Destroy(other.gameObject);  // Simule le dépôt de l'objet

            // Vérifie si tous les objets requis sont déposés
            CheckCrafting();
        }
    }

    void CheckCrafting()
    {
        // Si tous les objets requis sont dans la liste
        if (depositedItems.Contains("Wood") && depositedItems.Contains("Stone"))
        {
            Debug.Log("Craft réussi ! Vous avez fabriqué une hache.");
            // Ici, tu peux déclencher la fabrication de l'objet (par exemple, instancier une hache)
        }
    }
}
