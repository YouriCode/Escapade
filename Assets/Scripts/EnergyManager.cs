using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnergyManager : MonoBehaviour
{
    public float maxEnergy = 100f;
    public float energy;
    public float energyDepletionRate = 1f;
    public float energyDepletionFactor = 2f;

    private bool isSprinting = false;

    void OnEnable()
    {
        energy = maxEnergy;

        EventManager.StartListening("OnSprint", StartSprint);
        EventManager.StartListening("OnWalk", StopSprint);
        // EventManager.StartListening("OnEat", RegainEnergy);

        //InvokeRepeating("LogMessage", 1f, 1f);
    }

    void OnDisable()
    {
        EventManager.StopListening("OnSprint", StartSprint);
        EventManager.StopListening("OnWalk", StopSprint);

        CancelInvoke("LogMessage");
    }

    void Update()
    {
        if (energy > 0)
        {
            energy -= energyDepletionRate * Time.deltaTime;

            if (isSprinting)
            {
                energy -= energyDepletionRate * energyDepletionFactor * Time.deltaTime;
                Debug.Log("sprinting");
            }
            else
            {
                energy -= energyDepletionRate * Time.deltaTime;
            }
        }
        else
        {
            Debug.Log("No More Energy!");
        }

        energy = Mathf.Clamp(energy, 0f, 100f);

    }

    void StartSprint()
    {
        isSprinting = true;
    }

    void StopSprint()
    {
        isSprinting = false;
    }

    //void RegainEnergy() {}

    void LogMessage()
    {
        Debug.Log(energy);
    }
}