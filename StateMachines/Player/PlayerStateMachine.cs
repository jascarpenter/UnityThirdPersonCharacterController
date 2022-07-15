using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    // getters and setters
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public Animator Animator { get { return _animator; } }
    public CharacterController CharacterController { get { return _characterController; } }
    public Transform MainCameraTransform { get { return _mainCameraTransform; } }
    // Hashes
    public int IsWalkingHash { get { return _isWalkingHash; } }
    public int IsRunningHash { get { return _isRunningHash; } }
    public int IsJumpAnticipatingHash { get { return _isJumpAnticipatingHash; } }
    public int IsJumpingHash { get { return _isJumpingHash; } }
    public int IsFallingHash { get { return _isFallingHash; } }
    public int IsLandAnticipatingHash { get { return _isLandAnticipatingHash; } }
    public int IsLandingHash { get { return _isLandingHash; } }
    // Movement input
    public bool IsMovementPressed { get { return _isMovementPressed; } }
    public bool IsRunPressed { get { return _isRunPressed; } }
    public float CurrentMovementY { get { return _currentMovement.y; } set { _currentMovement.y = value; } }
    public float AppliedMovementY { get { return _appliedMovement.y; } set { _appliedMovement.y = value; } }
    public float AppliedMovementX { get { return _appliedMovement.x; } set { _appliedMovement.x = value; } }
    public float AppliedMovementZ { get { return _appliedMovement.z; } set { _appliedMovement.z = value; } }
    public float RunMulitiplier { get { return _runMultipier; } }
    public Vector2 CurrentMovementInput { get { return _currentMovementInput; } }
    // Jumping
    public bool IsJumping { set { _isJumping = value; } }
    public bool IsJumpAnimating { set { _isJumpAnimating = value; } }
    public bool IsJumpPressed { get { return _isJumpPressed; } }
    public bool RequireNewJumpPress { get { return _requireNewJumpPress; } set { _requireNewJumpPress = value; } }
    public float InitialJumpVelocity { get { return _initialJumpVelocity; } }
    public float MaxJumpHeight { set { _maxJumpHeight = value; } }
    public float MaxJumpTime { set { _maxJumpTime = value; } }
    // Gravity
    // public float GroundedGravity { get { return _groundedGravity; } set { _groundedGravity = value;} }
    public float Gravity { get { return _gravity; } set { _gravity = value;} }

    // variables
    PlayerBaseState _currentState;
    PlayerStateFactory _states;

    PlayerInput _playerInput;
    CharacterController _characterController;
    Animator _animator;

    private Transform _mainCameraTransform;

    int _isWalkingHash;
    int _isRunningHash;
    int _isJumpAnticipatingHash;
    int _isJumpingHash;
    int _isFallingHash;
    int _isLandAnticipatingHash;
    int _isLandingHash;

    private readonly int _jumpStartGroundedHash = Animator.StringToHash("JumpStartGrounded");
    
    private const float CrossFadeDuration = 0.1f;

    Vector2 _currentMovementInput;
    Vector3 _currentMovement;
    Vector3 _currentRunMovement;
    Vector3 _appliedMovement;

    bool _isMovementPressed;
    bool _isRunPressed = false;
    bool _isJumpPressed = false;
    bool _requireNewJumpPress = false;

    private float _initialJumpVelocity;
    [SerializeField] private float _rotationFactorPerFrame = 15f;
    [SerializeField] private float _runMultipier = 3.0f;
    // [SerializeField] private float _groundedGravity = -0.5f;
    [SerializeField] private float _gravity = -9.8f;
    [SerializeField] private float _maxJumpHeight = 1.0f;
    [SerializeField] private float _maxJumpTime = 0.75f;

    bool _isJumping = false;
    bool _isJumpAnimating = false;

    private void Awake()
    {
        _playerInput = new PlayerInput(); 
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

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

        _playerInput.CharacterControls.Move.started += OnMovementInput; 
        _playerInput.CharacterControls.Move.canceled += OnMovementInput; 
        _playerInput.CharacterControls.Move.performed += OnMovementInput; 

        _playerInput.CharacterControls.Run.started += OnRun;
        _playerInput.CharacterControls.Run.canceled += OnRun;

        _playerInput.CharacterControls.Jump.started += OnJump;
        _playerInput.CharacterControls.Jump.canceled += OnJump;

        SetupJumpVariables();
    }

    private void Start()
    {
        _mainCameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        HandleRotation();
        _currentState.UpdateStates();

        _appliedMovement.x = _isRunPressed ? _currentMovementInput.x * _runMultipier : _currentMovementInput.x;
        _appliedMovement.z = _isRunPressed ? _currentMovementInput.y * _runMultipier : _currentMovementInput.y;

        _characterController.Move(_appliedMovement * Time.deltaTime);
    }

    private void OnMovementInput(InputAction.CallbackContext context)
    {
        _currentMovementInput = context.ReadValue<Vector2>();
        _currentMovement.x = _currentMovementInput.x;
        _currentMovement.z = _currentMovementInput.y;
    
        _currentRunMovement.x = _currentMovementInput.x * _runMultipier;
        _currentRunMovement.z = _currentMovementInput.y * _runMultipier;
        _isMovementPressed = _currentMovementInput.x != 0 || _currentMovementInput.y != 0;
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

    private void SetupJumpVariables()
    {
        float timeToApex = _maxJumpTime / 2;
        _gravity = (-2 * _maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        _initialJumpVelocity = (2 * _maxJumpHeight) / timeToApex;
    }

    private void HandleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = _currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = _currentMovement.z;

        Quaternion currentRotation = transform.rotation;

        if (_isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationFactorPerFrame * Time.deltaTime);
        }
    }

    public void MovePlayerRelativeToCamera()
    {
        float playerVerticalInput = Input.GetAxis("Vertical");
        float playerHorizontalInput = Input.GetAxis("Horizontal");

        Vector3 forward = _mainCameraTransform.forward;
        Vector3 right = _mainCameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        // forward = forward.normalized;
        // right = right.normalized;

        Vector3 forwardRelativeVerticalInput = playerVerticalInput * forward;
        Vector3 rightRelativeVerticalInput = playerHorizontalInput * forward; 

        Vector3 cameraRelativeMovement = forwardRelativeVerticalInput + rightRelativeVerticalInput;

        transform.Translate(cameraRelativeMovement, Space.World);
    }

    private void OnEnable()
    {
        _playerInput.CharacterControls.Enable();    
    }

    private void OnDisable()
    {
        _playerInput.CharacterControls.Disable();
    }
}
