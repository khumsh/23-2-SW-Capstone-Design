using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHand_Ball : MonoBehaviour, ILeftGesture
{
    public GameObject target;

    public string targetName => LeftHandTargets.Ball.ToString();
    public GameObject targetGO => target;
}
