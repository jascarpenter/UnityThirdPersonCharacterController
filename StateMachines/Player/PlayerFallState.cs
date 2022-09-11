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
        InitializeSubState();
        Ctx.IsLandEntering = false;
        Debug.Log("Hello from FALL");
        Ctx.Animator.SetBool(Ctx.IsFallingHash, true);
        // Ctx.Animator.SetTrigger(Ctx.TriggerFallHash);
    }

    public override void UpdateState()
    {
        if (Ctx.IsLandEntering)
        {
            Ctx.Animator.SetBool(Ctx.IsLandAnticipatingHash, true);
        }

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
        Ctx.Animator.SetBool(Ctx.IsFallingHash, false);
        Ctx.Animator.SetBool(Ctx.IsLandAnticipatingHash, false);
        Ctx.IsFalling = false;
        Ctx.IsLandEntering = false;
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.CharacterController.isGrounded)
        {
            SwitchState(Factory.Land());
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

    public void HandleGravity()
    {
        // bool isLandEntering = Ctx.CurrentMovementY <= Ctx.HighFallTrigger;

        // Ctx.AppliedMovementY = (Ctx.CurrentMovementY * Time.deltaTime) + ((Ctx.Gravity * 0.5f) * (Time.deltaTime * Time.deltaTime));
        // Ctx.CurrentMovementY += (Ctx.Gravity * Time.deltaTime); 
        // if (isLandEntering) {
        //     Ctx.IsLandEntering = true;
        // }

        bool isFalling = Ctx.CurrentMovementY <= Ctx.Zero || !Ctx.IsJumpPressed;
        bool isLandEntering = Ctx.CurrentMovementY <= Ctx.HighFallTrigger;
        float fallMultiplier = 2.0f;

        if (isFalling)
        {
            float previousYVelocity = Ctx.CurrentMovementY;
            Ctx.CurrentMovementY = Ctx.CurrentMovementY + (Ctx.Gravity * fallMultiplier * Time.deltaTime);
            Ctx.AppliedMovementY = Mathf.Max((previousYVelocity + Ctx.CurrentMovementY) * 0.5f, -20.0f);
            if (isLandEntering) {
                Ctx.IsLandEntering = true;
            }
        } 
        else
        {
            float previousYVelocity = Ctx.CurrentMovementY;
            Ctx.CurrentMovementY = Ctx.CurrentMovementY + (Ctx.Gravity * Time.deltaTime);
            Ctx.AppliedMovementY = (previousYVelocity + Ctx.CurrentMovementY) * 0.5f;
        }
    }

    public void DisableGravity()
    {
        Ctx.CurrentMovement = Ctx.CurrentMovement * Ctx.Zero;
        Ctx.AppliedMovement = Ctx.CurrentMovement;
    }
}
