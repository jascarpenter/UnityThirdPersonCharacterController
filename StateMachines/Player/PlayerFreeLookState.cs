// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class PlayerFreeLookState : MonoBehaviour
// {
//     private readonly int FreeLookSpeedHash = Animator.StringToHash("FreeLookSpeed");

//     private const float AnimatorDampTime = 0.1f; 

//     public PlayerFreeLookState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base (currentContext, playerStateFactory)
//     {

//     }

//     // public override void Enter()
//     // {
//     //     Ctx.InputReader.JumpEvent += OnJump;
//     // }

//     // public override void Tick(float deltaTime)
//     // {
//     //     Vector3 movement = CalculateMovement();

//     //     Move(movement * Ctx.FreeLookMovementSpeed, deltaTime);

//     //     if (Ctx.InputReader.MovementValue == Vector2.zero)
//     //     {
//     //         Ctx.Animator.SetFloat(FreeLookSpeedHash, 0, AnimatorDampTime, deltaTime);
//     //         return;
//     //     }

//     //     Ctx.Animator.SetFloat(FreeLookSpeedHash, 1, AnimatorDampTime, deltaTime);

//     //     FaceMovementDirection(movement, deltaTime);
//     // }

//     // public override void Exit()
//     // {
//     //     Ctx.InputReader.JumpEvent -= OnJump;
//     // }

//     public void MovePlayerRelativeToCamera()
//     {
//         float playerVerticalInput = Input.GetAxis("Vertical");
//         float playerHorizontalInput = Input.GetAxis("Horizontal");

//         Vector3 forward = Ctx.MainCameraTransform.forward;
//         Vector3 right = Ctx.MainCameraTransform.right;

//         forward.y = 0f;
//         right.y = 0f;

//         forward.Normalize();
//         right.Normalize();

//         // forward = forward.normalized;
//         // right = right.normalized;

//         Vector3 forwardRelativeVerticalInput = playerVerticalInput * forward;
//         Vector3 rightRelativeVerticalInput = playerHorizontalInput * forward; 

//         Vector3 cameraRelativeMovement = forwardRelativeVerticalInput + rightRelativeVerticalInput;

//         transform.Translate(cameraRelativeMovement, Space.World);
//         // return forward * Ctx.CurrentMovementInput.y + right * Ctx.CurrentMovementInput.x;
//     }

//     // private void FaceMovementDirection(Vector3 movement, float deltaTime)
//     // {
//     //     Ctx.transform.rotation = Quaternion.Lerp(
//     //         Ctx.transform.rotation,
//     //         Quaternion.LookRotation(movement),
//     //         deltaTime * Ctx.RotationDamping);
//     // }

//     // private void OnJump()
//     // {
//     //     Ctx.SwitchState(new PlayerJumpingState(Ctx));
//     // }

//     public override void EnterState()
//     {
//         throw new System.NotImplementedException();
//     }

//     public override void UpdateState()
//     {
//         throw new System.NotImplementedException();
//     }

//     public override void ExitState()
//     {
//         throw new System.NotImplementedException();
//     }

//     public override void CheckSwitchStates()
//     {
//         throw new System.NotImplementedException();
//     }

//     public override void InitializeSubState()
//     {
//         throw new System.NotImplementedException();
//     }
// }
