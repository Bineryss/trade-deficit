using UnityEngine;

[RequireComponent(typeof(LineRenderer), typeof(Collider))]
public class SelectionIndicator : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int segments = 50;
    [SerializeField] private Transform point;

    void Start()
    {
        CreateCircle();
        lineRenderer.enabled = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        float radius = Vector3.Distance(transform.position, point.position);

        Vector3[] points = CalculatePoints(transform.position, radius);
        Vector3 previousPoint = points[0];

        foreach (Vector3 point in points)
        {
            Gizmos.DrawLine(previousPoint, point);
            previousPoint = point;
        }
        Gizmos.DrawLine(previousPoint, points[0]);
    }
#endif


    private void CreateCircle()
    {
        float radius = Vector3.Distance(transform.position, point.position);
        lineRenderer.positionCount = segments;
        Vector3[] points = CalculatePoints(transform.position, radius);
        lineRenderer.SetPositions(points);
    }

    private Vector3[] CalculatePoints(Vector3 origin, float radius)
    {
        Vector3[] points = new Vector3[segments];

        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2 / segments;
            float x = Mathf.Cos(angle) * radius + origin.x;
            float z = Mathf.Sin(angle) * radius + origin.z;

            points[i] = new(x, -5, z);
        }
        return points;
    }

    public void Enable()
    {
        lineRenderer.enabled = true;
    }

    public void Disable()
    {
        lineRenderer.enabled = false;
    }
}
