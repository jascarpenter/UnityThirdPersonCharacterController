using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using RootMotion.FinalIK;
using System;

public class PlayerCollisions : MonoBehaviour
{
    PlayerStateMachine Ctx;
    PlayerInput _playerInput;
    public InteractionSystem interactionSystem;
    [SerializeField] Transform _ledgeTransform;
    private Vector3 offset;

    private float _layerWeightVelocity;
    private float rotateSpeed = 0.1f;

    private float distancePlayerToLedge;
 
    public Vector3 ledgePos;
    private Vector3 ledgeNormal;
 
    [SerializeField] private LayerMask hitMask = 1;
 
    [SerializeField]
    float grabbingDistance = 0.3f;
 
    private Transform playerFace;
    private Transform playerSpine;
 
    private bool InRangeLedge = false;
    private bool InRangeObject = false;
    private bool hanging = false;
    GameObject ledgeDetector;
    GameObject ledgePosIndicator;
   
    private Vector3 raycastFloorPos;
 
    private Vector3 CombinedRaycastNormal;
    
    RaycastHit hit; 
    RaycastHit hitDown;
 
    Vector3 wallHitPoint;
    public Vector3 wallNormal;

    private float _castDownHeight = 4f;
    private float _castForwardDistance = 1f;
    private float _castForwardObjDist = 0.5f;

    [SerializeField] private float _overpassHeight = 1.8f;
    [SerializeField] private float _hangHeight = 2.1f;

    private void Awake()
    {
        Ctx = GetComponent<PlayerStateMachine>();
        _playerInput = new PlayerInput(); 
    }

    private void Start()
    {
        SpawnLedgePosIndicator();
        SpawnLedgeDetector();
    }

    private void Update()
    {
        CheckForLedge();
        CheckForObject();
    }

    private void CheckForLedge()
    {
        // draw ray to find walls, from the player head
        Vector3 rayForwardClimbPosition = new Vector3(Ctx.transform.position.x, Ctx.transform.position.y + Ctx.CharacterController.height, Ctx.transform.position.z);

        Debug.DrawRay(rayForwardClimbPosition, transform.forward, Color.red);
        if (Physics.Raycast(rayForwardClimbPosition, transform.forward, out hit, _castForwardDistance, hitMask))
        {
            Debug.DrawRay(rayForwardClimbPosition, transform.forward, Color.green);          
            wallHitPoint = hit.point;
            wallNormal = hit.normal;
            // print(hit.collider.gameObject.name);
            InRangeLedge = true;
        }
        else
        {
            InRangeLedge = false;
            Ctx.CanClimbLedge = false;
            // drop down if there is nothing in front of us and we are hanging
            // if (hanging)
            // {                
            //     StopHanging();              
            // }
        }
            
        // if in range of wall, shoot ray downward from the ledge detector
        if (InRangeLedge)
        {   
            Vector3 down = -ledgeDetector.transform.up;
            float _climbHeight;

            Debug.DrawRay(ledgeDetector.transform.position, down * 4f, Color.yellow);
            if (Physics.Raycast(ledgeDetector.transform.position, down, out hitDown, _castDownHeight, hitMask))
            {
                Debug.DrawRay(ledgeDetector.transform.position, down * 4f, Color.cyan);
                ledgeNormal = hitDown.normal;
                _climbHeight = hitDown.point.y - transform.position.y;
                ledgePos = new Vector3(wallHitPoint.x, hitDown.point.y, wallHitPoint.z);
                if (Ctx.CanClimbLedge != true)
                {
                    Ctx.CanClimbLedge = true;
                }

                // if (_climbHeight < _hangHeight)
                // {
                //     Ctx.IsGrabbingLedge = true;
                // }
            }
            else
            {
                //reset the position
                if (ledgePos != Vector3.zero)
                {
                    ledgePos = Vector3.zero;
                }
                Ctx.CanClimbLedge = false;
            }
            // set the ledge indicator block
            ledgePosIndicator.SetActive(true);
            ledgePosIndicator.transform.position = ledgePos; 
         }
    }

