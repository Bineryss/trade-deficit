using System.Collections.Generic;
using UnityEngine;

public class LineVisualizer : MonoBehaviour
{
    [SerializeField] private SmoothLineRenderer smoothLineRenderer;
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < smoothLineRenderer.Points.Count - 1; i++)
        {
            Gizmos.DrawLine(smoothLineRenderer.Points[i].position, smoothLineRenderer.Points[i + 1].position);
        }
    }
}