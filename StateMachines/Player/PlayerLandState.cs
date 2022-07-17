using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLandState : PlayerBaseState, IRootState
{
    private readonly int FreeFallHash = Animator.StringToHash("FreeFall");
    private readonly int JumpLandGroundedHash = Animator.StringToHash("JumpLandGrounded");
    private readonly int JumpLandAirborneHash = Animator.StringToHash("JumpLandAirborne");
    private Vector3 momentum;

    private const float CrossFadeDuration = 0.05f;

    public PlayerLandState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base (currentContext, playerStateFactory)
    {

    }

    public override void EnterState()
    {
        Debug.Log("I am in FALL state");
        // Ctx.Animator.CrossFadeInFixedTime(FreeFallHash, CrossFadeDuration);
        Ctx.Animator.SetBool(Ctx.IsLandingHash, true);
    }

    public override void UpdateState()
    {
        InitializeSubState();
        HandleGravity();
        // HandleFall();
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Ctx.Animator.SetBool(Ctx.IsLandingHash, false);
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.CharacterController.isGrounded)
        {
            SwitchState(Factory.Grounded());
        }
    }

    public override void InitializeSubState()
    {
        if (!Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SetSubState(Factory.Idle());
        } 
        else if (Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SetSubState(Factory.Walk());
        } 
        else 
        {
            SetSubState(Factory.Run());
        }
    }

    // private void HandleFall()
    // {
    //     if ((Ctx.AppliedMovementY >= -2f))
    //     {
    //         Ctx.Animator.CrossFadeInFixedTime(JumpLandAirborneHash, CrossFadeDuration);
    //     }
    // }

    public void HandleGravity()
    {
        float fallMultiplier = 2.0f;
        
        float previousYVelocity = Ctx.CurrentMovementY;
        Ctx.CurrentMovementY = Ctx.CurrentMovementY + (Ctx.Gravity * fallMultiplier * Time.deltaTime);
        Ctx.AppliedMovementY = Mathf.Max((previousYVelocity + Ctx.CurrentMovementY) * 0.5f, -20.0f);

    }

    // public override void EnterState()
    // {
    //     // Ctx.Animator.SetBool(Ctx.IsLandAnticipatingHash, true);
    // }

    // public override void UpdateState()
    // {
    //     // InitializeSubState();
    //     // HandleGravity();
    //     // CheckSwitchStates();
    // }

    // public override void ExitState()
    // {

    // }

    // public override void CheckSwitchStates()
    // {
    //     // if (Ctx.CharacterController.isGrounded)
    //     // {
    //     //     SwitchState(Factory.Grounded());
    //     // }
    // }

    // public override void InitializeSubState()
    // {

    // }

    // public void HandleGravity()
    // {
    //     // float fallMultiplier = 2.0f;
        
    //     // float previousYVelocity = Ctx.CurrentMovementY;
    //     // Ctx.CurrentMovementY = Ctx.CurrentMovementY + (Ctx.Gravity * fallMultiplier * Time.deltaTime);
    //     // Ctx.AppliedMovementY = Mathf.Max((previousYVelocity + Ctx.CurrentMovementY) * 0.5f, -20.0f);
    // }
}
