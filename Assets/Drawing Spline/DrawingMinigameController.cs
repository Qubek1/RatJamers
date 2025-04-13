using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class DrawingMinigameController : MinigameController
{
    public float maxMadeError = 0;

    [SerializeField]
    private float maxPossibleError = 1f;
    [SerializeField]
    private float movementSpeed = 0.1f;
    [SerializeField]
    private float velocityDamping;
    [SerializeField]
    private Rigidbody2D pen;

    [SerializeField] private float m_winConditionPathFill = 0.95f;


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

    private LineRenderer lineRenderer;
    private NativeSpline nativeSpline;
    //private InputActions inputActions;
    private InputAction move;

    private Vector2 lastFramePosition;
    private Vector2 currentFramePosition;

    private List<Vector2> gizmosPoints = new List<Vector2>();

    private List<Vector3> lineRendererPoints = new List<Vector3>();

    private void Update()
    {
        gizmosPoints.Clear();
        //Debug.Log(move.ReadValue<Vector2>());
        pen.velocity += move.ReadValue<Vector2>() * movementSpeed * Time.deltaTime;
        pen.velocity *= velocityDamping;
        currentFramePosition = pen.position;
        float newT = FindAndSetNewT(currentFramePosition);
        if (newT > currentT && Vector2.Distance(PositionOnSpline(newT), pen.position) < maxPossibleError)
        {
            currentT = newT;
            maxMadeError = Mathf.Max(maxMadeError,Vector2.Distance(PositionOnSpline(currentT), currentFramePosition));
            lineRendererPoints.Add(pen.position);
            lineRenderer.positionCount++;
            lineRenderer.SetPositions(lineRendererPoints.ToArray());
            //followingLineRender.Range = new Vector2(0, Mathf.Max(0.01f, currentT));
            //followingLineRender.Rebuild();
        }
        lastFramePosition = currentFramePosition;
        if (IsCompleted())
        {
            MinigameLeft();
        }
    }

    private float FindAndSetNewT(Vector2 point)
    {
        float previousT = currentT;
        float previousDistance = Vector2.Distance(PositionOnSpline(currentT), point);
        float nextT = currentT + splineSearchAccuracy;
        float nextDistance = Vector2.Distance(PositionOnSpline(nextT), point);
        while (previousDistance > nextDistance && nextDistance < maxPossibleError && nextT < 1)
        {
            gizmosPoints.Add(PositionOnSpline(nextT));
            previousT = nextT;
            previousDistance = nextDistance;
            nextT += splineSearchAccuracy;
            nextDistance = Vector2.Distance(PositionOnSpline(nextT), point);
        }
        nextT = Mathf.Min(1, nextT);
        nextDistance = Vector2.Distance(PositionOnSpline(nextT), point);
        gizmosPoints.Add(PositionOnSpline(nextT));
        previousT = previousT - splineSearchAccuracy;
        previousDistance = Vector2.Distance(PositionOnSpline(previousT), point);
        for(int i=0; i<10; i++)
        {
            float binSearchMidPoint = (previousT + nextT) / 2f; 
            gizmosPoints.Add(PositionOnSpline(binSearchMidPoint));
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

    public override void Launch(int launchingPlayer,int onPlayerSide)
    {
        base.Launch(launchingPlayer,onPlayerSide);
        gameObject.SetActive(true);
        move=
            PlayerController.GetPlayer(launchingPlayer).PlayerInput.actions.FindActionMap("UI").FindAction("Move");
        nativeSpline.Dispose();
        nativeSpline = new NativeSpline(splineContainer.Spline, Unity.Collections.Allocator.Persistent);
        currentT = 0;
        pen.position = PositionOnSpline(0);
        lineRenderer = GetComponent<LineRenderer>();
        lineRendererPoints.Add(pen.position);
        lineRenderer.positionCount = 1;
        lineRenderer.SetPositions(lineRendererPoints.ToArray());
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < gizmosPoints.Count; i++)
        {
            Gizmos.color = gizmosGradient.Evaluate(((float)i) / gizmosPoints.Count);
            Gizmos.DrawWireSphere(gizmosPoints[i], 2f);
        }
    }

    /*
    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
*/
    
    private Vector2 PositionOnSpline(float t) => Float3ToVector2(splineTransform.TransformPoint(nativeSpline.EvaluatePosition(t)));

    private Vector2 Float3ToVector2(float3 v) => new Vector2(v.x, v.y);
    public override void Hide()
    {
        gameObject.SetActive(false);
    }

    public override bool IsCompleted()
    {
        return currentT > m_winConditionPathFill;
    }
}
