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
        Debug.Log("Hello from JUMP");
        InitializeSubState();
        HandleJump();
    }


    public override void UpdateState()
    {
        if (Ctx.IsGrabbingLedge)
        {
            DisableGravity();
        }
        else
        {
            HandleGravity();
        }
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        if (Ctx.IsJumpPressed)
        {
            Ctx.RequireNewJumpPress = true;
        }
        Ctx.Animator.SetBool(Ctx.IsJumpingHash, false);
        // Ctx.Animator.ResetTrigger(Ctx.TriggerJumpHash);
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.IsFalling)
        {
            SwitchState(Factory.Fall());
        }

        if (Ctx.CharacterController.isGrounded)
        {
            SwitchState(Factory.Grounded());
        }

        if (Ctx.CanClimbLedge && Ctx.IsClimbPressed)
        {
            SwitchState(Factory.ClimbLedge());
        }

        if (Ctx.CanClimbObject && Ctx.IsClimbPressed)
        {
            SwitchState(Factory.ClimbObject());
        }
    }

    public override void InitializeSubState()
    {
        if (!Ctx.IsMovementPressed)
        {
            SetSubState(Factory.Idle());
        } 
        else if (Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SetSubState(Factory.Walk());
        }
        else if (Ctx.IsMovementPressed && Ctx.IsRunPressed)
        {
            SetSubState(Factory.Run());
        }
    }

    private void HandleJump()
    {
        Ctx.Animator.CrossFadeInFixedTime(JumpStartAirborneHash, CrossFadeDuration);
        Ctx.IsJumping = true;
        ApplyJumpVelocity();
    }

    public void HandleGravity()
    {
        // bool isFalling = Ctx.CurrentMovementY < Ctx.Zero || !Ctx.IsJumpPressed;
        // float fallMultiplier = 2.0f;

        // if (isFalling)
        // {
        //     Ctx.AppliedMovementY = Mathf.Max((Ctx.CurrentMovementY * Time.deltaTime) + ((Ctx.Gravity * fallMultiplier * 0.5f) * (Time.deltaTime * Time.deltaTime)), -20.0f);
        //     Ctx.CurrentMovementY += (Ctx.Gravity * fallMultiplier * Time.deltaTime); 
        //     Ctx.IsFalling = true;
        // } 
        // else
        // {
        //     Ctx.AppliedMovementY = (Ctx.CurrentMovementY * Time.deltaTime) + ((Ctx.Gravity * 0.5f) * (Time.deltaTime * Time.deltaTime));
        //     Ctx.CurrentMovementY += (Ctx.Gravity * Time.deltaTime); 
        //     Ctx.IsFalling = true;
        // }

        bool isFalling = Ctx.CurrentMovementY <= Ctx.Zero || !Ctx.IsJumpPressed;
        float fallMultiplier = 2.0f;

        if (isFalling)
        {
            float previousYVelocity = Ctx.CurrentMovementY;
            Ctx.CurrentMovementY = Ctx.CurrentMovementY + (Ctx.Gravity * fallMultiplier * Time.deltaTime);
            Ctx.AppliedMovementY = Mathf.Max((previousYVelocity + Ctx.CurrentMovementY) * 0.5f, -20.0f);
            Ctx.IsFalling = true;
        } 
        else
        {
            float previousYVelocity = Ctx.CurrentMovementY;
            Ctx.CurrentMovementY = Ctx.CurrentMovementY + (Ctx.Gravity * Time.deltaTime);
            Ctx.AppliedMovementY = (previousYVelocity + Ctx.CurrentMovementY) * 0.5f;
            // Ctx.IsFalling = true;
        }
    }

    public void DisableGravity()
    {
        Ctx.CurrentMovement = Ctx.CurrentMovement * Ctx.Zero;
        Ctx.AppliedMovement = Ctx.CurrentMovement;
    }

    private void ApplyJumpVelocity()
    {
        Ctx.CurrentMovementY = Ctx.InitialJumpVelocity;
        Ctx.AppliedMovementY = Ctx.InitialJumpVelocity;
    }
}
