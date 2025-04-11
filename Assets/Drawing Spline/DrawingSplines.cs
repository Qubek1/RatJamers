using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class DrawingSplines : MonoBehaviour
{
    public float maxMadeError = 0;

    [SerializeField]
    private float maxPossibleError = 1f;
    [SerializeField]
    private float movementSpeed = 0.1f;
    [SerializeField]
    private Transform pen;

    [SerializeField]
    private SplineContainer splineContainer;
    [SerializeField]
    private Transform splineTransform;
    [SerializeField]
    private SplineExtrude followingLineRender;
    [SerializeField]
    private float splineSearchAccuracy = 0.005f;
    [SerializeField]
    private float currentT;
    [SerializeField]
    private Gradient gizmosGradient;

    private NativeSpline nativeSpline;
    private InputActions inputActions;
    private InputAction move;

    private Vector2 lastFramePosition;
    private Vector2 currentFramePosition;

    private List<Vector2> pointsToDraw = new List<Vector2>();

    private void Awake()
    {
        inputActions = new InputActions();
        inputActions.Enable();
        move = inputActions.Player.Move;
        nativeSpline = new NativeSpline(splineContainer.Spline, Unity.Collections.Allocator.Persistent);
        currentT = 0;
        pen.position = PositionOnSpline(0);
        followingLineRender.Range = new Vector2(0, 0.01f);
    }

    private void Update()
    {
        pointsToDraw.Clear();
        Debug.Log(move.ReadValue<Vector2>());
        pen.position += (move.ReadValue<Vector2>().x * movementSpeed * Vector3.right + move.ReadValue<Vector2>().y * movementSpeed * Vector3.up) * Time.deltaTime;
        currentFramePosition = pen.position;
        float newT = FindAndSetNewT(currentFramePosition);
        if (newT > currentT)
        {
            currentT = newT;
            maxMadeError = Mathf.Max(maxMadeError,Vector2.Distance(PositionOnSpline(currentT), currentFramePosition));
            followingLineRender.Range = new Vector2(0, Mathf.Max(0.01f, currentT));
            followingLineRender.Rebuild();
        }
        lastFramePosition = currentFramePosition;
    }

    private float FindAndSetNewT(Vector2 point)
    {
        float previousT = currentT;
        float previousDistance = Vector2.Distance(PositionOnSpline(currentT), point);
        float nextT = currentT + splineSearchAccuracy;
        float nextDistance = Vector2.Distance(PositionOnSpline(nextT), point);
        while (previousDistance > nextDistance && nextDistance < maxPossibleError && nextT < 1)
        {
            pointsToDraw.Add(PositionOnSpline(nextT));
            previousT = nextT;
            previousDistance = nextDistance;
            nextT += splineSearchAccuracy;
            nextDistance = Vector2.Distance(PositionOnSpline(nextT), point);
        }
        nextT = Mathf.Min(1, nextT);
        nextDistance = Vector2.Distance(PositionOnSpline(nextT), point);
        pointsToDraw.Add(PositionOnSpline(nextT));
        previousT = previousT - splineSearchAccuracy;
        previousDistance = Vector2.Distance(PositionOnSpline(previousT), point);
        for(int i=0; i<10; i++)
        {
            float binSearchMidPoint = (previousT + nextT) / 2f; 
            pointsToDraw.Add(PositionOnSpline(binSearchMidPoint));
            if (previousDistance > nextDistance)
            {
                previousT = binSearchMidPoint;
                previousDistance = Vector2.Distance(PositionOnSpline(previousT), point);
            }
            else
            {
                nextT = binSearchMidPoint;
                nextDistance = Vector2.Distance(PositionOnSpline(nextT), point);
            }
        }
        return previousT;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < pointsToDraw.Count; i++)
        {
            Gizmos.color = gizmosGradient.Evaluate(((float)i) / pointsToDraw.Count);
            Gizmos.DrawWireSphere(pointsToDraw[i], 0.1f);
        }
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private Vector2 PositionOnSpline(float t) => Float3ToVector2(splineTransform.TransformPoint(nativeSpline.EvaluatePosition(t)));

    private Vector2 Float3ToVector2(float3 v) => new Vector2(v.x, v.y);
}
