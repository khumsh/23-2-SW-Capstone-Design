using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightHand_Pitching : MonoBehaviour
{
    [SerializeField] private GestureDetection_Demo GD;

    public GameObject targetGO;

    private string currentTargetName;
    private string currentInterface;

    Animator anim;

    private void Update()
    {
        currentTargetName = GD.Recognize().name;
        currentInterface = GD.RecognizeRight().name;
        targetGO = GD.targetGO;

        if (currentInterface == "Pitching" && currentTargetName == "HumanAvatar")
        {
            if (targetGO != null)
            {
                var humanAvatar = targetGO.GetComponent<LeftHand_HumanAvatar>();
                if (humanAvatar == null) return;

                GameObject socketObj = humanAvatar.socketObject;
                if (socketObj == null) return;

                if (anim == null)
                    InitAnim();

                if (anim != null && !anim.GetBool("Pitching"))
                {
                    anim.SetBool("Pitching", true);
                    StartCoroutine(AnimFalseCo());

                    humanAvatar.avatarState = AvatarState.Pitching;
                }
            }
        }
    }

    void InitAnim()
    {
        anim = targetGO.GetComponent<Animator>();
        AnimationEvtHandler animEvtHandler = targetGO.GetComponent<AnimationEvtHandler>();
        if (animEvtHandler != null)
        {
            animEvtHandler.OnAnimEvt += () =>
            {
                LeftHand_HumanAvatar humanAvatar = targetGO.GetComponent<LeftHand_HumanAvatar>();
                if (humanAvatar.socketObject != null)
                {
                    GameObject go = humanAvatar.socketObject;
                    go.transform.SetParent(null);
                    Rigidbody rb = humanAvatar.socketObject.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.useGravity = true;
                        rb.freezeRotation = false;
                        rb.AddForce(Vector3.forward * 1000);
                        rb.constraints = RigidbodyConstraints.None;
                    }
                }

                anim.SetBool("Pitching", false);

                humanAvatar.avatarState = AvatarState.Idle;
            };
        }
    }

    IEnumerator AnimFalseCo()
    {
        yield return new WaitForSeconds(0.25f);
        anim.SetBool("Pitching", false);

    }
}
