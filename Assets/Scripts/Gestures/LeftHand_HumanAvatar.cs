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

    private Transform _rightHandSocket;
    public Transform RightHandSocket
    {
        get
        {
            if (_rightHandSocket == null)
            {
                if (targetGO != null)
                    _rightHandSocket = targetGO.transform.FindChildRecursive("Right_Hand");
            }
            return _rightHandSocket;
        }
    }

    public GameObject socketObject;
    
}
