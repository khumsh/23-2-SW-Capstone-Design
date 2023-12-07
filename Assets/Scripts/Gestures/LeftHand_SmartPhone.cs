using OculusSampleFramework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeftHand_SmartPhone : MonoBehaviour, ILeftGesture
{
    public GameObject target;

    public string targetName => LeftHandTargets.SmartPhone.ToString();
    public GameObject targetGO => target;

    
}
