using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpAnticipationState : PlayerBaseState
{
    private readonly int JumpStartGroundedHash = Animator.StringToHash("JumpStartGrounded");
    
    private const float CrossFadeDuration = 0.1f;
    
    public PlayerJumpAnticipationState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base (currentContext, playerStateFactory)
    {

    }

    public override void EnterState()
    {
        Debug.Log("I am in JUMP ANTICIPATION state");
        Ctx.Animator.Play(JumpStartGroundedHash);
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        if (Ctx.IsJumpPressed)
        {
            Ctx.RequireNewJumpPress = true;
        }
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

    }
}