    private void CheckForObject()
    {
        // draw ray to find walls, from the player head
        Vector3 rayForwardClimbPosition = new Vector3(Ctx.transform.position.x, Ctx.transform.position.y + Ctx.CharacterController.height / 2.5f, Ctx.transform.position.z);

        Debug.DrawRay(rayForwardClimbPosition, transform.forward, Color.red);
        if (Physics.Raycast(rayForwardClimbPosition, transform.forward, out hit, _castForwardObjDist, hitMask))
        {
            Debug.DrawRay(rayForwardClimbPosition, transform.forward, Color.green);          
            wallHitPoint = hit.point;
            wallNormal = hit.normal;
            // print(hit.collider.gameObject.name);
            InRangeObject = true;
        }
        else
        {
            InRangeObject = false;
            Ctx.CanClimbObject = false;
            // drop down if there is nothing in front of us and we are hanging
            // if (hanging)
            // {                
            //     StopHanging();              
            // }
        }
            
        // if in range of wall, shoot ray downward from the ledge detector
        if (InRangeObject && !InRangeLedge)
        {   
            Vector3 down = -ledgeDetector.transform.up;
            float _climbHeight;

            Debug.DrawRay(ledgeDetector.transform.position, down * 4f, Color.yellow);
            if (Physics.Raycast(ledgeDetector.transform.position, down, out hitDown, _castDownHeight, hitMask))
            {
                Debug.DrawRay(ledgeDetector.transform.position, down * 4f, Color.cyan);
                ledgeNormal = hitDown.normal;
                _climbHeight = hitDown.point.y - transform.position.y;
                ledgePos = new Vector3(wallHitPoint.x, hitDown.point.y, wallHitPoint.z);
                if (Ctx.CanClimbObject != true)
                {
                    Ctx.CanClimbObject = true;
                }

                // if (_climbHeight < _hangHeight)
                // {
                //     Ctx.IsGrabbingLedge = true;
                // }
            }
            else
            {
                //reset the position
                if (ledgePos != Vector3.zero)
                {
                    ledgePos = Vector3.zero;
                }
                Ctx.CanClimbObject = false;
            }
            // set the ledge indicator block
            ledgePosIndicator.SetActive(true);
            ledgePosIndicator.transform.position = ledgePos; 
         }
    }

    // Vector3 FloorRaycasts(float offsetx, float offsetz, float raycastLength)
    // {       
    //     //  raycast from multiple points
    //     raycastFloorPos = ledgeDetector.transform.TransformPoint(0 + offsetx, -2f, 0 + offsetz);

    //     Debug.DrawRay(raycastFloorPos, Vector3.down * raycastLength, Color.red);
    //     if (Physics.Raycast(raycastFloorPos, Vector3.down, out hit, raycastLength, hitMask, QueryTriggerInteraction.Collide))
    //     {
    //         if(hit.normal.y > 0.9f && hit.normal.y < 1.1f)
    //        Debug.DrawRay(raycastFloorPos, Vector3.down * raycastLength, Color.green);
    //         return hit.normal;
    //     }
    //     else return Vector3.zero;
    // }

    void SpawnLedgePosIndicator()
    {
        ledgePosIndicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ledgePosIndicator.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        ledgePosIndicator.GetComponent<Collider>().enabled = false;     
    }
 
    void SpawnLedgeDetector()
    {
        ledgeDetector = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ledgeDetector.transform.SetParent(transform);
        ledgeDetector.transform.localPosition = new Vector3(0, 3.5f, 0.5f);
        ledgeDetector.GetComponent<Collider>().enabled = false;
    }

    public void OnTriggerEnter(Collider other) 
    {
        if (Ctx.CharacterController.isGrounded && other.CompareTag("Edge"))
        {
            EnterEdge();
        }
    }

    public void OnTriggerStay(Collider other) 
    {
        if (Ctx.CharacterController.isGrounded)
        {
            if (other.CompareTag("Edge"))
            {
                if (!Ctx.IsMovementPressed && !Ctx.IsRunPressed)
                {
                    EnterEdge();
                }
                else if ((Ctx.IsMovementPressed && !Ctx.IsRunPressed) || (Ctx.IsMovementPressed && Ctx.IsRunPressed) || (Ctx.IsJumpPressed))
                {
                    ExitEdge();
                }
            }
        }
        else if (!Ctx.CharacterController.isGrounded || Ctx.IsJumpPressed)
        {
            ExitEdge();
        }
    }

    public void OnTriggerExit(Collider other) 
    {
        ExitEdge();
    }

    private void EnterWallHang()
    {
        Ctx.Animator.SetBool(Ctx.IsGrabbingLedgeHash, true);
    }

    private void ExitWallHang()
    {
        Ctx.Animator.SetBool(Ctx.IsClimbingLedgeHash, true);
    }

    private void EnterEdge()
    {
        Ctx.Animator.SetBool(Ctx.IsEdgeBalancingHash, true);
    }

    private void ExitEdge()
    {
        Ctx.Animator.SetBool(Ctx.IsEdgeBalancingHash, false);
    }

    private void EnableCharacterController()
    {
        if (Ctx.CharacterController.enabled)
        {
            return;
        }
        else
        {
            Ctx.CharacterController.enabled = true; 
        }   
    }

    private void DisableCharacterController()
    {
        if (!Ctx.CharacterController.enabled)
        {
            return;
        }
        else
        {
            Ctx.CharacterController.enabled = false; 
        }  
    }
}
