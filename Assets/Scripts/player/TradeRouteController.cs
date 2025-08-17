using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.AI;

public class TradeRouteController : SerializedMonoBehaviour, IPort
{
    [OdinSerialize] private readonly List<TradeRoute> tradeRoutes = new();
    [SerializeField] private Inventory inventory;
    [SerializeField] private LineRenderer tradeRoutePrefab;
    [SerializeField] private Transform port;
    [SerializeField] private SelectionIndicator selectionIndicator;

    public Transform Position => port;
    public Transform Offset => transform;

    [SerializeField] private int speed;

    public void AddTradeRoute(ResourcePoint port)
    {
        if (port == null) return;

        TradeRoute newRouteCandidate = new(port, tradeRoutePrefab, this.port.position);
        Debug.Log($"Added port to route: {port.name} with distance: {newRouteCandidate.Distance}");

        tradeRoutes.Add(newRouteCandidate);
    }

    void Update()
    {
        foreach (var route in tradeRoutes)
        {
            if (route.Progress(Time.deltaTime * speed))
            {
                inventory.AddMoney(route.Port.Reward);
            }
        }
    }

    public void EnableHighlight()
    {
        selectionIndicator?.Enable();
    }

    public void DisableHighlight()
    {
        selectionIndicator?.Disable();
    }
}

public class TradeRoute
{
    public LineRenderer TradeRoutePrefab { get; set; }
    public Vector3 Origin { get; set; }
    public ResourcePoint Port { get; set; }
    public float Distance => PathLength(path);
    private LineRenderer lineRenderer;
    private NavMeshPath path = new();
    private float progress = 0;

    public TradeRoute(ResourcePoint port, LineRenderer tradeRoutePrefab, Vector3 origin)
    {
        Port = port;
        TradeRoutePrefab = tradeRoutePrefab;
        Origin = origin;
        // InstantiateTradeRoute(port);
    }

    public bool Progress(float deltaTime)
    {
        progress += deltaTime;
        // Debug.Log($"Progressing trade route to {Port.name}: {progress}/{Distance}");
        if (progress >= Distance)
        {
            // Debug.Log("Trade route completed");
            progress = 0;
            return true;
        }

        return false;
    }

    private void InstantiateTradeRoute(ResourcePoint port)
    {
        if (NavMesh.CalculatePath(Origin, port.Port.position, NavMesh.AllAreas, path))
        {
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                RenderPath(path.corners, port.Port.position);
            }
            else
            {
                // Partial path or no path - handle as needed
                Debug.Log("No complete path found");
            }
        }
        else
        {
            Debug.Log("No path found");
        }

    }

    private void RenderPath(Vector3[] corners, Vector3 destination)
    {
        Debug.Log("Rendering path with " + corners.Length + " corners to destination: " + destination);
        // Create a new LineRenderer for the trade route
        lineRenderer = Object.Instantiate(TradeRoutePrefab);
        lineRenderer.positionCount = corners.Length;

        // Set the positions of the line renderer to the positions of the resource points
        // lineRenderer.SetPosition(0, port.position);

        lineRenderer.SetPositions(corners);

        // lineRenderer.SetPosition(lineRenderer.positionCount - 1, destination);
    }

    private float PathLength(NavMeshPath path)
    {
        if (path.corners.Length < 2)
            return 0;

        Vector3 previousCorner = path.corners[0];
        float lengthSoFar = 0.0f;

        for (int i = 1; i < path.corners.Length; i++)
        {
            Vector3 currentCorner = path.corners[i];
            lengthSoFar += Vector3.Distance(previousCorner, currentCorner);
            previousCorner = currentCorner;
        }

        return lengthSoFar;
    }
}
