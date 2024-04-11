using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvtHandler : MonoBehaviour
{
    public Action OnAnimEvt;

    public void OnAnimEvent()
    {
        Debug.Log("OnAnimEvent");
        OnAnimEvt?.Invoke();
    }
}
