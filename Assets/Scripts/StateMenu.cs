using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateMenu : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject fondu;
    public StateMachine state;

    public void OnClickQuit()
    {
#if UNITY_EDITOR
        // Si nous sommes dans l'éditeur, quitter simplement le mode Play
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Si nous sommes dans une build, fermer l'application
        Application.Quit();
#endif
    }

    public void OnClickPlay()
    {
        animator.SetTrigger("FadeOut");
        fondu.SetActive(true);
    }
    public void OnClickCredits()
    {
        state.SetState(State.CREDITS);
    }


    public void OnFadeComplete()
    {
        SceneManager.LoadScene("Escapade");
    }
}
