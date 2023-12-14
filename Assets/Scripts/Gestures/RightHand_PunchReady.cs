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

    private Vector3 index_previousVelocity; // 이전 프레임의 속도
    private Vector3 index_currentVelocity; // 현재 프레임의 속도

    private Vector3 index_previousPosition; // 이전 프레임의 위치
    private Vector3 index_currentPosition; // 현재 프레임의 위치

    private Vector3 index_acceleration; // 가속도

    private float deltaTime; // 프레임 간의 시간 차이

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
                // 구현
                if (!GD.thereAreBonesRight) return;

                // 중지 시작 위치
                Vector3 middlePos = GD.skeletonRight.Bones[9].Transform.position;

                // 현재 프레임의 속도와 위치를 갱신
                index_currentVelocity = (middlePos - index_previousPosition) / Time.deltaTime;
                index_currentPosition = middlePos;

                // 프레임 간의 시간 차이
                deltaTime = Time.deltaTime;

                // 가속도 계산
                // 검지
                index_acceleration = (index_currentVelocity - index_previousVelocity) / deltaTime;

                // 가속도 값을 사용하여 원하는 작업 수행
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


                // 이전 프레임의 속도와 위치를 현재 값으로 갱신
                // 검지
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
