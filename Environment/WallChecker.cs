using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine.Animations.Rigging;
using UnityEngine;

public class WallChecker : MonoBehaviour
{
    PlayerStateMachine Ctx;
    public InteractionSystem interactionSystem;
    PlayerInput _playerInput;
    [SerializeField] GameObject bodyTarget;
    [SerializeField] GameObject bodyDefault;

    private float _layerWeightVelocity;
    private float _targetHandsLayerWeight = 1f;
    private float _targetBodyLayerWeight = 0.7f;
    private float _enterTime = 0.1f;
    private float _exitTime = 0.2f;
    private float _defaultRunMultiplier = 4.5f;
    private float _targetRunMultiplier = 2f;
    private float _transitionRunTime = 0.1f;

    public WallCollider ColliderTopLeft;
    public WallCollider ColliderBottomLeft;
    public WallCollider ColliderTopRight;
    public WallCollider ColliderBottomRight;
    
    private void Start()
    {
        Ctx = GetComponentInParent<PlayerStateMachine>();
        _playerInput = new PlayerInput(); 
    }

    private void FixedUpdate()
    {
        if (Ctx.CharacterController.isGrounded)
        {
            foreach (GameObject obj in ColliderTopLeft.CollidedObjects)
            {
                if (ColliderTopLeft.CollidedObjects.Contains(obj))
                {
                    if ((Ctx.IsMovementPressed && !Ctx.IsRunPressed) || (Ctx.IsMovementPressed && Ctx.IsRunPressed))
                    {
                        EnterWallHugBody();
                        EnterWallHugLeft();
                    }
                    else if ((!Ctx.IsMovementPressed && !Ctx.IsRunPressed))
                    {
                        ExitWallHugBody();
                        ExitWallHugLeft();
                    }
                }
            }

            foreach (GameObject obj in ColliderTopRight.CollidedObjects)
            {
                if (ColliderTopRight.CollidedObjects.Contains(obj))
                {
                    if ((Ctx.IsMovementPressed && !Ctx.IsRunPressed) || (Ctx.IsMovementPressed && Ctx.IsRunPressed))
                    {
                        EnterWallHugBody();
                        EnterWallHugRight();
                    }
                    else if ((!Ctx.IsMovementPressed && !Ctx.IsRunPressed))
                    {
                        ExitWallHugBody();
                        ExitWallHugRight();
                    }
                }

                // if (ColliderTopRight.CollidedObjects.Contains(obj))
                // {
                //     if ((Ctx.IsMovementPressed && !Ctx.IsRunPressed) || (Ctx.IsMovementPressed && Ctx.IsRunPressed))
                //     {
                //         EnterWallHugRight();
                //     }
                //     else if ((!Ctx.IsMovementPressed && !Ctx.IsRunPressed))
                //     {
                //         ExitWallHugRight();
                //     }
                // }
            }
        }
        else if (!Ctx.CharacterController.isGrounded || ((ColliderTopLeft.CollidedObjects.Count == 0) && (ColliderTopRight.CollidedObjects.Count == 0)))
        {
            ExitWallHugBody();
            ExitWallHugLeft();
            ExitWallHugRight();
        }

        if ((ColliderTopLeft.CollidedObjects.Count == 0))
        {
            ExitWallHugLeft();
            ExitWallHugBody();
        }   

        if ((ColliderTopRight.CollidedObjects.Count == 0))
        {
            ExitWallHugRight();
            ExitWallHugBody();
        }   
    }

    private void LateUpdate()
    {
        
    }

    // private void EnterWallHug()
    // {
    //     float currentHandLeftLayerWeight = Ctx.Animator.GetLayerWeight(Ctx.ArmLeftLayerMaskHash);
    //     float currentHandRightLayerWeight = Ctx.Animator.GetLayerWeight(Ctx.ArmRightLayerMaskHash);
    //     float currentBodyFullLayerWeight = Ctx.Animator.GetLayerWeight(Ctx.BodyFullLayerMaskHash);
    //     float targetHandsLayerWeight = 1f;
    //     float targetBodyLayerWeight = 0.7f;

    //     Ctx.Animator.SetLayerWeight(Ctx.ArmLeftLayerMaskHash, Mathf.SmoothDamp(currentHandLeftLayerWeight, targetHandsLayerWeight, ref _layerWeightVelocity, 0.1f));
    //     Ctx.Animator.SetLayerWeight(Ctx.ArmRightLayerMaskHash, Mathf.SmoothDamp(currentHandRightLayerWeight, targetHandsLayerWeight, ref _layerWeightVelocity, 0.1f));
    //     Ctx.Animator.SetLayerWeight(Ctx.BodyFullLayerMaskHash, Mathf.SmoothDamp(currentBodyFullLayerWeight, targetBodyLayerWeight, ref _layerWeightVelocity, 0.1f));
    //     Ctx.RunMulitiplier = 2f;
    //     Ctx.RotationFactorPerFrame = 7f;
    // }

    // private void ExitWallHug()
    // {
    //     float currentBodyFullLayerWeight = Ctx.Animator.GetLayerWeight(Ctx.BodyFullLayerMaskHash);
    //     float currentHandLeftLayerWeight = Ctx.Animator.GetLayerWeight(Ctx.ArmLeftLayerMaskHash);
    //     float currentHandRightLayerWeight = Ctx.Animator.GetLayerWeight(Ctx.ArmRightLayerMaskHash);

