using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightHand_Go : MonoBehaviour
{
    [SerializeField] private GestureDetection_Demo GD;

    public GameObject targetGO;

    private string currentInterface;

    private void Update()
    {
        currentInterface = GD.RecognizeRight().name;
        targetGO = GD.targetGO;

        if (currentInterface == "Go")
        {
            if (GD.targetName == "We")
            {
                foreach (GameObject t in GD.leftHandTargets)
                {
                    t.transform.position += Vector3.forward * 0.3f * Time.deltaTime;
                }

            }
            else if (targetGO != null)
                targetGO.transform.position += Vector3.forward * 0.3f * Time.deltaTime;
            
        }
        else
        {
            targetGO = null;
        }
    }

}
