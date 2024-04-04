using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHand_That : MonoBehaviour, ILeftGesture
{
    [SerializeField] private GestureDetection_Demo GD;
    public string targetName => "That";

    public GameObject targetGO => GD.MyTargetGO;

    public LineRenderer lineRenderer; // 레이를 시각적으로 나타낼 라인 렌더러
    public GameObject objectToInteract; // 상호작용할 오브젝트


    private void Update()
    {
        string currentInterface = GD.Recognize().name;

        if (currentInterface == "That")
        {
            // 검지 중간 관절
            Vector3 middlePos = GD.skeletonLeft.Bones[7].Transform.position;
            // 검지 끝 관절
            Vector3 indexPos = GD.skeletonLeft.Bones[8].Transform.position;

            // 방향 벡터 계산
            Vector3 direction = indexPos - middlePos;

            lineRenderer.SetPosition(0, indexPos);

            // 레이캐스트 발사
            RaycastHit hit;
            bool isHit;
            isHit = Physics.Raycast(middlePos, direction, out hit, Mathf.Infinity, layerMask: LayerMask.GetMask("InteractObj"));

            // 라인 렌더러로 레이를 시각적으로 표현
            lineRenderer.SetPosition(0, indexPos);
            if (isHit)
            {
                // 레이캐스트가 오브젝트에 부딪힌 경우
                lineRenderer.SetPosition(1, hit.transform.position);
                objectToInteract = hit.transform.gameObject;
                GD.targetGO = objectToInteract;
            }
            else
            {
                lineRenderer.SetPosition(1, indexPos + direction * 500);
            }
        }
        else
        {
            lineRenderer.SetPosition(0, Vector3.zero);
            lineRenderer.SetPosition(1, Vector3.zero);
        }
        
        
    }
}
