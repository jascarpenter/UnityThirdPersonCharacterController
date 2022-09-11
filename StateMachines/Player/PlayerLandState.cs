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
        IsRootState = true;
    }

    public override void EnterState()
    {
        InitializeSubState();
        Ctx.IsJumping = false;
        Debug.Log("Hello from LAND");
        // Ctx.Animator.CrossFadeInFixedTime(FreeFallHash, CrossFadeDuration);
    }

    public override void UpdateState()
    {
        InitializeSubState();
        if (Ctx.IsGrabbingLedge)
        {
            DisableGravity();
        }
        else
        {
            HandleGravity();
        }
        // HandleHighFall();
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.IsJumpPressed && !Ctx.RequireNewJumpPress)
        {
            SwitchState(Factory.Jump());
        }
        if (!Ctx.IsJumpPressed && !Ctx.CharacterController.isGrounded)
        {
            SwitchState(Factory.Fall());
        }
        else if (Ctx.CharacterController.isGrounded)
        {
            Ctx.Animator.SetBool(Ctx.IsLandAnticipatingHash, false);
            if (!Ctx.IsClimbTeleporting)
            {
                Ctx.Animator.CrossFadeInFixedTime(JumpLandGroundedHash, CrossFadeDuration);
            }
            Ctx.IsClimbTeleporting = false;
            // Ctx.Animator.SetBool(Ctx.IsLandAnticipatingHash, true);
            // Ctx.Animator.SetBool(Ctx.IsLandingHash, true);
            SwitchState(Factory.Grounded());
        }
        else if (Ctx.CanClimbLedge && Ctx.IsClimbPressed)
        {
            SwitchState(Factory.ClimbLedge());
        }
        else if (Ctx.CanClimbObject && Ctx.IsClimbPressed)
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

    // private void HandleHighFall()
    // {
    //     if ((Ctx.AppliedMovementY >= HighFallTirgger))
    //     {
    //         Ctx.Animator.CrossFadeInFixedTime(JumpLandAirborneHash, CrossFadeDuration);
    //     }
    // }

    public void HandleGravity()
    {
        Ctx.CurrentMovementY = Ctx.Gravity;
        Ctx.AppliedMovementY = Ctx.Gravity;

    }

    public void DisableGravity()
    {
        Ctx.CurrentMovement = Ctx.CurrentMovement * Ctx.Zero;
        Ctx.AppliedMovement = Ctx.CurrentMovement;
    }
}
