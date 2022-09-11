using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState, IRootState
{
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base (currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Debug.Log("Hello from GROUNDED");
        InitializeSubState();
        HandleGravity();
        ResetAnimBools();
    }

    public override void UpdateState()
    {
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
        else if (!Ctx.CharacterController.isGrounded)
        {
            SwitchState(Factory.Fall());
        }
        else if (Ctx.CanClimbObject && Ctx.IsClimbPressed)
        {
            SwitchState(Factory.ClimbObject());
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
        else if (Ctx.IsMovementPressed && Ctx.IsRunPressed)
        {
            SetSubState(Factory.Run());
        }
    }

    public void HandleGravity()
    {
        Ctx.CurrentMovementY = Ctx.Gravity;
        Ctx.AppliedMovementY = Ctx.Gravity;
    }

    private void ResetAnimBools()
    {
        foreach(AnimatorControllerParameter parameter in Ctx.Animator.parameters)
        {            
            Ctx.Animator.SetBool(parameter.name, false);            
        }

        if (Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            Ctx.Animator.SetBool(Ctx.IsWalkingHash, true);
        }

        if (Ctx.IsMovementPressed && Ctx.IsRunPressed)
        {
            Ctx.Animator.SetBool(Ctx.IsWalkingHash, true);
            Ctx.Animator.SetBool(Ctx.IsRunningHash, true);
        }
    }
}
