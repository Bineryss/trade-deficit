using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshLineRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform origin;
    [SerializeField] private Transform destination;

    [SerializeField] private List<Vector3> points;
    [SerializeField] private float cornerRadius = 100f;
    [SerializeField] private int cornerSmoothness = 500;
    [SerializeField] private float minDistance = 0.1f;


    private NavMeshPath path;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        path = new NavMeshPath();
    }
    void Update()
    {
        CalculatePath();
    }

    private void CalculatePath()
    {
        if (!NavMesh.CalculatePath(origin.position, destination.position, NavMesh.AllAreas, path)) return;


        Vector3[] reducedPoints = PathUtils.ReduceClosePoints(path.corners, minDistance);
        points = PathUtils.RoundCorners(reducedPoints, cornerRadius, cornerSmoothness);
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }

    void OnDrawGizmos()
    {
        if (points == null || points.Count == 0) return;

        Gizmos.color = Color.red;
        for (int i = 0; i < points.Count - 1; i++)
        {
            Gizmos.DrawLine(points[i], points[i + 1]);
            Gizmos.DrawWireSphere(points[i], 100);
        }
    }
}