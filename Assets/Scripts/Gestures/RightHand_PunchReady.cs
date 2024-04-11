using OculusSampleFramework;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

public enum PunchState { PunchReady, Punch }
public class RightHand_PunchReady : MonoBehaviour
{
    [SerializeField] private GestureDetection_Demo GD;

    public GameObject targetGO;

    private string currentTargetName;
    private string currentInterface;

    private Vector3 index_previousVelocity; // ���� �������� �ӵ�
    private Vector3 index_currentVelocity; // ���� �������� �ӵ�

    private Vector3 index_previousPosition; // ���� �������� ��ġ
    private Vector3 index_currentPosition; // ���� �������� ��ġ

    private Vector3 index_acceleration; // ���ӵ�

    private float deltaTime; // ������ ���� �ð� ����

    private PunchState punchState = PunchState.PunchReady;

    Animator anim;

    private void Awake()
    {
        if (targetGO != null)
            anim = targetGO.GetComponent<Animator>();
    }

    void Update()
    {
        currentTargetName = GD.Recognize().name;
        currentInterface = GD.RecognizeRight().name;
        targetGO = GD.targetGO;

        if (currentInterface == "PunchReady" && currentTargetName == "HumanAvatar")
        {
            if (punchState == PunchState.PunchReady)
            {
                Debug.Log("0");
                if (targetGO != null && GD.targetName != "We")
                {
                    Debug.Log("1");
                    // ����
                    //@!
                    //if (!GD.thereAreBonesRight) return;
                    Debug.Log("2");
                    if (anim == null) anim = targetGO.GetComponent<Animator>();
                    if (anim != null && !anim.GetBool("PunchReady"))
                    {
                        anim.SetBool("PunchReady", true);
                        anim.SetBool("Punch", false);
                    }

                    // ���ӵ� ���� ����Ͽ� ���ϴ� �۾� ����
                    float speed = GetFingerSpeed();
                    Debug.Log($"PunchReadyState Speed : {speed}");
                    if (speed > 0.65f)
                    {
                        if (GD.targetName == LeftHandTargets.HumanAvatar.ToString()
                            && targetGO.TryGetComponent(out StarterAssetsInputs input))
                        {
                            if (anim != null)
                            {
                                // punch anim
                                anim.SetBool("Punch", true);

                                //@!
                                //if (index_previousVelocity.sqrMagnitude != 0)
                                {
                                    GameObject punchTarget = FindAndLookAtEachOther();
                                    if (punchTarget != null)
                                    {
                                        Animator punchTargetAnim = punchTarget.GetComponent<Animator>();
                                        punchTargetAnim.SetTrigger("Stumble");
                                    }
                                }
                                

                                punchState = PunchState.Punch;

                                Debug.Log($"Transition: PunchReady -> Punch");
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
            else if (punchState == PunchState.Punch)
            {
                float speed = GetFingerSpeed();
                Debug.Log($"PunchState Speed : {speed}");
                if (speed < 0.35f)
                {
                    if (anim != null && anim.GetBool("Punch"))
                    {
                        anim.SetBool("Punch", false);
                    }
                    punchState = PunchState.PunchReady;
                    Debug.Log($"Transition: Punch -> PunchReady");
                }

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

    private float GetFingerSpeed()
    {
        //@!
        float _speed = 0;
        if (Input.GetKeyDown(KeyCode.Alpha9))
            _speed = 1;
        return _speed;

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

        return speed;
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
