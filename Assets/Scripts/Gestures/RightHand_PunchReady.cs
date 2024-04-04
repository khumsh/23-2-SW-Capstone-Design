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
            if (targetGO != null && GD.targetName != "We")
            {
                // ����
                if (!GD.thereAreBonesRight) return;

                anim = targetGO.GetComponent<Animator>();
                if (anim != null)
                    anim.SetBool("PunchReady", true);

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

                if (speed > 0.5f)
                {
                    if (GD.targetName == LeftHandTargets.HumanAvatar.ToString()
                        && targetGO.TryGetComponent(out StarterAssetsInputs input))
                    {
                        if (anim != null)
                        {
                            // punch anim
                            anim.SetTrigger("Punch");

                            GameObject punchTarget = FindAndLookAtEachOther();

                            if (punchTarget != null)
                            {
                                Animator punchTargetAnim = punchTarget.GetComponent<Animator>();
                                punchTargetAnim.SetTrigger("Stumble");
                            }
                        }
                    }
                }
                else
                {
                    if (anim != null)
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
                if (anim != null)
                    anim.SetBool("PunchReady", false);
            }
            targetGO = null;
        }
    }

    private GameObject FindAndLookAtEachOther()
    {
        Collider[] hits = Physics.OverlapSphere(targetGO.transform.position, 3, LayerMask.GetMask("InteractObj"));

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player") && hit.gameObject != targetGO)
            {
                // Ÿ�� ������Ʈ�� �ٶ󺸰� �մϴ�.
                Vector3 directionToTarget = hit.transform.position - transform.position;
                directionToTarget.y = 0; // Y�� ȸ���� ������� ����
                Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

                // �ڽ��� Ÿ�� ������Ʈ�� �ٶ󺸰� �մϴ�.
                Vector3 directionToSelf = transform.position - hit.transform.position;
                directionToSelf.y = 0; // Y�� ȸ���� ������� ����
                Quaternion lookRotationToSelf = Quaternion.LookRotation(directionToSelf);
                hit.transform.rotation = Quaternion.Slerp(hit.transform.rotation, lookRotationToSelf, Time.deltaTime * 5f);

                return hit.gameObject;
            }
            
        }

        return null;
    }
}
