using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class IKFootBehavior : MonoBehaviour
{
    [SerializeField] private Transform footTransformL;
    [SerializeField] private Transform footTransformR;
    private Transform[] allFootTransforms;
    [SerializeField] private Transform footTargetTransformL;
    [SerializeField] private Transform footTargetTransformR;
    private Transform[] allTargetTransforms;
    [SerializeField] private GameObject footRigL;
    [SerializeField] private GameObject footRigR;
    private TwoBoneIKConstraint[] allFootIKConstraints;
    private LayerMask groundLayerMask;
    [SerializeField] private float maxHitDistance = 1f;
    [SerializeField] private float addedHeight = 0.75f;
    private bool[] allGroundSpherecastHits;
    private LayerMask hitLayer;
    private Vector3[] allHitNormals;
    private float angleAboutX;
    private float angleAboutZ;
    [SerializeField] private float yOffset = 0.07f;

    // Start is called before the first frame update
    void Start()
    {
        allFootTransforms = new Transform[2];
        allFootTransforms[0] = footTransformL;
        allFootTransforms[1] = footTransformR;

        allTargetTransforms = new Transform[2];
        allTargetTransforms[0] = footTargetTransformL;
        allTargetTransforms[1] = footTargetTransformR;

        allFootIKConstraints = new TwoBoneIKConstraint[2];
        allFootIKConstraints[0] = footRigL.GetComponent<TwoBoneIKConstraint>();
        allFootIKConstraints[1] = footRigR.GetComponent<TwoBoneIKConstraint>();

        groundLayerMask = LayerMask.NameToLayer("Ground");

        allGroundSpherecastHits = new bool[3];

        allHitNormals = new Vector3[2];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 targetDown = footTargetTransformL.transform.TransformDirection(Vector3.down) * 0.5f;
        Vector3 footDown = footTransformL.transform.TransformDirection(Vector3.down) * 0.5f;
        Debug.DrawRay(footTargetTransformL.transform.position, targetDown, Color.green);
        Debug.DrawRay(footTransformL.transform.position, targetDown, Color.black);
        RotateCharacterFeet();
    }

    private void CheckGroundBelow(out Vector3 hitPoint, out bool gotGroundSpherecastHit, out Vector3 hitNormal, out LayerMask hitLayer, out float currentHitDistance, Transform objectTransform, int checkForLayerMask, float maxHitDistance, float addedHeight)
    {
        RaycastHit hit;
        Vector3 startSpherecast = objectTransform.position + new Vector3(0f, addedHeight, 0f);

        if (checkForLayerMask == -1)
        {
            Debug.LogError("Layer does not exist!");
            gotGroundSpherecastHit = false;
            currentHitDistance = 0f;
            hitLayer = LayerMask.NameToLayer("Player");
            hitNormal = Vector3.up;
            hitPoint = objectTransform.position;
        }
        else
        {
            int layerMask = (1 << checkForLayerMask);
            if (Physics.SphereCast(startSpherecast, 0.2f, Vector3.down, out hit, maxHitDistance, layerMask, QueryTriggerInteraction.UseGlobal))
            {
                hitLayer = hit.transform.gameObject.layer;
                currentHitDistance = hit.distance - addedHeight;
                hitNormal = hit.normal;
                gotGroundSpherecastHit = true;
                hitPoint = hit.point;
            }
            else
            {
                gotGroundSpherecastHit = false;
                currentHitDistance = 0f;
                hitLayer = LayerMask.NameToLayer("Player");
                hitNormal = Vector3.up;
                hitPoint = objectTransform.position;
            }
        }
    }

    Vector3 ProjectOnContactPlane(Vector3 vector, Vector3 hitNormal)
    {
        // catlikecoding
        return vector - hitNormal * Vector3.Dot(vector, hitNormal);
    }

    private void ProjectedAxisAngles(out float angleAboutX, out float angleAboutZ, Transform footTargetTransform, Vector3 hitNormal)
    {
        Vector3 xAxisProjected = ProjectOnContactPlane(footTargetTransform.forward, hitNormal).normalized;
        Vector3 zAxisProjected = ProjectOnContactPlane(footTargetTransform.right, hitNormal).normalized;

        angleAboutX = Vector3.SignedAngle(footTargetTransform.forward, xAxisProjected, footTargetTransform.right);
        angleAboutZ = Vector3.SignedAngle(footTargetTransform.right, zAxisProjected, footTargetTransform.forward);
    }

    private void RotateCharacterFeet()
    {    
        for (int i = 0; i < 2; i++)
        {
            CheckGroundBelow(out Vector3 hitPoint, out allGroundSpherecastHits[i], out Vector3 hitNormal, out hitLayer, out _, allFootTransforms[i], groundLayerMask, maxHitDistance, addedHeight);
            allHitNormals[i] = hitNormal;

            if (allGroundSpherecastHits[i] == true)
            {
                ProjectedAxisAngles(out angleAboutX, out angleAboutZ, allFootTransforms[i], allHitNormals[i]);

                allTargetTransforms[i].position = new Vector3(allFootTransforms[i].position.x, hitPoint.y + yOffset, allFootTransforms[i].position.z);

                allTargetTransforms[i].rotation = allFootTransforms[i].rotation;

                allTargetTransforms[i].localEulerAngles = new Vector3(allTargetTransforms[i].localEulerAngles.x + angleAboutX, allTargetTransforms[i].localEulerAngles.y, allTargetTransforms[i].localEulerAngles.z + angleAboutZ);
            }
            else
            {
                allTargetTransforms[i].position = allFootTransforms[i].position;
                allTargetTransforms[i].rotation = allFootTransforms[i].rotation;
            }
        }
    }
}
