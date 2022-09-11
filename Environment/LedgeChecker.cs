using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeChecker : MonoBehaviour
{
    public Vector3 LedgeCalibration = new Vector3();
    PlayerStateMachine Ctx;

    public LedgeCollider ColliderTop;
    public LedgeCollider ColliderBottom;

    public float _handPosY;

    private void Start()
    {
        Ctx = GetComponentInParent<PlayerStateMachine>();
        Ctx.IsGrabbingLedge = false;
    }

    private void FixedUpdate()
    {
        if (Ctx.CharacterController.isGrounded)
        {
            foreach (GameObject obj in ColliderBottom.CollidedObjects)
            {
                if (ColliderBottom.CollidedObjects.Contains(obj))
                {
                    ExitEdge();
                    // if (OffsetPosition(obj))
                    // {
                    //     break;
                    // }
                }
            }
            
            if (ColliderBottom.CollidedObjects.Count == 0)
            {
                if (!Ctx.IsMovementPressed && !Ctx.IsRunPressed)
                {
                    EnterEdge();
                }
                else if ((Ctx.IsMovementPressed && !Ctx.IsRunPressed) || (Ctx.IsMovementPressed && Ctx.IsRunPressed) || (Ctx.IsJumpPressed))
                {
                    ExitEdge();
                }
                else if (!Ctx.CharacterController.isGrounded || Ctx.IsJumpPressed)
                {
                    ExitEdge();
                }
            }
            // foreach (GameObject obj in ColliderBottom.CollidedObjects)
            // {
            //     if (!ColliderTop.CollidedObjects.Contains(obj))
            //     {
            //         if (OffsetPosition(obj))
            //         {
            //             break;
            //         }
            //     }
            //     else
            //     {
            //         // Ctx.IsGrabbingLedge = false;
            //     }
            // }
        }
        // else
        // {
        //     // Ctx.IsGrabbingLedge = false;
        // }

        // if (ColliderBottom.CollidedObjects.Count == 0)
        // {
        //     // Ctx.IsGrabbingLedge = false;
        // }
    }

    bool OffsetPosition(GameObject platform)
    {
        BoxCollider boxCollider = platform.GetComponent<BoxCollider>();

        if (boxCollider == null)
        {
            return false;
        }

        if (Ctx.IsGrabbingLedge)
        {
            return false;
        }

        // Ctx.IsGrabbingLedge = true;

        return true;
    }

    private void EnterEdge()
    {
        Ctx.Animator.SetBool(Ctx.IsEdgeBalancingHash, true);
    }

    private void ExitEdge()
    {
        Ctx.Animator.SetBool(Ctx.IsEdgeBalancingHash, false);
    }
}
