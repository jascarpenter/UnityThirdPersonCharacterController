using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;
using RootMotion.FinalIK;

public class PlayerStateMachine : MonoBehaviour
{
    // states
    PlayerBaseState _currentState;
    PlayerStateFactory _states;
    PlayerCollisions _detection;
    GrounderFBBIK _grounderFBBIK;

    // references
    PlayerInput _playerInput;
    CharacterController _characterController;
    // EasyCharacterMovement.CharacterMovement _characterController;
    Animator _animator;
    private Transform _mainCameraTransform;
    AvatarMask _avatarMask;
    public LedgeChecker _ledgeChecker;
    public LedgeCollider _ledgeCollider;
    public WallChecker _wallChecker;

    // player input values
    Vector2 _currentMovementInput;
    Vector3 _currentMovement;
    Vector3 _currentRunMovement;
    Vector3 _appliedMovement;
    bool _isMovementPressed;
    bool _isRunPressed;
    bool _isInteractPressed;
    bool _isClimbPressed;

    // jumping
    bool _isJumpPressed = false;
    bool _requireNewJumpPress = false;
    bool _isJumping;
    bool _isFalling;
    bool _isLandEntering;
    bool _isLanding;
    private float _initialJumpVelocity;
    [SerializeField] private float _maxJumpHeight = 1.0f;
    [SerializeField] private float _maxJumpTime = 0.75f;

    // gravity
    [SerializeField] private float _gravity = -9.8f;
    // [SerializeField] private float _groundedGravity = -0.5f;
    [SerializeField] private float _highFallTrigger = -3f;
    private int _zero = 0;

    // Edge detection
    private bool _isEdge;

    // optimization
    [SerializeField] private float _rotationFactorPerFrame = 15f;
    [SerializeField] private float _jumpRotationLimit = 4f;
    [SerializeField] private float _runMulitiplier = 4.5f;
    private const float CrossFadeDuration = 0.1f;

    // strings to hashes
    int _isWalkingHash;
    int _isRunningHash;
    int _isJumpAnticipatingHash;
    int _isJumpingHash;
    int _isFallingHash;
    int _isLandAnticipatingHash;
    int _isLandingHash;
    int _isEdgeBalancingHash;
    int _isWallCollidingHash;
    int _isGrabbingLedgeHash;
    int _isClimbingLedgeHash;
    int _isClimbingObjectHash;
    int _isClimbingTeleportHash;

    private readonly int _jumpStartGroundedHash = Animator.StringToHash("JumpStartGrounded");

    // Collisions
    private int _bodyFullLayerMaskHash;
    private int _bodyUpperLayerMaskHash;
    private int _bodyLowerLayerMaskHash;
    private int _armLeftLayerMaskHash;
    private int _armRightLayerMaskHash;
    private float _layerWeightVelocity;
    private bool _canClimbLedge;
    private bool _canClimbObject;
    private bool _isGrabbingLedge;
    private bool _isClimbingObject;
    private bool _isClimbTeleporting;

    // Climbing
    [Header("Climb Settings")]
    [SerializeField] private float _wallAngleMax;
    [SerializeField] private float _groundAngleMax;
    [SerializeField] private float _dropCheckDistance;
    [SerializeField] private LayerMask _layerMaskClimb;

    [Header("Heights")]
    [SerializeField] private float _overpassHeight;
    [SerializeField] private float _hangHeight;
    [SerializeField] private float _climbUpHeight;
    [SerializeField] private float _vaultHeight;
    [SerializeField] private float _stepHeight;

    [Header("Offsets")]
    [SerializeField] private Vector3 _endOffset;
    [SerializeField] private Vector3 _hangOffset;
    [SerializeField] private Vector3 _dropOffset;
    [SerializeField] private Vector3 _climbOriginDown;

    // [Header("Animation Settings")]
    // public CrossFadeSettings _standToFreeHandSetting;
    // public CrossFadeSettings _climbUpSetting;
    // public CrossFadeSettings _vaultSetting;
    // public CrossFadeSettings _stepUpSetting;
    // public CrossFadeSettings _dropSetting;
    // public CrossFadeSettings _dropToAirSetting;

    // Rigging (WIP)

