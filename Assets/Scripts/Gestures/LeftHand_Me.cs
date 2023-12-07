using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHand_Me : MonoBehaviour, ILeftGesture
{
    [SerializeField] private GestureDetection_Demo GD;
    public string targetName => "Me";

    public GameObject targetGO => GD.MyTargetGO;
}
