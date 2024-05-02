using OculusSampleFramework;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RightHand_Crawl : MonoBehaviour
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

    private Vector3 middle_previousVelocity; // ���� �������� �ӵ�
    private Vector3 middle_currentVelocity; // ���� �������� �ӵ�

    private Vector3 middle_previousPosition; // ���� �������� ��ġ
    private Vector3 middle_currentPosition; // ���� �������� ��ġ

    private Vector3 middle_acceleration; // ���ӵ�

    private float deltaTime; // ������ ���� �ð� ����

    void Update()
    {
        currentTargetName = GD.Recognize().name;
        currentInterface = GD.RecognizeRight().name;
        targetGO = GD.targetGO;

        if (currentInterface == "Crawl" && currentTargetName == "HumanAvatar")
        {
            if (targetGO != null && GD.targetName != "We")
            {
                // ����
                if (!GD.thereAreBonesRight) return;

                var humanAvatar = targetGO.GetComponent<LeftHand_HumanAvatar>();
                if (humanAvatar == null) return;

                if (humanAvatar.avatarState != AvatarState.Idle && humanAvatar.avatarState != AvatarState.Run) return;

                // ����, ���� �� ��ġ
                Vector3 indexPos = GD.skeletonRight.Bones[8].Transform.position;
                Vector3 middlePos = GD.skeletonRight.Bones[11].Transform.position;

                // ���� �������� �ӵ��� ��ġ�� ����
                // ����
                index_currentVelocity = (indexPos - index_previousPosition) / Time.deltaTime;
                index_currentPosition = indexPos;
                // ����
                middle_currentVelocity = (middlePos - middle_previousPosition) / Time.deltaTime;
                middle_currentPosition = middlePos;

                // ������ ���� �ð� ����
                deltaTime = Time.deltaTime;

                // ���ӵ� ���
                // ����
                index_acceleration = (index_currentVelocity - index_previousVelocity) / deltaTime;
                // ����
                middle_acceleration = (middle_currentVelocity - middle_previousVelocity) / deltaTime;

                //velocityText.text = $"Acceleration : {index_acceleration.magnitude}, {middle_acceleration.magnitude}";

                // ���ӵ� ���� ����Ͽ� ���ϴ� �۾� ����
                float maxAccel = Mathf.Max(index_acceleration.magnitude, middle_acceleration.magnitude);
                float speed = maxAccel * 0.01f;

                if (speed > 0.05f)
                {
                    targetGO.GetComponent<Animator>().SetBool("Crawl", true);

                    if (GD.targetName == LeftHandTargets.HumanAvatar.ToString()
                        && targetGO.TryGetComponent(out StarterAssetsInputs input))
                    {
                        input.sprint = (speed > 0.35f) ? true : false;

                        //input.move = Vector2.zero;

                        // ȸ��
                        var leftHand = HandsManager.Instance.LeftHand;
                        Vector3 leftHandRotation = leftHand.transform.rotation.eulerAngles;
                        float t = 40;

                        bool isLeft = (leftHandRotation.x > 360 - t || leftHandRotation.x < t)
                                    && (leftHandRotation.z > 360 - t || leftHandRotation.z < t);
                        bool isRight = (leftHandRotation.x > 360 - t || leftHandRotation.x < t)
                                    && (leftHandRotation.z > 180 - t || leftHandRotation.z < 180 + t);

                        if (isLeft) // ����
                        {
                            input.move += Vector2.left;
                        }
                        else if (isRight) // ������
                        {
                            input.move += Vector2.right;
                        }
                        else
                        {
                            if (leftHandRotation.x > leftHandRotation.z)
                                input.move += Vector2.up;
                            else
                                input.move += Vector2.down;

                        }

                        //rotText.text = $"rot : {leftHandRotation}\n" +
                        //    $"isLeft : {isLeft}, isRight : {isRight}";
                    }
                    else
                        targetGO.transform.position += Vector3.forward * speed * Time.deltaTime;


                }
                else
                {
                    if (targetGO.TryGetComponent(out StarterAssetsInputs input))
                    {
                        input.sprint = false;
                        input.move = Vector2.zero;
                    }

                    targetGO.GetComponent<Animator>().SetBool("Crawl", false);
                }


                // ���� �������� �ӵ��� ��ġ�� ���� ������ ����
                // ����
                index_previousVelocity = index_currentVelocity;
                index_previousPosition = index_currentPosition;
                // ����
                middle_previousVelocity = middle_currentVelocity;
                middle_previousPosition = middle_currentPosition;

            }
        }
        else
        {
            if (targetGO != null
                && GD.targetName == LeftHandTargets.HumanAvatar.ToString()
                && targetGO.TryGetComponent(out StarterAssetsInputs input))
            {
                input.sprint = false;
                input.move = Vector2.zero;

                targetGO.GetComponent<Animator>().SetBool("Crawl", false);
            }
            targetGO = null;
        }
    }

}