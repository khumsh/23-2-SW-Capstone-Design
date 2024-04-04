using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHand_That : MonoBehaviour, ILeftGesture
{
    [SerializeField] private GestureDetection_Demo GD;
    public string targetName => "That";

    public GameObject targetGO => GD.MyTargetGO;

    public LineRenderer lineRenderer; // ���̸� �ð������� ��Ÿ�� ���� ������
    public GameObject objectToInteract; // ��ȣ�ۿ��� ������Ʈ


    private void Update()
    {
        string currentInterface = GD.Recognize().name;

        if (currentInterface == "That")
        {
            // ���� �߰� ����
            Vector3 middlePos = GD.skeletonLeft.Bones[7].Transform.position;
            // ���� �� ����
            Vector3 indexPos = GD.skeletonLeft.Bones[8].Transform.position;

            // ���� ���� ���
            Vector3 direction = indexPos - middlePos;

            lineRenderer.SetPosition(0, indexPos);

            // ����ĳ��Ʈ �߻�
            RaycastHit hit;
            bool isHit;
            isHit = Physics.Raycast(middlePos, direction, out hit, Mathf.Infinity, layerMask: LayerMask.GetMask("InteractObj"));

            // ���� �������� ���̸� �ð������� ǥ��
            lineRenderer.SetPosition(0, indexPos);
            if (isHit)
            {
                // ����ĳ��Ʈ�� ������Ʈ�� �ε��� ���
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
