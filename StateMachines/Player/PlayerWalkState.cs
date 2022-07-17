using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    private readonly int WalkHash = Animator.StringToHash("Walk");

    private const float CrossFadeDuration = 0.1f;

    public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base (currentContext, playerStateFactory)
    {

    }

    public override void EnterState()
    {
        Ctx.Animator.SetBool(Ctx.IsWalkingHash, true);
        Ctx.Animator.SetBool(Ctx.IsRunningHash, false);
        // InitializeSubState();
    }

    public override void UpdateState()
    {
        // Ctx.Animator.SetBool(Ctx.IsWalkingHash, true);
        Ctx.AppliedMovementX = Ctx.CurrentMovementInput.x;
        Ctx.AppliedMovementZ = Ctx.CurrentMovementInput.y;
        CheckSwitchStates();
    }

    public override void ExitState()
    {

    }

    public override void CheckSwitchStates()
    {
        if (!Ctx.IsMovementPressed)
        {
            SwitchState(Factory.Idle());
        } 
        else if (Ctx.IsMovementPressed && Ctx.IsRunPressed)
        {
            SwitchState(Factory.Run());
        }
    }

    public override void InitializeSubState()
    {
        
        // Ctx.Animator.CrossFadeInFixedTime(WalkHash, CrossFadeDuration);
    }
}
