using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState, IRootState
{
    private readonly int JumpStartGroundedHash = Animator.StringToHash("JumpStartGrounded");
    private readonly int JumpStartAirborneHash = Animator.StringToHash("JumpStartAirborne");
    private readonly int JumpLandAirborneHash = Animator.StringToHash("JumpLandAirborne");
    private readonly int FreeFallHash = Animator.StringToHash("FreeFall");

    private Vector3 momentum;
    
    private const float CrossFadeDuration = 0.1f;

    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base (currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        InitializeSubState();
        HandleJump();
    }


    public override void UpdateState()
    {
        HandleGravity();
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        // Ctx.Animator.SetBool(Ctx.IsJumpAnticipatingHash, false);
        // Ctx.Animator.SetBool(Ctx.IsJumpingHash, false);
        if (Ctx.IsJumpPressed)
        {
            Ctx.RequireNewJumpPress = true;
        }
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.CharacterController.isGrounded)
        {
            SwitchState(Factory.Fall());
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

    private void HandleJump()
    {
        Ctx.Animator.CrossFadeInFixedTime(JumpStartAirborneHash, CrossFadeDuration);
        // Ctx.IsJumping = true;
        // if (Ctx.Animator.GetCurrentAnimatorStateInfo(0).fullPathHash == JumpStartGroundedHash)
        // {
            
        // }
        ApplyJumpVelocity();
    }

    public void HandleGravity()
    {
        bool isFalling = Ctx.CurrentMovementY <= 0.0f || !Ctx.IsJumpPressed;
        bool isLanding = Ctx.CurrentMovementY <= -3.0f;
        float fallMultiplier = 2.0f;

        if (isFalling)
        {
            float previousYVelocity = Ctx.CurrentMovementY;
            Ctx.CurrentMovementY = Ctx.CurrentMovementY + (Ctx.Gravity * fallMultiplier * Time.deltaTime);
            Ctx.AppliedMovementY = Mathf.Max((previousYVelocity + Ctx.CurrentMovementY) * 0.5f, -20.0f);
            if (isLanding) {
                Ctx.Animator.SetBool(Ctx.IsLandAnticipatingHash, true);
            }
        } 
        else
        {
            float previousYVelocity = Ctx.CurrentMovementY;
            Ctx.CurrentMovementY = Ctx.CurrentMovementY + (Ctx.Gravity * Time.deltaTime);
            Ctx.AppliedMovementY = (previousYVelocity + Ctx.CurrentMovementY) * 0.5f;
            Ctx.Animator.SetBool(Ctx.IsLandAnticipatingHash, false);
        }
    }

    private void ApplyJumpVelocity()
    {
        Ctx.CurrentMovementY = Ctx.InitialJumpVelocity;
        Ctx.AppliedMovementY = Ctx.InitialJumpVelocity;
    }
}
