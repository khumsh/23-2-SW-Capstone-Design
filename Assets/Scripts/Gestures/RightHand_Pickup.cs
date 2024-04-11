using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightHand_Pickup : MonoBehaviour
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

        if (currentInterface == "Pickup" && currentTargetName == "HumanAvatar")
        {
            if (targetGO != null) 
            {
                var humanAvatar = targetGO.GetComponent<LeftHand_HumanAvatar>();
                if (humanAvatar == null) return;


                GameObject socketObj = humanAvatar.socketObject;
                if (socketObj != null) return;

                if (anim == null)
                    InitAnim();

                if (anim != null && !anim.GetBool("Pickup"))
                {
                    anim.SetBool("Pickup", true);
                    StartCoroutine(AnimFalseCo());

                    humanAvatar.avatarState = AvatarState.Pickup;
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
                GameObject hitGO = null;
                Collider[] hits = Physics.OverlapSphere(targetGO.transform.position, 3, LayerMask.GetMask("InteractObj"));
                foreach (Collider hit in hits)
                {
                    Debug.Log($"hitName : {hit.name}");
                    if (hit.CompareTag("ball"))
                    {
                        hitGO = hit.gameObject;
                    }
                }

                if (hitGO != null)
                {
                    LeftHand_HumanAvatar humanAvatar = targetGO.GetComponent<LeftHand_HumanAvatar>();
                    Transform rightHandSocket = humanAvatar.RightHandSocket;
                    if (rightHandSocket != null && humanAvatar.socketObject == null)
                    {
                        hitGO.transform.SetParent(rightHandSocket);
                        Rigidbody rb = hitGO.GetComponent<Rigidbody>();
                        if (rb != null) 
                        {
                            rb.useGravity = false;
                            rb.velocity = Vector3.zero;
                            rb.rotation = Quaternion.identity;
                            rb.freezeRotation = true;
                            rb.constraints = RigidbodyConstraints.FreezeAll;
                        }
                        humanAvatar.socketObject = hitGO;
                        hitGO.transform.localPosition = Vector3.zero;
                    }
                    else
                        Debug.LogError("rightHandSocket is null");

                    humanAvatar.avatarState = AvatarState.Idle;
                }
                else
                {
                    Debug.LogWarning("hitGO null");
                }

                anim.SetBool("Pickup", false);
            };
        }
    }

    IEnumerator AnimFalseCo()
    {
        yield return new WaitForSeconds(0.25f);
        anim.SetBool("Pickup", false);

    }
}
