using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingZone : MonoBehaviour
{
    [System.Serializable]
    public class CraftingRequirement
    {
        public string itemTag;  // Tag de l'objet requis
        public int requiredAmount; // Quantité requise
    }

    public List<CraftingRequirement> requiredItems; // Liste des objets requis pour le craft
    private Dictionary<string, int> itemsInZone = new Dictionary<string, int>(); // Dictionnaire des objets dans la zone de craft
    public GameObject craftResult;
    public bool finalCraftingZone = false;

    private void OnTriggerEnter(Collider other)
    {
        // Vérifier si l'objet entrant est un objet requis
        if (IsRequiredItem(other.gameObject.tag))
        {
            // Ajouter l'objet à la liste des objets dans la zone
            if (itemsInZone.ContainsKey(other.gameObject.tag))
            {
                itemsInZone[other.gameObject.tag]++;
            }
            else
            {
                itemsInZone[other.gameObject.tag] = 1;
            }

            Debug.Log(other.gameObject.name + " ajouté à la zone de craft.");

            // Vérifier si tous les objets requis sont présents pour lancer le craft
            CheckCraftingItems();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Retirer l'objet de la liste s'il quitte la zone
        if (itemsInZone.ContainsKey(other.gameObject.tag))
        {
            itemsInZone[other.gameObject.tag]--;
            if (itemsInZone[other.gameObject.tag] <= 0)
            {
                itemsInZone.Remove(other.gameObject.tag);
            }

            Debug.Log(other.gameObject.name + " retiré de la zone de craft.");
        }
    }

    private bool IsRequiredItem(string tag)
    {
        foreach (var requirement in requiredItems)
        {
            if (requirement.itemTag == tag)
            {
                return true;
            }
        }
        return false;
    }

    private void CheckCraftingItems()
    {
        // Vérifier si tous les objets requis sont présents dans la zone
        foreach (var requirement in requiredItems)
        {
            if (!itemsInZone.ContainsKey(requirement.itemTag) || itemsInZone[requirement.itemTag] < requirement.requiredAmount)
            {
                Debug.Log("Il manque des objets pour le craft.");
                return;
            }
        }

        // Si tous les objets sont présents, lancer le craft
        Craft();
    }
    public float craftPositionOffsetX = 1f;
    public float craftPositionOffsetY = 1f;
    public float craftPositionOffsetZ = 1f;
    public float craftRotationX = 0f;
    public float craftRotationY = 0f;
    public float craftRotationZ = 0f;
    private void Craft()
    {
        Debug.Log("Tous les objets sont présents ! Craft réussi !");

        foreach (var requirement in requiredItems)
        {
            // Vérifier si des objets requis sont dans la zone de craft
            if (itemsInZone.ContainsKey(requirement.itemTag))
            {
                int amountToDestroy = Mathf.Min(itemsInZone[requirement.itemTag], requirement.requiredAmount);

                // Trouver les objets dans la zone de craft ayant le tag requis
                GameObject[] objectsInZone = GameObject.FindGameObjectsWithTag(requirement.itemTag);
                Debug.Log($"Essai de détruire {amountToDestroy} de {requirement.itemTag}. Objets trouvés : {objectsInZone.Length}");

                int destroyedCount = 0;
                foreach (var obj in objectsInZone)
                {
                    Collider objCollider = obj.GetComponent<Collider>();
                    if (objCollider != null)
                    {
                        // Vérifier si l'objet est dans la zone de craft
                        if (objCollider.bounds.Intersects(GetComponent<Collider>().bounds))
                        {
                            Destroy(obj);
                            destroyedCount++;
                            Debug.Log($"{obj.name} détruit.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"{obj.name} n'a pas de collider, il ne peut pas être détruit.");
                    }

                    // Arrêter si on a détruit le nombre requis d'objets
                    if (destroyedCount >= amountToDestroy)
                        break;
                }

                // Mettre à jour le nombre d'objets dans la zone
                itemsInZone[requirement.itemTag] -= destroyedCount;
                if (itemsInZone[requirement.itemTag] <= 0)
                {
                    itemsInZone.Remove(requirement.itemTag);
                }
            }
        }

        Vector3 craftPosition = new Vector3(transform.position.x + craftPositionOffsetX, transform.position.y + craftPositionOffsetY, transform.position.z + craftPositionOffsetZ);
        Quaternion craftRotation = Quaternion.Euler(craftRotationX, craftRotationY, craftRotationZ);
        Instantiate(craftResult, craftPosition, craftRotation);

        if (finalCraftingZone) {
            EventManager.instance.Victory();
        }
    }



}
