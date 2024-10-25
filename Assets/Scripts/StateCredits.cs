using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateCredits : MonoBehaviour
{
    public StateMachine state;

    public void OnClickBack()
    {
        state.SetState(State.MENU);
    }
}