    // getters and setters
    // references and states
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public CharacterController CharacterController { get { return _characterController; } }
    // public EasyCharacterMovement.CharacterMovement CharacterController { get { return _characterController; } }
    public Animator Animator { get { return _animator; } }
    public Transform MainCameraTransform { get { return _mainCameraTransform; } }
    public PlayerCollisions Detection { get { return _detection; } }
    public GrounderFBBIK GrounderFBBIK { get { return _grounderFBBIK; } }
    // player input values
    public Vector2 CurrentMovementInput { get { return _currentMovementInput; } }
    public Vector3 CurrentMovement { get { return _currentMovement; } set { _currentMovement = value; } }
    public float CurrentMovementY { get { return _currentMovement.y; } set { _currentMovement.y = value; } }
    public Vector3 AppliedMovement { get { return _appliedMovement; } set { _appliedMovement = value; } }
    public float AppliedMovementY { get { return _appliedMovement.y; } set { _appliedMovement.y = value; } }
    public float AppliedMovementX { get { return _appliedMovement.x; } set { _appliedMovement.x = value; } }
    public float AppliedMovementZ { get { return _appliedMovement.z; } set { _appliedMovement.z = value; } }
    public bool IsMovementPressed { get { return _isMovementPressed; } }
    public bool IsRunPressed { get { return _isRunPressed; } }
    public bool IsInteractPressed { get { return _isInteractPressed; } }
    public bool IsClimbPressed { get { return _isClimbPressed; } }
    // Jumping
    public bool IsJumpPressed { get { return _isJumpPressed; } }
    public bool RequireNewJumpPress { get { return _requireNewJumpPress; } set { _requireNewJumpPress = value; } }
    public bool IsJumping { set { _isJumping = value; } }
    public bool IsFalling { get { return _isFalling; } set { _isFalling = value; } }
    public bool IsLandEntering { get { return _isLandEntering; } set { _isLandEntering = value; } }
    public bool IsLanding { get { return _isLanding; } set { _isLanding = value; } }
    public float InitialJumpVelocity { get { return _initialJumpVelocity; } }
    public float MaxJumpHeight { set { _maxJumpHeight = value; } }
    public float MaxJumpTime { set { _maxJumpTime = value; } }
    // Gravity
    // public float GroundedGravity { get { return _groundedGravity; } set { _groundedGravity = value;} }
    public float Gravity { get { return _gravity; } }
    public float HighFallTrigger { get { return _highFallTrigger; } set { _highFallTrigger = value;} }
    public int Zero { get { return _zero; } set { _zero = value;} }
    // Edge detection
    public bool IsEdge { get { return _isEdge; } set { _isEdge = value; } }
    // optimization
    public float RunMulitiplier { get { return _runMulitiplier; } set { _runMulitiplier = value; } }
    public float RotationFactorPerFrame { get { return _rotationFactorPerFrame; } set { _rotationFactorPerFrame = value; } }

    // string to hashes
    public int IsWalkingHash { get { return _isWalkingHash; } }
    public int IsRunningHash { get { return _isRunningHash; } }
    public int IsJumpAnticipatingHash { get { return _isJumpAnticipatingHash; } }
    public int IsJumpingHash { get { return _isJumpingHash; } }
    public int IsFallingHash { get { return _isFallingHash; } }
    public int IsLandAnticipatingHash { get { return _isLandAnticipatingHash; } }
    public int IsLandingHash { get { return _isLandingHash; } }
    public int IsEdgeBalancingHash { get { return _isEdgeBalancingHash; } }
    public int IsWallCollidingHash { get { return _isWallCollidingHash; } }
    public int IsGrabbingLedgeHash { get { return _isGrabbingLedgeHash; } }
    public int IsClimbingLedgeHash { get { return _isClimbingLedgeHash; } }
    public int IsClimbingObjectHash { get { return _isClimbingObjectHash; } }
    public int IsClimbingTeleportHash { get { return _isClimbingTeleportHash; } }

