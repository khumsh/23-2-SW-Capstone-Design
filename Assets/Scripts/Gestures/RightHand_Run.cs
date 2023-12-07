using OculusSampleFramework;
using StarterAssets;
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

    private Vector3 index_previousVelocity; // 이전 프레임의 속도
    private Vector3 index_currentVelocity; // 현재 프레임의 속도

    private Vector3 index_previousPosition; // 이전 프레임의 위치
    private Vector3 index_currentPosition; // 현재 프레임의 위치

    private Vector3 index_acceleration; // 가속도

    private Vector3 middle_previousVelocity; // 이전 프레임의 속도
    private Vector3 middle_currentVelocity; // 현재 프레임의 속도

    private Vector3 middle_previousPosition; // 이전 프레임의 위치
    private Vector3 middle_currentPosition; // 현재 프레임의 위치

    private Vector3 middle_acceleration; // 가속도

    private float deltaTime; // 프레임 간의 시간 차이

    public TextMeshPro rotText;
    void Update()
    {
        currentInterface = GD.RecognizeRight().name;
        targetGO = GD.targetGO;

        if (currentInterface == "Run")
        {
            if (targetGO != null && GD.targetName != "We")
            {
                // 구현
                if (!GD.thereAreBonesRight) return;

                // 검지, 중지 끝 위치
                Vector3 indexPos = GD.skeletonRight.Bones[8].Transform.position;
                Vector3 middlePos = GD.skeletonRight.Bones[11].Transform.position;

                // 현재 프레임의 속도와 위치를 갱신
                // 검지
                index_currentVelocity = (indexPos - index_previousPosition) / Time.deltaTime;
                index_currentPosition = indexPos;
                // 중지
                middle_currentVelocity = (middlePos - middle_previousPosition) / Time.deltaTime;
                middle_currentPosition = middlePos;

                // 프레임 간의 시간 차이
                deltaTime = Time.deltaTime;

                // 가속도 계산
                // 검지
                index_acceleration = (index_currentVelocity - index_previousVelocity) / deltaTime;
                // 중지
                middle_acceleration = (middle_currentVelocity - middle_previousVelocity) / deltaTime;

                velocityText.text = $"Acceleration : {index_acceleration.magnitude}, {middle_acceleration.magnitude}";

                // 가속도 값을 사용하여 원하는 작업 수행
                float maxAccel = Mathf.Max(index_acceleration.magnitude, middle_acceleration.magnitude);
                float speed = maxAccel * 0.01f;

                if (speed > 0.05f)
                {
                    if (GD.targetName == LeftHandTargets.HumanAvatar.ToString() 
                        && targetGO.TryGetComponent(out StarterAssetsInputs input))
                    {
                        input.sprint = (speed > 0.35f) ? true : false;

                        //input.move = Vector2.zero;

                        // 회전
                        var leftHand = HandsManager.Instance.LeftHand;
                        Vector3 leftHandRotation = leftHand.transform.rotation.eulerAngles;
                        float t = 40;

                        bool isLeft = (leftHandRotation.x > 360 - t || leftHandRotation.x < t)
                                    && (leftHandRotation.z > 360 - t || leftHandRotation.z < t);
                        bool isRight = (leftHandRotation.x > 360 - t || leftHandRotation.x < t)
                                    && (leftHandRotation.z > 180 - t || leftHandRotation.z < 180 + t);

                        if (isLeft) // 왼쪽
                        {
                            input.move += Vector2.left;
                        }
                        else if (isRight) // 오른쪽
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

                        rotText.text = $"rot : {leftHandRotation}\n" +
                            $"isLeft : {isLeft}, isRight : {isRight}";
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
                }
                    

                // 이전 프레임의 속도와 위치를 현재 값으로 갱신
                // 검지
                index_previousVelocity = index_currentVelocity;
                index_previousPosition = index_currentPosition;
                // 중지
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
            }
            targetGO = null;
        }
    }


}
