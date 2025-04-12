using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

//[RequireComponent(typeof(LineRenderer))]
public class Cable : MonoBehaviour
{
    [NonSerialized]
    public List<Transform> pointsTransform;
    [NonSerialized]
    public Vector3[] points;
    [NonSerialized]
    public int lastRestrictedPoint = 0;

    public bool connected;
    public bool selected;

    [SerializeField]
    private Transform cableEnd;
    [SerializeField]
    private Transform connectionPoint;
    [SerializeField]
    private Transform jointsParent;
    [SerializeField]
    private Transform aboveOverlapRenderersParent;
    [SerializeField]
    private float minDistanceToConnect = 1f;
    [SerializeField]
    private float pullOutForce = 3f;
    [SerializeField]
    private float connectionMovementDelay = 0.5f;
    [SerializeField]
    public float distanceBetweenJoints = 0.2f;
    [SerializeField] 
    private float width = 0.2f;
    [SerializeField]
    private float speed = 15f;
    [SerializeField]
    private float stiffness = 10f;
    [SerializeField]
    private float velocityDamping = 0.7f;
    [SerializeField]
    private float angularVelocityDamping = 0.5f;
    [SerializeField]
    public int jointsCount = 10;
    [SerializeField]
    private float splineAccuracy = 0.01f;
    [SerializeField] 
    private GameObject jointPrefab;
    [SerializeField] 
    private GameObject anchorPrefab;
    [SerializeField]
    private GameObject overlapLineRendererPrefab;

    private LineRenderer lineRenderer;
    private NativeSpline initSpline;
    private List<Rigidbody2D> pointsRigidBodies;
    private Rigidbody2D lastPointRigidBody;
    private List<LineRenderer> aboveOverlapLineRenderers = new List<LineRenderer>();
    private int aboveOverlapLineRenderersUsed = 0;
    private List<int> currentBelowOverlapPoints = new List<int>();
    private float lastConnectionTime = -1;
    private float lastDisconnectionTime = -1;
    private float lastInputTime = -1f;
    private Vector2 inputVector = Vector2.zero;

    void Start()
    {
        InitSplineCable();
        if (connected)
        {
            Connect();
        }
    }

    private void InitSplineCable()
    {
        initSpline = new NativeSpline(GetComponent<SplineContainer>().Spline);
        pointsTransform = new List<Transform>();
        pointsRigidBodies = new List<Rigidbody2D>();
        pointsTransform.Add(Instantiate(anchorPrefab, initSpline.EvaluatePosition(0), new Quaternion(0, 0, 0, 0), jointsParent).transform);
        Vector3 startTangent = initSpline.EvaluateTangent(0);
        startTangent.Normalize();
        float currentT = 0;
        float newT = 0;
        Vector3 lastPointPosition = initSpline.EvaluatePosition(0);
        Vector3 newPointPosition;
        jointsCount = 1;
        JointAngleLimits2D angleLimits = new JointAngleLimits2D();
        angleLimits.min = -180f / stiffness;
        angleLimits.max = 180f / stiffness;
        while (currentT < 1 && jointsCount < 10000)
        {
            newT = currentT;
            while (Vector3.Distance(lastPointPosition, initSpline.EvaluatePosition(newT)) < distanceBetweenJoints && newT < 1)
            {
                newT += splineAccuracy;
            }
            float binLow = currentT;
            float binHigh = newT;
            float binMid;
            for (int i = 0; i < 10; i++)
            {
                binMid = (binLow + binHigh) / 2;
                if (Vector3.Distance(lastPointPosition, initSpline.EvaluatePosition(binMid)) < distanceBetweenJoints)
                {
                    binLow = binMid;
                }
                else
                {
                    binHigh = binMid;
                }
            }
            newT = binLow;
            newPointPosition = initSpline.EvaluatePosition(newT);

            pointsTransform.Add(Instantiate(
                jointPrefab,
                newPointPosition,
                new Quaternion(0, 0, 0, 0),
                //Quaternion.FromToRotation(Vector3.up, lastPointPosition - newPointPosition),
                jointsParent
                ).transform);
            pointsTransform.Last().name = "Joint " + jointsCount.ToString();
            pointsRigidBodies.Add(pointsTransform.Last().GetComponent<Rigidbody2D>());
            HingeJoint2D hingeJoint = pointsTransform.Last().GetComponent<HingeJoint2D>();
            hingeJoint.limits = angleLimits;
            hingeJoint.connectedBody = pointsTransform[^2].GetComponent<Rigidbody2D>();
            pointsTransform[^2].GetComponent<Rigidbody2D>().rotation = (Quaternion.LookRotation(Vector3.forward, -pointsTransform[^1].position + pointsTransform[^2].position).eulerAngles).z;
            //hingeJoint.connectedAnchor = startTangent * Vector3.Distance(newPointPosition, lastPointPosition);
            hingeJoint.connectedAnchor = Vector2.down * Vector3.Distance(newPointPosition, lastPointPosition);
            
            jointsCount++;
            currentT = newT;
            lastPointPosition = newPointPosition;
        }

        foreach (Rigidbody2D rigidbody in pointsRigidBodies)
        {
            rigidbody.GetComponent<HingeJoint2D>().enabled = true;
        }

        lastPointRigidBody = pointsTransform.Last().GetComponent<Rigidbody2D>();

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.widthMultiplier = width;
        points = new Vector3[jointsCount + 1];
        lineRenderer.positionCount = jointsCount + 1;
        UpdateLineRenderer();
    }

