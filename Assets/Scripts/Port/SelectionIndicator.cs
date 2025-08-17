using UnityEngine;

[RequireComponent(typeof(LineRenderer), typeof(Collider))]
public class SelectionIndicator : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float radius = 5f;
    [SerializeField] private int segments = 50;

    void Start()
    {
        CreateCircle();
        lineRenderer.enabled = false;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        CreateCircle();
    }
#endif


    private void CreateCircle()
    {
        lineRenderer.positionCount = segments;

        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2 / segments;
            float x = Mathf.Cos(angle) * radius + transform.position.x;
            float z = Mathf.Sin(angle) * radius + transform.position.z;

            Vector3 point = new(x, -5, z);
            lineRenderer.SetPosition(i, point);
        }
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
