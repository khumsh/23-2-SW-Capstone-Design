using OculusSampleFramework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeftHand_HumanAvatar : MonoBehaviour, ILeftGesture
{
    public GameObject target;

    public string targetName => LeftHandTargets.HumanAvatar.ToString();
    public GameObject targetGO => target;


    private void Update()
    {
        
    }
}
