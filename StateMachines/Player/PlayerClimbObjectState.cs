using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbObjectState : PlayerBaseState, IRootState
{
    private Vector3 momentum;
    private float rotationSpeed = 0.7f;
    private Quaternion rotationAdjustment;
    private Quaternion correctRotation;
    private const float CrossFadeDuration = 0.1f;

    public PlayerClimbObjectState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base (currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Ctx.IsJumping = false;
        Ctx.RequireNewJumpPress = false;
        Debug.Log("Hello from CLIMB OBJECT");
        InitializeSubState();
        DisableGravity();
        Ctx.GrounderFBBIK.enabled = false;
    }


    public override void UpdateState()
    {
        InitializeSubState();
        if (Ctx.IsClimbingObject || Ctx.IsClimbTeleporting)
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
        Ctx.Animator.SetBool(Ctx.IsClimbingObjectHash, false);
        Ctx.Animator.SetBool(Ctx.IsClimbingTeleportHash, false);
        Ctx.IsFalling = false;
        Ctx.IsClimbTeleporting = false;
        Ctx.IsClimbingObject = false;
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
        if (Ctx.CanClimbObject && Ctx.IsClimbPressed)
        {
            ClimbObjectEnter();
        }

        if (Ctx.IsClimbingObject)
        {
            ClimbObjectExit();
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

    private void ClimbObjectEnter() 
    {
        Ctx.Animator.SetBool(Ctx.IsClimbingObjectHash, true);
        Vector3 objectPos = Ctx.Detection.ledgePos;
        Vector3 startPos = Ctx.transform.position;

        objectPos.y = Ctx.Detection.ledgePos.y - Ctx.CharacterController.height / 2.5f;
        Ctx.transform.position = Vector3.Lerp(startPos, objectPos - (Ctx.transform.forward * 0.4f), Time.deltaTime * 10f);

        Vector3 targetDir = -Ctx.Detection.wallNormal;
        
        targetDir.y = 0f;
        if (targetDir == Vector3.zero)
        {
            targetDir = Ctx.transform.forward;
        }
        Quaternion ledgeRotation = Quaternion.LookRotation(targetDir, Vector3.up);
        Ctx.transform.rotation = Quaternion.Slerp(Ctx.transform.rotation, ledgeRotation, Time.fixedDeltaTime * 5f);

        Ctx.IsClimbingObject = true;
    }

    private void ClimbObjectExit()
    {
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