    // testing
    // Collisions
    public int BodyFullLayerMaskHash { get { return _bodyFullLayerMaskHash; } }
    public int BodyUpperLayerMaskHash { get { return _bodyUpperLayerMaskHash; } }
    public int BodyLowerLayerMaskHash { get { return _bodyLowerLayerMaskHash; } }
    public int ArmLeftLayerMaskHash { get { return _armLeftLayerMaskHash; } }
    public int ArmRightLayerMaskHash { get { return _armRightLayerMaskHash; } }
    public bool CanClimbLedge { get { return _canClimbLedge; } set { _canClimbLedge = value; } }
    public bool CanClimbObject { get { return _canClimbObject; } set { _canClimbObject = value; } }
    public bool IsGrabbingLedge { get { return _isGrabbingLedge; } set { _isGrabbingLedge = value; } }
    public bool IsClimbingObject { get { return _isClimbingObject; } set { _isClimbingObject = value; } }
    public bool IsClimbTeleporting { get { return _isClimbTeleporting; } set { _isClimbTeleporting = value; } }

    // foot IK
    public Transform _footL;

    private void Awake()
    {
        _playerInput = new PlayerInput(); 
        _characterController = GetComponent<CharacterController>();
        _detection = GetComponent<PlayerCollisions>();
        _grounderFBBIK = GetComponent<GrounderFBBIK>();
        _animator = GetComponent<Animator>();
        _avatarMask = GetComponent<AvatarMask>();
        _ledgeChecker = GameObject.Find("LedgeCollider").GetComponent<LedgeChecker>();
        _ledgeCollider = GameObject.Find("LedgeCollider").GetComponent<LedgeCollider>();
        _wallChecker = GameObject.Find("WallCollider").GetComponent<WallChecker>();


        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();

        _isWalkingHash = Animator.StringToHash("isWalking");
        _isRunningHash = Animator.StringToHash("isRunning");
        _isJumpAnticipatingHash = Animator.StringToHash("isJumpAnticipating");
        _isJumpingHash = Animator.StringToHash("isJumping");
        _isFallingHash = Animator.StringToHash("isFalling");
        _isLandAnticipatingHash = Animator.StringToHash("IsLandAnticipating");
        _isLandingHash = Animator.StringToHash("isLanding");
        _isEdgeBalancingHash = Animator.StringToHash("isEdgeBalancing");
        _isWallCollidingHash = Animator.StringToHash("isWallColliding");
        _isGrabbingLedgeHash = Animator.StringToHash("isGrabbingLedge");
        _isClimbingLedgeHash = Animator.StringToHash("isClimbingLedge");
        _isClimbingObjectHash = Animator.StringToHash("isClimbingObject");
        _isClimbingTeleportHash = Animator.StringToHash("isClimbingTeleport");

        _bodyFullLayerMaskHash = _animator.GetLayerIndex("BodyFull");
        _bodyUpperLayerMaskHash = _animator.GetLayerIndex("BodyUpper");
        _bodyLowerLayerMaskHash = _animator.GetLayerIndex("BodyLower");
        _armLeftLayerMaskHash = _animator.GetLayerIndex("ArmLeft");
        _armRightLayerMaskHash = _animator.GetLayerIndex("ArmRight");
        
        _playerInput.CharacterControls.Move.started += OnMovementInput; 
        _playerInput.CharacterControls.Move.canceled += OnMovementInput; 
        _playerInput.CharacterControls.Move.performed += OnMovementInput; 

        _playerInput.CharacterControls.Run.started += OnRun;
        _playerInput.CharacterControls.Run.canceled += OnRun;

        _playerInput.CharacterControls.Jump.started += OnJump;
        _playerInput.CharacterControls.Jump.canceled += OnJump;

        _playerInput.CharacterControls.Interact.started += OnInteract;
        _playerInput.CharacterControls.Interact.canceled += OnInteract;

        _playerInput.CharacterControls.Climb.started += OnClimb;
        _playerInput.CharacterControls.Climb.canceled += OnClimb;

        SetupJumpVariables();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        _mainCameraTransform = Camera.main.transform;
        _characterController.Move(_appliedMovement * Time.deltaTime);
    }

