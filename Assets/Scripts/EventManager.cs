using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class EventManager : MonoBehaviour
{
    private Dictionary<string, UnityEvent> eventDictionary;
    private static EventManager eventManager;
    public static EventManager instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!eventManager)
                {
                    Debug.LogError("There needs to be one active EventManager script on a GameObject in your scene.");
                }
                else
                {
                    eventManager.Init();
                }
            }
            return eventManager;
        }
    }
    private float currentEnergy;
    private float maxEnergy;
    private bool isInWater;

    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, UnityEvent>();
        }
    }

    public static void StartListening(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction listener)
    {
        if (eventManager == null) return;
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(string eventName)
    {
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }
     
    public void UpdateEnergy(float energy, float max)
    {
        currentEnergy = energy;
        maxEnergy = max;
    }

    public float GetCurrentEnergy()
    {
        return currentEnergy;
    }

    public float GetMaxEnergy()
    {
        return maxEnergy;
    }

    public void SetWaterState(bool answer) {
        isInWater = answer;
    }
    
    public bool WaterState() {
        return isInWater;
    }

    public GameObject character;
    public Canvas victoryCanvas; 
    public Canvas defeatCanvas; 
    public void GameOver()
    {
        Debug.Log("Game Over");
        character.GetComponent<PlayerController>().enabled = false;
        character.GetComponent<PickUpDrop>().enabled = false;
        character.GetComponent<Animator>().enabled = false;
        
        StartCoroutine(ShowDefeatCanvas());
    }

    private IEnumerator ShowDefeatCanvas()
    {
        yield return new WaitForSeconds(1);
        defeatCanvas.enabled = true;
    }

    public void Victory() {
        Debug.Log("Gagné");
        character.GetComponent<PlayerController>().enabled = false;
        character.GetComponent<PickUpDrop>().enabled = false;
        character.GetComponent<Animator>().enabled = false;
        StartCoroutine(ShowVictoryCanvas());
    }

    private IEnumerator ShowVictoryCanvas()
    {
        yield return new WaitForSeconds(1);
        victoryCanvas.enabled = true;
    }

    public void Menu() {
        SceneManager.LoadScene("LevelDesign");
    }
}
