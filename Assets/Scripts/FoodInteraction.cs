using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodInteraction : MonoBehaviour
{
    private PickUpDrop playerPickUpDrop;

    private void Start()
    {
        playerPickUpDrop = FindObjectOfType<PickUpDrop>();
    }

    private void Update()
    {
        if (playerPickUpDrop != null && playerPickUpDrop.GetCarriedObject() != null && playerPickUpDrop.GetCarriedObject() == this.gameObject)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                ConsumeFood();
            }
        }
    }

    private void ConsumeFood()
    {
        EventManager.TriggerEvent("OnEat");
        Debug.Log("Nourriture consommée par : " + gameObject.name);

        Destroy(gameObject);
    }
}