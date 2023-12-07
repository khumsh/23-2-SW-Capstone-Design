using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightHand_Angle : MonoBehaviour
{
    [SerializeField] private GestureDetection_Demo GD;
    [SerializeField] private Transform rightHandTr;

    public GameObject targetGO;

    private string currentInterface;

    private void Update()
    {
        currentInterface = GD.RecognizeRight().name;
        targetGO = GD.targetGO;

        if (currentInterface == "Angle")
        {
            if (targetGO != null)
                targetGO.transform.localRotation = rightHandTr.localRotation;
        }
        else
        {
            targetGO = null;
        }
    }
}
