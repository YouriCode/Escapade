using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnergyManager : MonoBehaviour
{
    public float maxEnergy = 100f;
    public float currentEnergy { get; private set; }

    public float energyDepletionRate = 1f;
    public float energyDepletionFactor = 2f;

    private bool isSprinting = false;

    void OnEnable()
    {
        currentEnergy = maxEnergy;

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
        if (currentEnergy > 0)
        {
            currentEnergy -= energyDepletionRate * Time.deltaTime;

            if (isSprinting)
            {
                currentEnergy -= energyDepletionRate * energyDepletionFactor * Time.deltaTime;
                Debug.Log("sprinting");
            }
            else
            {
                currentEnergy -= energyDepletionRate * Time.deltaTime;
            }
        }
        else
        {
            Debug.Log("No More Energy!");
        }

        currentEnergy = Mathf.Clamp(currentEnergy, 0f, 100f);
        EventManager.instance.UpdateEnergy(currentEnergy, maxEnergy);
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
        Debug.Log(currentEnergy);
    }
}
