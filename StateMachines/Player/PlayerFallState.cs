using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerBaseState, IRootState
{
    private readonly int FreeFallHash = Animator.StringToHash("FreeFall");
    private readonly int JumpLandGroundedHash = Animator.StringToHash("JumpLandGrounded");
    private readonly int JumpLandAirborneHash = Animator.StringToHash("JumpLandAirborne");
    private Vector3 momentum;

    private const float CrossFadeDuration = 0.05f;

    public PlayerFallState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base (currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        // Ctx.Animator.CrossFadeInFixedTime(FreeFallHash, CrossFadeDuration);
        Ctx.Animator.SetBool(Ctx.IsFallingHash, true);
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
        Ctx.Animator.SetBool(Ctx.IsFallingHash, false);
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.CharacterController.isGrounded)
        {
            Ctx.Animator.CrossFadeInFixedTime(JumpLandGroundedHash, CrossFadeDuration);
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
            Ctx.AppliedMovementX = 0;
            Ctx.AppliedMovementZ = 0;
            SetSubState(Factory.Walk());
        } 
        else 
        {
            Ctx.AppliedMovementX = 0;
            Ctx.AppliedMovementZ = 0;
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
}
