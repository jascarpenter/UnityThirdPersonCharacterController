using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFreeClimbState : MonoBehaviour
{
    public Animator animator;
    public bool isClimbing;

    bool inPosition;
    bool isLerping;
    float t;
    Vector3 startPos;
    Vector3 targetPos;
    Quaternion startRot;
    Quaternion targetRot;
    public float positionOffset;
    public float offsetFromWall = 0.3f;
    public float speedMultiplier = 0.2f;

    Transform helper;
    float delta;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public void Init()
    {
        helper = new GameObject().transform;
        helper.name = "climb helper";
        CheckForClimb();
    }

    public void CheckForClimb()
    {
        Vector3 origin = transform.position;
        origin.y += 1.4f;
        Vector3 dir = transform.forward;
        RaycastHit hit;
        if (Physics.Raycast(origin, dir, out hit, 5))
        {
            InitForClimb(hit);
        }
    }

    void InitForClimb(RaycastHit hit)
    {
        isClimbing = true;
        helper.transform.rotation = Quaternion.LookRotation(-hit.normal);
        startPos = transform.position;
        targetPos = hit.point + (hit.normal * offsetFromWall);
        t = 0;
        inPosition = false;
        animator.CrossFade("CLIMB_FULL_STAY", 2);
        Debug.DrawRay(helper.transform.position, transform.forward, Color.black);
    }

    // Update is called once per frame
    void Update()
    {
        delta = Time.deltaTime;
        Tick(delta);
    }

    public void Tick(float delta)
    {
        if (!inPosition)
        {
            GetInPosition();
            return;
        }
    }

    void GetInPosition()
    {
        t += delta;

        if (t > 1)
        {
            t = 1;
            inPosition = true;
            Debug.Log(inPosition);

            // enable IK
        }

        Vector3 tp = Vector3.Lerp(startPos, targetPos, t);
        transform.position = tp;
    }

    Vector3 PosWithOffset(Vector3 origin, Vector3 target)
    {
        Vector3 direction = origin - target;
        direction.Normalize();
        Vector3 offset = direction * offsetFromWall;
        return target + offset;

    }
}
