using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class TradeRouteBuilder : MonoBehaviour
{
    [SerializeField] private LineRenderer tradeRouteLineRenderer;

    private InputSystem_Actions actions;
    [SerializeField] private Transform origin;
    [SerializeField] private Vector2 mousePosition;
    [SerializeField] private bool isDrawing;

    private IPort selectedPort;
    private IPort currentMousePort;
    private NavMeshPath path;

    void Awake()
    {
        actions = new();
        path = new();
        actions.Enable();

        actions.Player.Select.performed += OnSelect;
        actions.Player.MousePosition.performed += ctx => mousePosition = ctx.ReadValue<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        // OnHover();
        DrawLine();
    }

    void OnDestroy()
    {
        path = null;
    }

    public void OnSelect(InputAction.CallbackContext context)
    {
        selectedPort = GetPortFromMousePosition();

        if (selectedPort == null)
        {
            Debug.Log("No port selected");
            return;
        }

        if (isDrawing)
        {
            Debug.Log($"Establishing trade route");
            isDrawing = false;
            selectedPort.DisableHighlight();
            selectedPort = null;
        }
        else
        {
            selectedPort.EnableHighlight();
            origin = selectedPort.Position;
            isDrawing = true;
        }
    }

    private void DrawLine()
    {
        if (selectedPort == null) return;
        currentMousePort?.DisableHighlight();
        if (tradeRouteLineRenderer == null) return;
        if (origin == null) return;
        if (!isDrawing)
        {
            tradeRouteLineRenderer.positionCount = 0;
            return;
        }


        Vector3 currentMousePosition = GetMouseWorldPosition();
        currentMousePort = GetPortFromMousePosition();
        if (selectedPort.Equals(currentMousePort))
        {
            currentMousePort = null;
            return;
        }

        Vector3[] points;
        if (currentMousePort != null)
        {
            currentMousePort.EnableHighlight();
            points = CalculatePoints(selectedPort.Offset.position, origin.position, currentMousePort.Position.position, currentMousePort.Offset.position);
        }
        else
        {
            points = CalculatePoints(selectedPort.Offset.position, origin.position, currentMousePosition);
        }
        tradeRouteLineRenderer.positionCount = points.Length;
        tradeRouteLineRenderer.SetPositions(points);
    }

    private Vector3[] CalculatePoints(Vector3 offset, Vector3 start, Vector3 end, Vector3 endOffset = default)
    {
        if (!NavMesh.CalculatePath(start, end, NavMesh.AllAreas, path)) return new Vector3[] { offset, start, end, endOffset };
        if (path.status == NavMeshPathStatus.PathComplete)
        {
            int arrayOffset = endOffset == default ? 2 : 1;
            Vector3[] points = new Vector3[path.corners.Length + arrayOffset];
            points[0] = offset;
            for (int i = 0; i < path.corners.Length; i++)
            {
                points[i + 1] = path.corners[i];
            }
            if (endOffset != default)
            {
                points[^1] = endOffset;
            }

            return points;
        }
        else
        {
            Debug.Log("No complete path found");
        }

        return new Vector3[] { offset, start, end, endOffset };
    }

    private void OnHover()
    {
        IPort hoveredPort = GetPortFromMousePosition();
        if (hoveredPort == null)
        {
            hoveredPort?.DisableHighlight();
        }
        else
        {
            hoveredPort.EnableHighlight();
        }
    }

    private IPort GetPortFromMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return null;
        if (!hit.collider.TryGetComponent<IPort>(out var originPort)) return null;
        Debug.Log($"Clicked on: {hit.collider.name} - ${originPort.Position.position}");

        return originPort;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        Plane xzPlane = new(Vector3.up, 0);
        if (xzPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }
        return Vector3.zero;
    }

}
