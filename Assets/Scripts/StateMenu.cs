using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateMenu : MonoBehaviour
{
   public void OnClickQuit()
    {
        Application.Quit();
    }

    public void OnClickPlay()
    {

        SceneManager.LoadScene("Escapade");
    }

}