    private void InitLineCable()
    {
        pointsTransform = new List<Transform>(jointsCount);
        pointsRigidBodies = new List<Rigidbody2D>();
        pointsTransform.Add(Instantiate(anchorPrefab, transform.position, new Quaternion(0, 0, 0, 0), jointsParent).transform);
        for (int i = 1; i < jointsCount; i++)
        {
            pointsTransform.Add(Instantiate(
                jointPrefab,
                transform.position + Vector3.down * distanceBetweenJoints * i,
                new Quaternion(0, 0, 0, 0), jointsParent
                ).transform);
            pointsRigidBodies.Add(pointsTransform.Last().GetComponent<Rigidbody2D>());
            HingeJoint2D hingeJoint = pointsTransform.Last().GetComponent<HingeJoint2D>();
            JointAngleLimits2D angleLimits = new JointAngleLimits2D();
            angleLimits.min = -180f / stiffness;
            angleLimits.max = 180f / stiffness;
            hingeJoint.limits = angleLimits;
            hingeJoint.connectedBody = pointsTransform[i - 1].GetComponent<Rigidbody2D>();
            hingeJoint.connectedAnchor = Vector2.down * distanceBetweenJoints;
            //hingeJoint.anchor = Vector2.down * distanceBetweenJoints;
        }
        lastPointRigidBody = pointsTransform.Last().GetComponent<Rigidbody2D>();

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.widthMultiplier = width;
        points = new Vector3[jointsCount + 1];
        lineRenderer.positionCount = jointsCount + 1;
        UpdateLineRenderer();
    }

    void Update()
    {
        if (lastInputTime + 0.1f > Time.time)
        {
            lastPointRigidBody.velocity *= velocityDamping;
        }
        foreach (Rigidbody2D pointRigiBody in pointsRigidBodies)
        {
            pointRigiBody.angularVelocity *= angularVelocityDamping;
            if (pointRigiBody != lastPointRigidBody)
            {
                pointRigiBody.velocity *= velocityDamping;
            }
        }
        if (connected)
        {
            lastPointRigidBody.position = connectionPoint.position;
            lastPointRigidBody.rotation = connectionPoint.rotation.eulerAngles.z;
            lastPointRigidBody.velocity  = Vector3.zero;
            lastPointRigidBody.angularVelocity = 0;
            cableEnd.rotation = connectionPoint.rotation;
            cableEnd.position = lastPointRigidBody.position;
        }
        else
        {
            cableEnd.position = Vector3.Lerp(lastPointRigidBody.position, pointsTransform[^1].position, 0.2f);
            cableEnd.rotation = Quaternion.Lerp(cableEnd.rotation, pointsTransform[^2].rotation, 0.1f);
        }
        CheckForSuccesfullConnection();
        UpdateLineRenderer();
    }

    private void FixedUpdate()
    {
        if (!connected && selected)
        {
            lastPointRigidBody.velocity += inputVector * speed;
        }
    }

