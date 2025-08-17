using System.Collections.Generic;
using UnityEngine;

public class SmoothLineRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private List<Transform> points;
    [SerializeField] private float cornerRadius = 0.1f;
    [SerializeField] private int arcSamples = 4;

    public List<Transform> Points => points;
    private List<Vector3> lineVertices;

    void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
    }


    // void OnValidate()
    // {
    //     if (lineRenderer == null)
    //     {
    //         lineRenderer = GetComponent<LineRenderer>();
    //     }

    //     lineVertices = CornerSmoothing.RoundCorners(points.ConvertAll(p => p.position).ToArray(), cornerRadius, arcSamples);

    //     lineRenderer.positionCount = lineVertices.Count;
    //     lineRenderer.SetPositions(lineVertices.ToArray());
    // }

    void OnDrawGizmos()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        lineVertices = PathUtils.RoundCorners(points.ConvertAll(p => p.position).ToArray(), cornerRadius, arcSamples);

        lineRenderer.positionCount = lineVertices.Count;
        lineRenderer.SetPositions(lineVertices.ToArray());
    }
}