    private void Update()
    {
        _currentState.UpdateStates(); 

        if (_isGrabbingLedge || _isClimbingObject || _isClimbTeleporting)
        {
            _characterController.Move(Vector3.zero);
        }
        else
        {
            HandleMovement();
            HandleRotation();
            _characterController.Move((_appliedMovement) * Time.deltaTime);
        }

        // //Climbing
        //         if (_isMovementPressed)
        //             if (_isClimbPressed)
        //                 if (CanClimb(out _downRaycastHit, out _forwardRaycastHit, out _endPosition))
        //                     InitiateClimb();
    }

    private void OnMovementInput(InputAction.CallbackContext context)
    {
        _currentMovementInput = context.ReadValue<Vector2>();
        _isMovementPressed = _currentMovementInput.x != _zero || _currentMovementInput.y != _zero;
    }

    private void OnRun(InputAction.CallbackContext context)
    {
        _isRunPressed = context.ReadValueAsButton();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
        _requireNewJumpPress = false;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        _isInteractPressed = context.ReadValueAsButton();
    }

    private void OnClimb(InputAction.CallbackContext context)
    {
        _isClimbPressed = context.ReadValueAsButton();
    }

    private void SetupJumpVariables()
    {
        float timeToApex = _maxJumpTime / 2;
        _gravity = (-2 * _maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        _initialJumpVelocity = (2 * _maxJumpHeight) / timeToApex;
    }

    private void HandleMovement()
    {
        Vector3 forward = _mainCameraTransform.forward;
        Vector3 right = _mainCameraTransform.right;
        forward.y = _zero;
        right.y = _zero;

        forward = forward.normalized;
        right = right.normalized;

        float currentGravity = _appliedMovement.y;
        _appliedMovement = AppliedMovementX * right + AppliedMovementZ * forward;
        _appliedMovement.y = currentGravity;
    }

    private void HandleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = AppliedMovementX;
        positionToLookAt.y = _zero;
        positionToLookAt.z = AppliedMovementZ;

        Quaternion currentRotation = transform.rotation;

        if (_isMovementPressed && positionToLookAt != Vector3.zero && !_isJumpPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationFactorPerFrame * Time.deltaTime);
        }
        else if (_isMovementPressed && positionToLookAt != Vector3.zero && _isJumpPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, (_rotationFactorPerFrame / _jumpRotationLimit) * Time.deltaTime);
        }
    }

    public void OffsetOnLedge()
    {
        _characterController.transform.parent = _ledgeChecker.transform;
        _characterController.transform.localPosition = _ledgeChecker.transform.position;
    }

    public void OnEnable()
    {
        _playerInput.CharacterControls.Enable();    
    }

    public void OnDisable()
    {
        _playerInput.CharacterControls.Disable();
    }

    // private void OnDrawGizmos()
    // {
    //     if (_drawLocal) { DrawLocal(); }
    //     if (_drawWorld) { DrawWorld(); }
    // }

    // private void DrawLocal()
    // {
    //     float _radius = _characterController.radius;
    //     float _totalHeight = Mathf.Max(_characterController.height, _radius * 2);

    //     Vector3 _top = _characterController.center + Vector3.up * (_totalHeight / 2 - _radius);
    //     Vector3 _bottom = _characterController.center - Vector3.up * (_totalHeight / 2 - _radius);

    //     Gizmos.color = Color.blue;
    //     Gizmos.DrawWireSphere(_top, _radius);
    //     Gizmos.DrawWireSphere(_bottom, _radius);
    // }

    // private void DrawWorld()
    // {
    //     float _heightScale = Mathf.Abs(transform.lossyScale.y);
    //     float _radiusScale = Mathf.Max(Mathf.Abs(transform.lossyScale.x), Mathf.Abs(transform.lossyScale.z));

    //     float _radius = _characterController.radius * _radiusScale;
    //     float _totalHeight = Mathf.Max(_characterController.height * _heightScale, _radius * 2);

    //     Vector3 _direction = transform.up;
    //     Vector3 _center = transform.TransformPoint(_characterController.center);
    //     Vector3 _top = _center + _direction * (_totalHeight / 2 - _radius);
    //     Vector3 _bottom = _center - _direction * (_totalHeight / 2 - _radius);

    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(_top, _radius);
    //     Gizmos.DrawWireSphere(_bottom, _radius);
    // }

    // private bool CanClimb(out RaycastHit downRaycastHit, out RaycastHit forwardRaycastHit, out Vector3 endPosition)
    // {
    //     endPosition = Vector3.zero;
    //     downRaycastHit = new RaycastHit();
    //     forwardRaycastHit = new RaycastHit();

    //     bool _downHit;
    //     bool _forwardHit;
    //     bool _overpassHit;
    //     float _climbHeight;
    //     float _groundAngle;
    //     float _wallAngle;

    //     RaycastHit _downRaycastHit;
    //     RaycastHit _forwardRaycastHit;
    //     RaycastHit _overpassRaycastHit;

    //     Vector3 _endPosition;
    //     Vector3 _forwardDirectionXZ;
    //     Vector3 _forwardNormalXZ;

    //     Vector3 _downDirection = Vector3.down;
    //     Vector3 _downOrigin = transform.TransformPoint(_climbOriginDown);

    //     _downHit = Physics.Raycast(_downOrigin, _downDirection, out _downRaycastHit, _climbOriginDown.y - _stepHeight, _layerMaskClimb);

    //     GameObject TestingSphere = GameObject.Find("TestingSphere");
    //     TestingSphere.transform.position = _downOrigin;

    //     if (_downHit)
    //     {
    //         float _forwardDistance = _climbOriginDown.z;
    //         Vector3 _forwardOrigin = new Vector3(transform.position.x, _downRaycastHit.point.y - 0.1f, transform.position.z);
    //         Vector3 _overpassOrigin = new Vector3(transform.position.x, _overpassHeight, transform.position.z);

    //         _forwardDirectionXZ = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
    //         _forwardHit = Physics.Raycast(_forwardOrigin, _forwardDirectionXZ, out _forwardRaycastHit, _forwardDistance, _layerMaskClimb);
    //         _overpassHit = Physics.Raycast(_overpassOrigin, _forwardDirectionXZ, out _overpassRaycastHit, _forwardDistance, _layerMaskClimb);
    //         _climbHeight = _downRaycastHit.point.y - transform.position.y;

    //         if (_forwardHit)
    //             if (_overpassHit || _climbHeight < _overpassHeight)
    //             {
    //                 //Angles
    //                 _forwardNormalXZ = Vector3.ProjectOnPlane(_forwardRaycastHit.normal, Vector3.up);
    //                 _groundAngle = Vector3.Angle(_downRaycastHit.normal, Vector3.up);
    //                 _wallAngle = Vector3.Angle(-_forwardNormalXZ, _forwardDirectionXZ);

    //                 if (_wallAngle <= _wallAngleMax)
    //                     if (_groundAngle <= _groundAngleMax)
    //                     {
    //                         //End offset
    //                         Vector3 _vectSurface = Vector3.ProjectOnPlane(_forwardDirectionXZ, _downRaycastHit.normal);
    //                         _endPosition = _downRaycastHit.point + Quaternion.LookRotation(_vectSurface, Vector3.up) * _endOffset;

    //                         //De-penetration
    //                         Collider _colliderB = _downRaycastHit.collider;
    //                         bool _penetrationOverlap = Physics.ComputePenetration(
    //                             colliderA: _characterController,
    //                             positionA: _endPosition,
    //                             rotationA: transform.rotation,
    //                             colliderB: _colliderB,
    //                             positionB: _colliderB.transform.position,
    //                             rotationB: _colliderB.transform.rotation,
    //                             direction: out Vector3 _penetrationDirection,
    //                             distance: out float _penetrationDistance);
    //                         if (_penetrationOverlap)
    //                             _endPosition += _penetrationDirection * _penetrationDistance;

    //                         //Up Sweep
    //                         float _inflate = -0.05f;
    //                         float _upsweepDistance = _downRaycastHit.point.y - transform.position.y;
    //                         Vector3 _upSweepDirection = transform.up;
    //                         Vector3 _upSweepOrigin = transform.position;
    //                         bool _upSweepHit = CharacterSweep(
    //                             position: _upSweepOrigin,
    //                             rotation: transform.rotation,
    //                             direction: _upSweepDirection,
    //                             distance: _upsweepDistance,
    //                             layerMask: _layerMaskClimb,
    //                             inflate: _inflate);

    //                         //Forward Sweep
    //                         Vector3 _forwardSweepOrigin = transform.position + _upSweepDirection * _upsweepDistance;
    //                         Vector3 _forwardSweepVector = _endPosition - _forwardSweepOrigin;
    //                         bool _forwardSweepHit = CharacterSweep(
    //                             position: _forwardSweepOrigin,
    //                             rotation: transform.rotation,
    //                             direction: _forwardSweepVector.normalized,
    //                             distance: _forwardSweepVector.magnitude,
    //                             layerMask: _layerMaskClimb,
    //                             inflate: _inflate);

    //                         if (!_upSweepHit && !_forwardSweepHit)
    //                         {
    //                             endPosition = _endPosition;
    //                             downRaycastHit = _downRaycastHit;
    //                             forwardRaycastHit = _forwardRaycastHit;
    //                             return true;
    //                         }
    //                     }
    //             }
    //     }
    // return false;
    // }

    // private bool CharacterSweep(Vector3 position, Quaternion rotation, Vector3 direction, float distance, LayerMask layerMask, float inflate)
    // {
    //     //Assuming capusle is on y axis
    //     float _heightScale = Mathf.Abs(transform.lossyScale.y);
    //     float _radiusScale = Mathf.Max(Mathf.Abs(transform.lossyScale.x), Mathf.Abs(transform.lossyScale.z));

    //     float _radius = _characterController.radius * _radiusScale;
    //     float _totalHeight = Mathf.Max(_characterController.height * _heightScale, _radius * 2);

    //     Vector3 _capsuleUp = rotation * Vector3.up;
    //     Vector3 _center = position + rotation * _characterController.center;
    //     Vector3 _top = _center + _capsuleUp * (_totalHeight / 2 - _radius);
    //     Vector3 _bottom = _center - _capsuleUp * (_totalHeight / 2 - _radius);

    //     bool _sweepHit = Physics.CapsuleCast(
    //         point1: _bottom,
    //         point2: _top,
    //         radius: _radius + inflate,
    //         direction: direction,
    //         maxDistance: distance,
    //         layerMask: layerMask);


    //     return _sweepHit;
    // }

    // private void InitiateClimb()
    // {
    //     _isClimbing = true;
    //     // _newSpeed = 0;
    //     // _animator.SetFloat("Forward", 0);
    //     _characterController.enabled = false;
    //     // _rigidBody.isKinematic = true;

    //     float _climbHeight = _downRaycastHit.point.y - transform.position.y;
    //     Vector3 _forwardNormalXZ = Vector3.ProjectOnPlane(_forwardRaycastHit.normal, Vector3.up);
    //     _forwardNormalXZRotation = Quaternion.LookRotation(-_forwardNormalXZ, Vector3.up);


    //     if (_climbHeight > _hangHeight)
    //     {
    //         _matchTargetPosition = _forwardRaycastHit.point + _forwardNormalXZRotation * _hangOffset;
    //         _matchTargetRotation = _forwardNormalXZRotation;
    //         _animator.CrossFadeInFixedTime(FreeFallHash, CrossFadeDuration);
    //     }
    //     else if (_climbHeight > _climbUpHeight)
    //     {
    //         _matchTargetPosition = _endPosition;
    //         _matchTargetRotation = _forwardNormalXZRotation;
    //         _animator.CrossFadeInFixedTime(FreeFallHash, CrossFadeDuration);
    //     }
    //     else if (_climbHeight > _vaultHeight)
    //     {
    //         _matchTargetPosition = _endPosition;
    //         _matchTargetRotation = _forwardNormalXZRotation;
    //         _animator.CrossFadeInFixedTime(FreeFallHash, CrossFadeDuration);
    //     }
    //     else if (_climbHeight > _stepHeight)
    //     {
    //         _matchTargetPosition = _endPosition;
    //         _matchTargetRotation = _forwardNormalXZRotation;
    //         _animator.CrossFadeInFixedTime(FreeFallHash, CrossFadeDuration);
    //     }
    //     else
    //     {
    //         _isClimbing = false;
    //     }
    // }
}