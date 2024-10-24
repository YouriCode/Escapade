using UnityEngine;

public enum State
{
    MENU,
    CREDITS,
}

public class StateMachine : MonoBehaviour
{
    public State state;

    public GameObject guiMenu;
    public GameObject guiCredits;

    static public StateMachine instance;  //singleton

    void Awake()
    {
        if (instance != null) Debug.LogError("Double singleton!");
        instance = this;
    }

    void Start()
    {
        state = State.MENU;
        guiMenu.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        guiCredits.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

    }

    void Update()
    {
        guiMenu.SetActive(state == State.MENU);
        guiCredits.SetActive(state == State.CREDITS);
    }
    public void SetState(State newState)
    {
        state = newState;
    }
}
