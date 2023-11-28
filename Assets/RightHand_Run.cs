using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RightHand_Run : MonoBehaviour
{
    [SerializeField] private GestureDetection_Demo GD;

    public GameObject targetGO;

    [SerializeField] private TextMeshPro velocityText;

    private string currentInterface;

    int count = 1;
    int c = 0;

    Vector2 index_0;
    Vector2 index_1;

    Vector2 index_v;

    Vector2 middle_0;
    Vector2 middle_1;

    Vector2 middle_v;

    void Update()
    {
        currentInterface = GD.RecognizeRight().name;
        targetGO = GD.targetGO;

        if (currentInterface == "Run")
        {
            if (targetGO != null)
            {
                // 구현
                if (!GD.thereAreBonesRight) return;

                float max = Mathf.Max(index_v.magnitude * 100, middle_v.magnitude * 100);

                if (max >= 0.8f && max < 1.2f)
                {
                    targetGO.transform.position += Vector3.forward * 0.05f * Time.deltaTime;
                }
                else if (max >= 1.2f)
                {
                    targetGO.transform.position += Vector3.forward * 0.15f * Time.deltaTime;
                }
                
                

            }
            //velocityText.text = $"index v : {index_v.magnitude * 100} / middle v : {middle_v.magnitude * 100}";
        }
        else
        {
            targetGO = null;
        }
    }

    private void FixedUpdate()
    {
        if (!GD.thereAreBonesRight) return;

        // velocity 측정

        if (count == 1 && c % 19 == 0)
        {
            c = 0;

            // 검지 끝
            index_0 = GD.skeletonRight.Bones[8].Transform.position;

            // 중지 끝
            middle_0 = GD.skeletonRight.Bones[11].Transform.position;

            bool isValid = index_0 != null && index_1 != null && middle_0 != null && middle_1 != null;
            if (isValid)
            {
                index_v = index_0 - index_1;
                //index_v = index_0;
                middle_v = middle_0 - middle_1;
                //middle_v = middle_0;
            }

            count *= -1;
            
        }
        else if (count == -1 && c % 19 == 0)
        {
            c = 0;

            // 검지 끝
            index_1 = GD.skeletonRight.Bones[8].Transform.position;

            // 중지 끝
            middle_1 = GD.skeletonRight.Bones[11].Transform.position;

            bool isValid = index_0 != null && index_1 != null && middle_0 != null && middle_1 != null;
            if (isValid)
            {
                index_v = index_1 - index_0;
                middle_v = middle_1 - middle_0;
            }

            count *= -1;
        }

        c++;
    }
}