    public void Connect()
    {
        lastPointRigidBody.position = connectionPoint.position;
        lastPointRigidBody.rotation = connectionPoint.rotation.eulerAngles.z;
        lastPointRigidBody.velocity = Vector2.zero;
        lastPointRigidBody.angularVelocity = 0;
        lastPointRigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        lastPointRigidBody.isKinematic = true;
        connected = true;
        lastConnectionTime = Time.time;
    }

    public void Disconnect()
    {
        lastPointRigidBody.velocity = connectionPoint.up * pullOutForce;
        lastPointRigidBody.isKinematic = false;
        lastPointRigidBody.constraints = RigidbodyConstraints2D.None;
        connected = false;
        lastDisconnectionTime = Time.time;
    }

    public void SetMovementVector(Vector2 movementVector)
    {
        if (lastConnectionTime + connectionMovementDelay < Time.time)
        {
            lastInputTime = Time.time;
            if (connected && movementVector != Vector2.zero)
            {
                Disconnect();
            }
            else if (!connected)
            {
                inputVector = movementVector;
            }
        }
    }

    private void UpdateLineRenderer(bool firstFrame = false)
    {
        for (int i = 0; i < jointsCount; i++)
        {
            points[i] = pointsTransform[i].position;
        }
        points[jointsCount] = cableEnd.position;
        lineRenderer.SetPositions(points);
    }

    private void CheckForSuccesfullConnection()
    {
        if (lastDisconnectionTime + connectionMovementDelay < Time.time &&
            !connected &&
            Vector2.Distance(pointsTransform.Last().position, connectionPoint.position) < minDistanceToConnect)
        {
            Connect();
        }
    }

    public void SetNewAboveOverlap(int pointIndex, int orderInLayer)
    {
        if (aboveOverlapLineRenderersUsed >= aboveOverlapLineRenderers.Count)
        {
            aboveOverlapLineRenderers.Add(Instantiate(overlapLineRendererPrefab, aboveOverlapRenderersParent).GetComponent<LineRenderer>());
            aboveOverlapLineRenderers.Last().colorGradient = lineRenderer.colorGradient;
            aboveOverlapLineRenderers.Last().widthMultiplier = lineRenderer.widthMultiplier;
            aboveOverlapLineRenderers.Last().material = lineRenderer.material;
            aboveOverlapLineRenderers.Last().numCornerVertices = lineRenderer.numCornerVertices;
        }
        List<Vector3> positions = new List<Vector3>();
        if (pointIndex + 1 < points.Length)
        {
            positions.Add(points[pointIndex + 1]);
        }
        positions.Add(points[pointIndex]);
        positions.Add(points[pointIndex - 1]);
        if (pointIndex - 2 >= 0)
        {
            positions.Add(points[pointIndex - 2]);
        }
        aboveOverlapLineRenderers[aboveOverlapLineRenderersUsed].positionCount = positions.Count;
        aboveOverlapLineRenderers[aboveOverlapLineRenderersUsed].SetPositions(positions.ToArray());
        aboveOverlapLineRenderers[aboveOverlapLineRenderersUsed].sortingOrder = orderInLayer;
        aboveOverlapLineRenderersUsed++;
    }

    public void SetNewBelowOverlap(int pointIndex)
    {
        lastRestrictedPoint = Mathf.Max(lastRestrictedPoint, pointIndex);
    }

    public void ResetOverlaps()
    {
        lastRestrictedPoint = 0;
        foreach(LineRenderer aboveOverlapLineRenderer in aboveOverlapLineRenderers)
        {
            aboveOverlapLineRenderer.positionCount = 0;
            aboveOverlapLineRenderer.SetPositions(new Vector3[0] { });
        }
        if (selected)
        {
            foreach (Rigidbody2D rigidbody in pointsRigidBodies)
            {
                rigidbody.isKinematic = false;
                rigidbody.constraints = RigidbodyConstraints2D.None;
            }
        }
        aboveOverlapLineRenderersUsed = 0;
        currentBelowOverlapPoints.Clear();
    }

    public void Deselect()
    {
        selected = false;
        for(int i=0; i<jointsCount-1; i++)
        {
            RestrictPoint(i);
        }
    }

    public void Select()
    {
        selected = true;
    }

