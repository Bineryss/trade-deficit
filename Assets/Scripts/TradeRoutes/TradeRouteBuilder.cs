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
        DrawLine();
    }

    public void OnSelect(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;
        if (!hit.collider.TryGetComponent<IPort>(out var originPort)) return;
        Debug.Log($"Clicked on: {hit.collider.name} - ${originPort.Position.position}");

        if (isDrawing)
        {
            // logic for buidling the route
            isDrawing = false;
        }
        else
        {
            origin = originPort.Position;
            isDrawing = true;
        }
    }

    private void DrawLine()
    {
        if (tradeRouteLineRenderer == null) return;
        if (origin == null) return;
        if (!isDrawing)
        {
            tradeRouteLineRenderer.positionCount = 0;
            return;
        }


        Vector3 currentMousePosition = GetMouseWorldPosition();
        Debug.Log($"Drawing line from {origin.position} to mouse position {currentMousePosition}");


        Vector3[] points = CalculatePoints(origin.position, currentMousePosition);
        tradeRouteLineRenderer.positionCount = points.Length;
        tradeRouteLineRenderer.SetPositions(points);
    }

    private Vector3[] CalculatePoints(Vector3 start, Vector3 end)
    {
        if (!NavMesh.CalculatePath(start, end, NavMesh.AllAreas, path)) return new Vector3[] { start, end };
        if (path.status == NavMeshPathStatus.PathComplete)
        {
            return path.corners;
        }
        else
        {
            Debug.Log("No complete path found");
        }

        return new Vector3[] { start, end };
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
