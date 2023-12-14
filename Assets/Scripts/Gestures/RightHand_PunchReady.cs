using OculusSampleFramework;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightHand_PunchReady : MonoBehaviour
{
    [SerializeField] private GestureDetection_Demo GD;

    public GameObject targetGO;

    private string currentInterface;

    private Vector3 index_previousVelocity; // ���� �������� �ӵ�
    private Vector3 index_currentVelocity; // ���� �������� �ӵ�

    private Vector3 index_previousPosition; // ���� �������� ��ġ
    private Vector3 index_currentPosition; // ���� �������� ��ġ

    private Vector3 index_acceleration; // ���ӵ�

    private float deltaTime; // ������ ���� �ð� ����

    Animator anim;

    void Update()
    {
        currentInterface = GD.RecognizeRight().name;
        targetGO = GD.targetGO;

        if (currentInterface == "PunchReady")
        {

            anim = targetGO.GetComponent<Animator>();
            anim.SetBool("PunchReady", true);

            if (targetGO != null && GD.targetName != "We")
            {
                // ����
                if (!GD.thereAreBonesRight) return;

                // ���� ���� ��ġ
                Vector3 middlePos = GD.skeletonRight.Bones[9].Transform.position;

                // ���� �������� �ӵ��� ��ġ�� ����
                index_currentVelocity = (middlePos - index_previousPosition) / Time.deltaTime;
                index_currentPosition = middlePos;

                // ������ ���� �ð� ����
                deltaTime = Time.deltaTime;

                // ���ӵ� ���
                // ����
                index_acceleration = (index_currentVelocity - index_previousVelocity) / deltaTime;

                // ���ӵ� ���� ����Ͽ� ���ϴ� �۾� ����
                float speed = index_acceleration.magnitude * 0.01f;

                if (speed > 0.05f)
                {
                    if (GD.targetName == LeftHandTargets.HumanAvatar.ToString()
                        && targetGO.TryGetComponent(out StarterAssetsInputs input))
                    {
                        anim.SetBool("Punch", true);
                    }

                }
                else
                {
                    anim.SetBool("Punch", false);
                }


                // ���� �������� �ӵ��� ��ġ�� ���� ������ ����
                // ����
                index_previousVelocity = index_currentVelocity;
                index_previousPosition = index_currentPosition;

            }
        }
        else
        {
            if (targetGO != null
                && GD.targetName == LeftHandTargets.HumanAvatar.ToString()
                && targetGO.TryGetComponent(out StarterAssetsInputs input))
            {
                anim = targetGO.GetComponent<Animator>();
                anim.SetBool("PunchReady", false);
            }
            targetGO = null;
        }
    }
}
