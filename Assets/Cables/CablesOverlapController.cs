using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class CablesOverlapController : MonoBehaviour
{
    public List<Cable> cables;

    [NonSerialized]
    public Cable interactedCable;

    [SerializeField]
    private float minX = -5;
    [SerializeField] 
    private float maxX = 5;
    [SerializeField]
    private float minY = -5;
    [SerializeField] 
    private float maxY = 5;
    //[SerializeField]
    //private float frictionDistance = 2f;
    //[SerializeField]
    //private float frictionStrength = 10f;
    //[SerializeField]
    //private float repulsionDistance = 0.5f;
    //[SerializeField]
    //private float repulsionStrenght = 10f;
    [SerializeField]
    private float gridCellSize = 5f;
    [SerializeField]
    private float maxOverlapPointMovement = 0.2f;
    [SerializeField]
    private List<OverlapPoint> overlapPoints = new List<OverlapPoint>(50);
    [SerializeField]
    private List<OverlapPoint> overlapPointsFound = new List<OverlapPoint>(50);
    [SerializeField]
    private Gradient gizmosGradient;

    private List<PointInCable>[,] pointsInGrid;
    private int currentMaxOrderInLayer = 1;
    private List<PointInCable> nearPoints = new List<PointInCable>(100);

    private List<Vector2> pointsToDraw = new List<Vector2>();

    private void Awake()
    {
        int gridXSize = Mathf.CeilToInt((maxX - minX) / gridCellSize + 2);
        int gridYSize = Mathf.CeilToInt((maxY - minY) / gridCellSize + 2);
        pointsInGrid = new List<PointInCable>[gridXSize, gridYSize];
        for (int x = 0; x < gridXSize; x++)
        {
            for (int y = 0; y < gridYSize; y++)
            {
                pointsInGrid[x, y] = new List<PointInCable>(5);
            }
        }
        //Debug.Log(GetLineEquasionFromPoints(
        //        new Vector2(1, 3),
        //        new Vector2(-2, 0)));
        //Debug.Log(GetLineEquasionFromPoints(
        //        new Vector2(2, -1),
        //        new Vector2(-2, 3)));
        //Debug.Log(FindLinesIntersection(
        //    GetLineEquasionFromPoints(
        //        new Vector2(1, 3),
        //        new Vector2(-2, 0)),
        //    GetLineEquasionFromPoints(
        //        new Vector2(2, -1),
        //        new Vector2(-2, 3))));
        //Debug.Log(CrossingPointsCheck(
        //    new Vector2(2, -1),
        //    new Vector2(-2, 3),
        //    new Vector2(-2, 0),
        //    new Vector2(1, 3)
        //    ));
    }

    // Update is called once per frame
    void Update()
    {
        ResetOverlaps();
        FindNewOverlapsWithGrid();
        AssignNewOverlaps();
        SetOverlapsInCables();
        UpdateOverlapsInCables();
    }

    private Vector2Int GetGridPosition(Vector2 position)
    {
        int x = Mathf.Clamp(Mathf.FloorToInt((position.x - (minX - gridCellSize)) / gridCellSize), 1, pointsInGrid.GetLength(0) - 2);
        int y = Mathf.Clamp(Mathf.FloorToInt((position.y - (minY - gridCellSize)) / gridCellSize), 1, pointsInGrid.GetLength(1) - 2);
        return new Vector2Int(x, y);
    }

    private void AddPointToGrid(PointInCable pointInCable)
    {
        Vector2Int gridPosition = GetGridPosition(pointInCable.GetPosition());
        pointsInGrid[gridPosition.x, gridPosition.y].Add(pointInCable);
    }

    private void AddNearPointsToList(Vector2 position, List<PointInCable> list)
    {
        Vector2Int gridPosition = GetGridPosition(position);
        for (int x=gridPosition.x - 1; x <= gridPosition.x + 1; x++)
        {
            for (int y=gridPosition.y - 1; y<=gridPosition.y + 1; y++)
            {
                list.AddRange(pointsInGrid[x, y]);
            }
        }
    }

    private void FindNewOverlapsWithGrid()
    {
        foreach(List<PointInCable> pointInCableList in pointsInGrid)
        {
            pointInCableList.Clear();
        }
        for(int ci=0; ci<cables.Count; ci++)
        {
            for(int pi=1; pi < cables[ci].points.Length; pi++)
            {
                AddPointToGrid(new PointInCable(pi, ci, cables[ci]));
            }
        }
        overlapPointsFound.Clear();
        for (int ci = 0; ci < cables.Count; ci++)
        {
            for (int pi = 1; pi < cables[ci].points.Length; pi++)
            {
                nearPoints.Clear();
                LineEquasion iLineEquasion = GetLineEquasionFromPoints(cables[ci].points[pi], cables[ci].points[pi - 1]);
                AddNearPointsToList(cables[ci].points[pi], nearPoints);
                foreach (PointInCable pointInCable in nearPoints)
                {
                    if (pointInCable.cableIndex <= ci)
                    {
                        continue;
                    }
                    if (CrossingPointsCheck(
                        cables[ci].points[pi], cables[ci].points[pi - 1],
                        pointInCable.GetPosition(), pointInCable.GetPreviousPosition()))
                    {
                        LineEquasion cablePointLineEquasion = GetLineEquasionFromPoints(pointInCable.GetPosition(), pointInCable.GetPreviousPosition());
                        overlapPointsFound.Add(new OverlapPoint(
                            FindLinesIntersection(iLineEquasion, cablePointLineEquasion),
                            cables[ci], pointInCable.cable, pi, pointInCable.pointIndex, 0));
                    }
                }
            }
        }
    }

    //private void FindNewOverlaps()
    //{
    //    overlapPointsFound.Clear();
    //    for(int ci=0; ci<cables.Count; ci++)
    //    {
    //        for (int cj = ci + 1; cj < cables.Count; cj++)
    //        {
    //            //Debug.Log(ci);
    //            //Debug.Log(cj);
    //            for (int pi = 1; pi < cables[ci].jointsCount; pi++)
    //            {
    //                LineEquasion iLineEquasion = GetLineEquasionFromPoints(cables[ci].points[pi], cables[ci].points[pi - 1]);
    //                for (int pj = 1; pj < cables[cj].jointsCount; pj++)
    //                {
    //                    if (CrossingPointsCheck(cables[ci].points[pi], cables[ci].points[pi-1], cables[cj].points[pj], cables[cj].points[pj - 1]))
    //                    {
    //                        LineEquasion jLineEquasion = GetLineEquasionFromPoints(cables[cj].points[pj], cables[cj].points[pj - 1]);
    //                        overlapPointsFound.Add(
    //                            new OverlapPoint(FindLinesIntersection(iLineEquasion, jLineEquasion), cables[ci], cables[cj], pi, pj, 0));
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}

    private void AssignNewOverlaps()
    {
        List<OverlapPoint> newOverlapPoints = new List<OverlapPoint>(50);
        var oldPointsFoundPointsPairs = new List<Tuple<OverlapPoint, OverlapPoint, float>>(overlapPoints.Count * overlapPointsFound.Count);
        foreach (OverlapPoint oldPoint in overlapPoints)
        {
            foreach (OverlapPoint foundPoint in overlapPointsFound)
            {
                oldPointsFoundPointsPairs.Add(new Tuple<OverlapPoint, OverlapPoint, float>(
                    oldPoint, 
                    foundPoint, 
                    Vector3.Distance(oldPoint.position, foundPoint.position
                    )));
            }
        }
        var sortedPointsPairs = oldPointsFoundPointsPairs.OrderBy(pointsPair => pointsPair.Item3);
        foreach (var pointsPair in sortedPointsPairs)
        {
            if (overlapPoints.Contains(pointsPair.Item1) && overlapPointsFound.Contains(pointsPair.Item2) &&
                pointsPair.Item3 < maxOverlapPointMovement &&
                pointsPair.Item1.SameCables(pointsPair.Item2))
            {
                OverlapPoint newPoint = pointsPair.Item2;
                if (!newPoint.SameCablesAndOrderAs(pointsPair.Item1))
                {
                    newPoint = newPoint.Reversed();
                }
                newPoint.orderInLayer = pointsPair.Item1.orderInLayer;
                newOverlapPoints.Add(newPoint);
                overlapPoints.Remove(pointsPair.Item1);
                overlapPointsFound.Remove(pointsPair.Item2);
            }
        }
        //foreach (OverlapPoint oldPoint in overlapPoints)
        //{
        //    OverlapPoint closestPoint = new OverlapPoint(new Vector2(-100000, -100000), null, null, -1, -1, 0);
        //    foreach (OverlapPoint newPoint in overlapPointsFound)
        //    {
        //        if (newPoint.SameCables(oldPoint) && Vector2.Distance(closestPoint.position, oldPoint.position) > Vector2.Distance(newPoint.position, oldPoint.position))
        //        {
        //            closestPoint = newPoint;
        //        }
        //    }
        //    if (Vector2.Distance(closestPoint.position, oldPoint.position) < maxOverlapPointMovement)
        //    {
        //        overlapPointsFound.Remove(closestPoint);
        //        if (!closestPoint.SameCablesAndOrderAs(oldPoint))
        //        {
        //            closestPoint = closestPoint.Reversed();
        //        }
        //        closestPoint.orderInLayer = oldPoint.orderInLayer;
        //        newOverlapPoints.Add(closestPoint);
        //    }
        //}
        foreach (OverlapPoint newPoint in overlapPointsFound)
        {
            //Debug.Log("new overlap!");
            OverlapPoint newPointWithUpdatedOrder = newPoint.Copy();
            newPointWithUpdatedOrder.orderInLayer = currentMaxOrderInLayer;
            currentMaxOrderInLayer++;
            if (newPoint.cableBelow == interactedCable)
            {
                newOverlapPoints.Add(newPointWithUpdatedOrder.Reversed());
            }
            else
            {
                newOverlapPoints.Add(newPointWithUpdatedOrder);
            }
        }
        overlapPoints = newOverlapPoints;
    }

    private void SetOverlapsInCables()
    {
        foreach(OverlapPoint pointToMove in overlapPoints)
        {
            pointToMove.cableAbove.SetNewAboveOverlap(pointToMove.cableAbovePointIndex, pointToMove.orderInLayer);
            pointToMove.cableBelow.SetNewBelowOverlap(pointToMove.cableBelowPointIndex);
        }
    }

    private void UpdateOverlapsInCables()
    {
        foreach (Cable cable in cables)
        {
            cable.UpdateOverlaps();
        }
    }

    private void ResetOverlaps()
    {
        foreach(Cable cable in cables)
        {
            cable.ResetOverlaps();
        }
    }

    //private void HandleOverlapPhysics()
    //{
    //    foreach(OverlapPoint overlapPoint in overlapPoints)
    //    {
    //        overlapPoint.cableBelow.ApplyFrictionInPoint(overlapPoint.cableBelowPointIndex, 1 / frictionStrength);
    //        overlapPoint.cableBelow.RestrictPoint(overlapPoint.cableBelowPointIndex);
    //    }
    //    foreach(OverlapPoint overlapPoint in overlapPoints)
    //    { 
    //        foreach(OverlapPoint anotherOverlapPoint in overlapPoints)
    //        {
    //            if (overlapPoint.position == anotherOverlapPoint.position)
    //            {
    //                continue;
    //            }
    //            if (Vector2.Distance(overlapPoint.position, anotherOverlapPoint.position) < repulsionDistance)
    //            {
    //                Vector2 forceVector = Vector2.zero;
    //                if (overlapPoint.cableBelow == anotherOverlapPoint.cableAbove && overlapPoint.cableAbove == anotherOverlapPoint.cableBelow)
    //                {
    //                    Vector2 center = (overlapPoint.position + anotherOverlapPoint.position) / 2f;
    //                    Cable cable1 = overlapPoint.cableAbove;
    //                    Cable cable2 = overlapPoint.cableBelow;
    //                    float cable1MaxDistance, cable2MaxDistance;
    //                    LineEquasion lineBetweenOverlaps = GetLineEquasionFromPoints(overlapPoint.position, anotherOverlapPoint.position);
    //                    Vector2 centerOfMass1 = cable1.GetMaxDistantPointFromLine(
    //                        overlapPoint.cableAbovePointIndex,
    //                        anotherOverlapPoint.cableBelowPointIndex,
    //                        lineBetweenOverlaps,
    //                        out cable1MaxDistance);
    //                    Vector2 centerOfMass2 = cable2.GetMaxDistantPointFromLine(
    //                        overlapPoint.cableBelowPointIndex,
    //                        anotherOverlapPoint.cableAbovePointIndex,
    //                        lineBetweenOverlaps,
    //                        out cable2MaxDistance);
    //                    pointsToDraw.Add(centerOfMass1);
    //                    pointsToDraw.Add(centerOfMass2);
    //                    bool cable1Side = VM(overlapPoint.position, anotherOverlapPoint.position, centerOfMass1);
    //                    bool cable2Side = VM(overlapPoint.position, anotherOverlapPoint.position, centerOfMass2);
    //                    if (cable1Side == cable2Side)
    //                    {
    //                        Debug.Log("same side");
    //                        forceVector = RotateRight(overlapPoint.position - anotherOverlapPoint.position).normalized * repulsionStrenght / 
    //                            (Vector2.Distance(overlapPoint.position, anotherOverlapPoint.position));
    //                        Debug.Log(forceVector);
    //                        Debug.Log(cable1MaxDistance);
    //                        Debug.Log(cable2MaxDistance);
    //                        if (cable1MaxDistance > cable2MaxDistance == cable1Side)
    //                        {
    //                            cable1.ReppelPoints(overlapPoint.cableAbovePointIndex, anotherOverlapPoint.cableBelowPointIndex, -forceVector);
    //                            cable2.ReppelPoints(overlapPoint.cableBelowPointIndex, anotherOverlapPoint.cableAbovePointIndex, forceVector);
    //                        }
    //                        else
    //                        {
    //                            cable1.ReppelPoints(overlapPoint.cableAbovePointIndex, anotherOverlapPoint.cableBelowPointIndex, forceVector);
    //                            cable2.ReppelPoints(overlapPoint.cableBelowPointIndex, anotherOverlapPoint.cableAbovePointIndex, -forceVector);
    //                        }
    //                    }
    //                    else
    //                    {
    //                        Debug.Log("different side");
    //                        forceVector = RotateRight(overlapPoint.position - anotherOverlapPoint.position).normalized * repulsionStrenght /
    //                            (Vector2.Distance(overlapPoint.position, anotherOverlapPoint.position));
    //                        Debug.Log(forceVector);
    //                        Debug.Log(cable1MaxDistance);
    //                        Debug.Log(cable2MaxDistance);
    //                        if (cable1Side)
    //                        {
    //                            cable1.ReppelPoints(overlapPoint.cableAbovePointIndex, anotherOverlapPoint.cableBelowPointIndex, -forceVector);
    //                            cable2.ReppelPoints(overlapPoint.cableBelowPointIndex, anotherOverlapPoint.cableAbovePointIndex, forceVector);
    //                        }
    //                        else
    //                        {
    //                            cable1.ReppelPoints(overlapPoint.cableAbovePointIndex, anotherOverlapPoint.cableBelowPointIndex, forceVector);
    //                            cable2.ReppelPoints(overlapPoint.cableBelowPointIndex, anotherOverlapPoint.cableAbovePointIndex, -forceVector);
    //                        }
    //                        forceVector = anotherOverlapPoint.position - overlapPoint.position;
    //                        forceVector *= repulsionStrenght;
    //                        anotherOverlapPoint.cableBelow.ApplyForceInPoint(anotherOverlapPoint.cableBelowPointIndex, forceVector);
    //                        anotherOverlapPoint.cableAbove.ApplyForceInPoint(anotherOverlapPoint.cableAbovePointIndex, forceVector);
    //                        overlapPoint.cableBelow.ApplyForceInPoint(overlapPoint.cableBelowPointIndex, -forceVector);
    //                        overlapPoint.cableAbove.ApplyForceInPoint(overlapPoint.cableAbovePointIndex, -forceVector);
    //                    }
    //                }
    //                else
    //                {
    //                    forceVector = anotherOverlapPoint.position - overlapPoint.position;
    //                    forceVector *= repulsionStrenght / (Vector2.Distance(overlapPoint.position, anotherOverlapPoint.position));
    //                    anotherOverlapPoint.cableBelow.ApplyForceInPoint(anotherOverlapPoint.cableBelowPointIndex, forceVector);
    //                    anotherOverlapPoint.cableAbove.ApplyForceInPoint(anotherOverlapPoint.cableAbovePointIndex, forceVector);
    //                    overlapPoint.cableBelow.ApplyForceInPoint(overlapPoint.cableBelowPointIndex, -forceVector);
    //                    overlapPoint.cableAbove.ApplyForceInPoint(overlapPoint.cableAbovePointIndex, -forceVector);
    //                }
    //            }
    //            else if (Vector2.Distance(overlapPoint.position, anotherOverlapPoint.position) < frictionDistance)
    //            {
    //                anotherOverlapPoint.cableBelow.ApplyFrictionInPoint(anotherOverlapPoint.cableBelowPointIndex, 1 / frictionStrength);
    //                anotherOverlapPoint.cableAbove.ApplyFrictionInPoint(anotherOverlapPoint.cableAbovePointIndex, 1 / frictionStrength);
    //                overlapPoint.cableBelow.ApplyFrictionInPoint(overlapPoint.cableBelowPointIndex, 1 / frictionStrength);
    //                overlapPoint.cableAbove.ApplyFrictionInPoint(overlapPoint.cableAbovePointIndex, 1 / frictionStrength);
    //            }
    //        }
    //    }
    //}

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0), new Vector3(maxX - minX, maxY - minY, 0));
        foreach (OverlapPoint overlapPoint in overlapPoints)
        {
            if (overlapPoint.cableAbove.selected)
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawWireSphere(overlapPoint.position, 0.2f);
        }
        Gizmos.color = Color.blue;
        foreach (Vector2 pointToDraw in pointsToDraw)
        {
            Gizmos.DrawWireSphere(pointToDraw, 0.5f);
        }
        pointsToDraw.Clear();
    }

    private Vector2 FindLinesIntersection(LineEquasion line1, LineEquasion line2)
    {
        return new Vector2(
            (line1.b * line2.c - line2.b * line1.c) / (line1.a * line2.b - line2.a * line1.b),
            (line1.c * line2.a - line2.c * line1.a) / (line1.a * line2.b - line2.a * line1.b));
    }

    private LineEquasion GetLineEquasionFromPoints(Vector2 A, Vector2 B)
    {
        return new LineEquasion(A.y - B.y, B.x - A.x, ((B.y - A.y) * (A.x + B.x) + (A.x - B.x) * (A.y + B.y)) / 2);
    }

    private bool CrossingPointsCheck(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2)
    {
        return VM(A1, B1, B2) != VM(A2, B1, B2) && VM(A1, A2, B1) != VM(A1, A2, B2);
    }

    /// <summary>
    /// Vector Multiplication
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <param name="C"></param>
    /// <returns></returns>
    private bool VM(Vector2 A, Vector2 B, Vector2 C)
    {
        return (C.y - A.y) * (B.x - A.x) < (B.y - A.y) * (C.x - A.x);
    }

    private Vector2 RotateRight(Vector2 vector)
    {
        return new Vector2(vector.y, -vector.x);
    }
}

