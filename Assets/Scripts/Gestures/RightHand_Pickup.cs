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
                if (anim == null)
                    InitAnim();

                if (anim != null)
                {
                    anim.SetBool("Pickup", true);
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
                        hitGO.transform.localPosition = Vector3.zero;
                        Rigidbody rb = hitGO.GetComponent<Rigidbody>();
                        if (rb != null) 
                        {
                            rb.useGravity = false;
                            rb.velocity = Vector3.zero;
                            rb.rotation = Quaternion.identity;
                            rb.freezeRotation = true;
                        }
                        humanAvatar.socketObject = hitGO;
                    }
                    else
                        Debug.Log("rightHandSocket is null");
                }
                else
                {
                    Debug.Log("hitGO null");
                }

                anim.SetBool("Pickup", false);
            };
        }
    }
}
