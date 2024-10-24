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
        EventManager.StartListening("OnJump", OnJump);
        EventManager.StartListening("OnEat", RegainEnergy);

        //InvokeRepeating("LogMessage", 1f, 1f);
    }

    void OnDisable()
    {
        EventManager.StopListening("OnSprint", StartSprint);
        EventManager.StopListening("OnJump", OnJump);

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
            }
            else
            {
                currentEnergy -= energyDepletionRate * Time.deltaTime;
            }
        }
        else
        {
            EventManager.instance.GameOver();
        }

        currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);
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

    void OnJump()
    {
        currentEnergy -= 1f;
    }

    void RegainEnergy() {
        currentEnergy += 100f;
        Debug.Log("fruit mangé " + currentEnergy);
    }

    void LogMessage()
    {
        Debug.Log(currentEnergy);
    }
}