    //     Ctx.Animator.SetLayerWeight(Ctx.BodyFullLayerMaskHash, Mathf.SmoothDamp(currentBodyFullLayerWeight, Ctx.Zero, ref _layerWeightVelocity, 0.1f));
    //     Ctx.Animator.SetLayerWeight(Ctx.ArmLeftLayerMaskHash, Mathf.SmoothDamp(currentHandLeftLayerWeight, Ctx.Zero, ref _layerWeightVelocity, 0.1f));
    //     Ctx.RunMulitiplier = 4.5f;
    //     Ctx.RotationFactorPerFrame = 15f;
    // }

    // BODY
    private void EnterWallHugBody()
    {
        float currentBodyFullLayerWeight = Ctx.Animator.GetLayerWeight(Ctx.BodyFullLayerMaskHash);

        Ctx.Animator.SetLayerWeight(Ctx.BodyFullLayerMaskHash, Mathf.SmoothDamp(currentBodyFullLayerWeight, _targetBodyLayerWeight, ref _layerWeightVelocity, _enterTime));
        Ctx.RunMulitiplier = Mathf.Lerp(_defaultRunMultiplier, _targetRunMultiplier, _transitionRunTime / Time.deltaTime);
        Ctx.RotationFactorPerFrame = 7f;
    }

    private void ExitWallHugBody()
    {
        float currentBodyFullLayerWeight = Ctx.Animator.GetLayerWeight(Ctx.BodyFullLayerMaskHash);

        Ctx.Animator.SetLayerWeight(Ctx.BodyFullLayerMaskHash, Mathf.SmoothDamp(currentBodyFullLayerWeight, Ctx.Zero, ref _layerWeightVelocity, _exitTime));

        Ctx.RunMulitiplier = Mathf.Lerp(_targetRunMultiplier, _defaultRunMultiplier, _transitionRunTime / Time.deltaTime);
        Ctx.RotationFactorPerFrame = 15f;
    }

    // LEFT
    private void EnterWallHugLeft()
    {
        float currentArmLeftLayerWeight = Ctx.Animator.GetLayerWeight(Ctx.ArmLeftLayerMaskHash);
        // float currentBodyLayerWeight = Ctx.Animator.GetLayerWeight(Ctx.WallBodyLayerMaskHash);

        Ctx.Animator.SetLayerWeight(Ctx.ArmLeftLayerMaskHash, Mathf.SmoothDamp(currentArmLeftLayerWeight, _targetHandsLayerWeight, ref _layerWeightVelocity, _enterTime));
        // Ctx.Animator.SetLayerWeight(Ctx.WallBodyLayerMaskHash, Mathf.SmoothDamp(currentBodyLayerWeight, targetBodyLayerWeight, ref _layerWeightVelocity, 0.1f));
        // Ctx.RunMulitiplier = 2f;
        // Ctx.RotationFactorPerFrame = 7f;
    }

    private void ExitWallHugLeft()
    {
        float currentArmLeftLayerWeight = Ctx.Animator.GetLayerWeight(Ctx.ArmLeftLayerMaskHash);
        // float currentBodyLayerWeight = Ctx.Animator.GetLayerWeight(Ctx.WallBodyLayerMaskHash);

        Ctx.Animator.SetLayerWeight(Ctx.ArmLeftLayerMaskHash, Mathf.SmoothDamp(currentArmLeftLayerWeight, Ctx.Zero, ref _layerWeightVelocity, _exitTime));
        // Ctx.Animator.SetLayerWeight(Ctx.WallBodyLayerMaskHash, Mathf.SmoothDamp(currentBodyLayerWeight, Ctx.Zero, ref _layerWeightVelocity, 0.1f));

        // Ctx.RunMulitiplier = 4.5f;
        // Ctx.RotationFactorPerFrame = 15f;
    }

    // RIGHT
    private void EnterWallHugRight()
    {
        float currentArmRightLayerWeight = Ctx.Animator.GetLayerWeight(Ctx.ArmRightLayerMaskHash);
        // float currentBodyLayerWeight = Ctx.Animator.GetLayerWeight(Ctx.WallBodyLayerMaskHash);

        Ctx.Animator.SetLayerWeight(Ctx.ArmRightLayerMaskHash, Mathf.SmoothDamp(currentArmRightLayerWeight, _targetHandsLayerWeight, ref _layerWeightVelocity, _enterTime));
        // Ctx.Animator.SetLayerWeight(Ctx.WallBodyLayerMaskHash, Mathf.SmoothDamp(currentBodyLayerWeight, targetBodyLayerWeight, ref _layerWeightVelocity, 0.1f));

        // Ctx.RunMulitiplier = 2f;
        // Ctx.RotationFactorPerFrame = 7f;
    }

    private void ExitWallHugRight()
    {
        float currentArmRightLayerWeight = Ctx.Animator.GetLayerWeight(Ctx.ArmRightLayerMaskHash);
        // float currentBodyLayerWeight = Ctx.Animator.GetLayerWeight(Ctx.WallBodyLayerMaskHash);

        Ctx.Animator.SetLayerWeight(Ctx.ArmRightLayerMaskHash, Mathf.SmoothDamp(currentArmRightLayerWeight, Ctx.Zero, ref _layerWeightVelocity, _exitTime));
        // Ctx.Animator.SetLayerWeight(Ctx.WallBodyLayerMaskHash, Mathf.SmoothDamp(currentBodyLayerWeight, Ctx.Zero, ref _layerWeightVelocity, 0.1f));

        // Ctx.RunMulitiplier = 4.5f;
        // Ctx.RotationFactorPerFrame = 15f;
    }
}
