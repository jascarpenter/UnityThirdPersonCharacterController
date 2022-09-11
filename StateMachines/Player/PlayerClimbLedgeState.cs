using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbLedgeState : PlayerBaseState, IRootState
{
    private Vector3 momentum;
    private float rotationSpeed = 0.7f;
    private Quaternion rotationAdjustment;
    private Quaternion correctRotation;
    private const float CrossFadeDuration = 0.1f;

    public PlayerClimbLedgeState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base (currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Ctx.IsJumping = false;
        Ctx.RequireNewJumpPress = false;
        Debug.Log("Hello from CLIMB LEDGE");
        InitializeSubState();
        DisableGravity();
        Ctx.GrounderFBBIK.enabled = false;
    }


    public override void UpdateState()
    {
        InitializeSubState();
        if (Ctx.IsGrabbingLedge || Ctx.IsClimbTeleporting)
        {
            DisableGravity();
        }
        else
        {
            // HandleGravity();
        }
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        DisableGravity();
        // HandleGravity();
        Ctx.Animator.SetBool(Ctx.IsGrabbingLedgeHash, false);
        Ctx.Animator.SetBool(Ctx.IsClimbingLedgeHash, false);
        Ctx.Animator.SetBool(Ctx.IsClimbingTeleportHash, false);
        Ctx.IsFalling = false;
        Ctx.IsClimbTeleporting = false;
        Ctx.IsGrabbingLedge = false;
        Ctx.GrounderFBBIK.enabled = true;
    }

    public override void CheckSwitchStates()
    {
        if (!Ctx.CharacterController.isGrounded && !Ctx.IsClimbTeleporting && !Ctx.IsClimbPressed)
        {
            SwitchState(Factory.Fall());
        }
        if (Ctx.CharacterController.isGrounded)
        {
            SwitchState(Factory.Grounded());
        }
    }

    public override void InitializeSubState()
    {
        if (Ctx.CanClimbLedge && Ctx.IsClimbPressed)
        {
            ClimbLedgeEnter();
        }

        if (Ctx.IsGrabbingLedge)
        {
            if (Ctx.IsInteractPressed || Ctx.IsMovementPressed)
            {
                ClimbLedgeExit();
            }
        }

        if (Ctx.CharacterController.isGrounded)
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
    }

    private void ClimbLedgeEnter() 
    {
        Vector3 offsetLedgePos = Ctx.Detection.ledgePos;

        offsetLedgePos.y = Ctx.Detection.ledgePos.y - Ctx.CharacterController.height;
        Ctx.transform.position = Vector3.Lerp(Ctx.transform.position, offsetLedgePos - (Ctx.transform.forward * 0.4f), Time.deltaTime * 10f);

        Vector3 targetDir = -Ctx.Detection.wallNormal;
        
        targetDir.y = 0f;
        if (targetDir == Vector3.zero)
        {
            targetDir = Ctx.transform.forward;
        }
        Quaternion ledgeRotation = Quaternion.LookRotation(targetDir, Vector3.up);
        Ctx.transform.rotation = Quaternion.Slerp(Ctx.transform.rotation, ledgeRotation, Time.fixedDeltaTime * 5f);

        Ctx.Animator.SetBool(Ctx.IsGrabbingLedgeHash, true);
        Ctx.IsGrabbingLedge = true;
    }

    private void ClimbLedgeExit()
    {
        Ctx.Animator.SetBool(Ctx.IsClimbingLedgeHash, true);
        Ctx.Animator.SetBool(Ctx.IsClimbingTeleportHash, true);
        Ctx.IsClimbTeleporting = true;
    }

    public void HandleGravity()
    {
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