    public void UpdateOverlaps()
    {
        if (selected)
        {
            for (int i = 0; i <= pointsRigidBodies.Count; i++)
            {
                if (i <= lastRestrictedPoint)
                {
                    RestrictPoint(i);
                }
                else
                {
                    RemoveRestrictionInPoint(i);
                }
            }
        }
    }

    private void RemoveRestrictionInPoint(int pointIndex)
    {
        if (pointIndex < 0 || pointIndex >= pointsRigidBodies.Count)
        {
            return;
        }
        pointsRigidBodies[pointIndex].constraints = RigidbodyConstraints2D.None;
        pointsRigidBodies[pointIndex].isKinematic = false;
    }

    private void RestrictPoint(int pointIndex)
    {
        if (pointIndex < 0 || pointIndex >= pointsRigidBodies.Count)
        {
            return;
        }
        pointsRigidBodies[pointIndex].velocity = Vector3.zero;
        pointsRigidBodies[pointIndex].angularVelocity = 0;
        pointsRigidBodies[pointIndex].constraints = RigidbodyConstraints2D.FreezeAll;
        pointsRigidBodies[pointIndex].isKinematic = true;
    }

    //public void RestrictPoint(int pointIndex)
    //{
    //    pointsRigidBodies[pointIndex - 1].velocity = Vector3.zero;
    //    pointsRigidBodies[pointIndex - 1].angularVelocity = 0;
    //    pointsRigidBodies[pointIndex - 1].constraints = RigidbodyConstraints2D.FreezeAll;
    //    pointsRigidBodies[pointIndex - 1].isKinematic = true;
    //}

    //public void ReleaseRestrictions()
    //{
    //    foreach (Rigidbody2D pointRigidbody2D in pointsRigidBodies)
    //    {
    //        pointRigidbody2D.constraints = RigidbodyConstraints2D.None;
    //        pointRigidbody2D.isKinematic = false;
    //    }
    //}

    //public void ApplyFrictionInPoint(int pointIndex, float friction)
    //{
    //    if (pointIndex == 0)
    //    {
    //        return;
    //    }
    //    //Debug.Log(pointsRigidBodies[pointIndex - 1].angularVelocity);
    //    pointsRigidBodies[pointIndex - 1].velocity *= friction;
    //    pointsRigidBodies[pointIndex - 1].angularVelocity *= friction;
    //    //Debug.Log(pointsRigidBodies[pointIndex - 1].velocity);
    //}

    //public void ApplyForceInPoint(int pointIndex, Vector2 forceVector)
    //{
    //    if (pointIndex == 0)
    //    { 
    //        return;
    //    }
    //    pointsRigidBodies[pointIndex - 1].velocity = forceVector;
    //}

    //public void ReppelPoints(int pointIndex1, int pointIndex2, Vector3 forceVector)
    //{
    //    int startIndex = Math.Min(pointIndex1, pointIndex2);
    //    int endIndex = Math.Max(pointIndex1, pointIndex2) + 1;
    //    for (int i = startIndex; i < endIndex; i++)
    //    {
    //        ApplyForceInPoint(i, forceVector);
    //    }
    //}

    //public Vector2 GetCenterOfMassBetweenPoints(int pointIndex1, int pointIndex2)
    //{
    //    Vector3 positionsSum = Vector3.zero;
    //    int startIndex = Math.Min(pointIndex1, pointIndex2);
    //    int endIndex = Math.Max(pointIndex1, pointIndex2) + 1;
    //    for (int i = startIndex; i < endIndex; i++)
    //    {
    //        positionsSum += points[i];
    //    }
    //    return positionsSum / (pointIndex2 - pointIndex1 + 1);
    //}

    //public Vector2 GetMaxDistantPointFromLine(int pointIndex1, int pointIndex2, LineEquasion lineEquasion, out float maxDistance)
    //{
    //    Vector2 point = Vector2.zero;
    //    maxDistance = -1;
    //    int startIndex = Math.Min(pointIndex1, pointIndex2);
    //    int endIndex = Math.Max(pointIndex1, pointIndex2) + 1;
    //    for (int i = startIndex; i < endIndex; i++)
    //    {
    //        float distance = lineEquasion.DistanceToPoint(points[i]);
    //        if (maxDistance < distance)
    //        {
    //            maxDistance = distance;
    //            point = points[i];
    //        }
    //    }
    //    return point;
    //}
}
