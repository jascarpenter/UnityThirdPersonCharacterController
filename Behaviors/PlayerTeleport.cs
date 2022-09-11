using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleport : StateMachineBehaviour
{
    PlayerStateMachine Ctx;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Ctx = animator.GetComponent<PlayerStateMachine>();
        TeleportOnLedge();
        Ctx.IsClimbTeleporting = false;
        Ctx.IsGrabbingLedge = false;
        Ctx.IsClimbingObject = false;
        HandleGravity();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}

    // public void OffsetOnLedge()
    // {
    //     Debug.Log(Ctx._ledgeChecker.transform.position);
    //     Ctx.transform.parent = Ctx._ledgeChecker.transform;
    //     Ctx.transform.localPosition = Ctx._ledgeChecker.transform.position;
    // }

    public void TeleportOnLedge()
    {
        Vector3 offsetLedgePos = Ctx.Detection.ledgePos + (Ctx.transform.forward * 0.4f);

        Ctx.transform.position = offsetLedgePos;
    }

    public void HandleGravity()
    {
        Ctx.CurrentMovementY = Ctx.Gravity;
        Ctx.AppliedMovementY = Ctx.Gravity;
    }
}