struct PointInCable
{
    public int pointIndex;
    public int cableIndex;
    public Cable cable;

    public PointInCable(int pointIndex, int cableIndex, Cable cable)
    {
        this.pointIndex = pointIndex;
        this.cableIndex = cableIndex;
        this.cable = cable;
    }

    public Vector2 GetPosition() => cable.points[pointIndex];
    public Vector2 GetPreviousPosition() => cable.points[pointIndex - 1];
}

[Serializable]
struct OverlapPoint
{
    public Vector2 position;
    public Cable cableBelow;
    public Cable cableAbove;
    public int cableBelowPointIndex;
    public int cableAbovePointIndex;
    public int orderInLayer;

    public OverlapPoint(Vector2 position, Cable cableBelow, Cable cableAbove, int cableBelowPointIndex, int cableAbovePointIndex, int orderInLayer)
    {
        this.position = position;
        this.cableBelow = cableBelow;
        this.cableAbove = cableAbove;
        this.cableBelowPointIndex = cableBelowPointIndex;
        this.cableAbovePointIndex = cableAbovePointIndex;
        this.orderInLayer = orderInLayer;
    }

    public OverlapPoint Reversed() => new OverlapPoint(position, cableAbove, cableBelow, cableAbovePointIndex, cableBelowPointIndex, orderInLayer);

    public OverlapPoint Copy() => new OverlapPoint(position, cableBelow, cableAbove, cableBelowPointIndex, cableAbovePointIndex, orderInLayer);

    public bool SameCablesAndOrderAs(OverlapPoint anotherPoint)
    {
        return (cableBelow == anotherPoint.cableBelow && cableAbove == anotherPoint.cableAbove);
    }

    public bool SameCables(OverlapPoint anotherPoint)
    {
        return (SameCablesAndOrderAs(anotherPoint) || SameCablesAndOrderAs(anotherPoint.Reversed()));
    }
}

public struct LineEquasion
{
    public float a, b, c;
    private float abVectorSquaredInverse;

    public LineEquasion(float a, float b, float c)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        abVectorSquaredInverse = 1 / Mathf.Sqrt(a * a + b * b);
    }
    public override string ToString()
    {
        return a.ToString() + "x + " + b.ToString() + "y + " + c.ToString() + " = 0";
    }

    public float DistanceToPoint(Vector2 point)
    {
        return Mathf.Abs(a * point.x + b * point.y + c) * abVectorSquaredInverse;
    }
}