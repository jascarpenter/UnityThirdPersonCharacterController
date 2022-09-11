using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitching : MonoBehaviour
{
    PlayerStateMachine Ctx;
    Cinemachine.CinemachineVirtualCamera c_VirtualCamera;

    [SerializeField] Transform _defaultTarget;
    [SerializeField] Transform _climbingTarget;

    // Start is called before the first frame update
    void Start()
    {
        Ctx = GetComponent<PlayerStateMachine>();
        c_VirtualCamera = GetComponent<Cinemachine.CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Ctx.IsGrabbingLedge)
        {
            c_VirtualCamera.m_LookAt = _climbingTarget;
        }
        else
        {
            c_VirtualCamera.m_LookAt = _defaultTarget;
        }
    }
}
