using UnityEngine;

public class MouseSelector : MonoBehaviour
{
    public Camera playerCamera;
    public float maxDistance = 10000f;

    public GameObject selectedPort;
    public TradeRouteController tradeRouteController;


    private InputSystem_Actions gameControls;

    void Awake()
    {
        gameControls = new InputSystem_Actions();
        gameControls.Enable();
    }

    void Update()
    {
        SelectPort();
    }

    public void OnClick()
    {
        if (selectedPort == null) return;

        tradeRouteController.AddTradeRoute(selectedPort.GetComponent<ResourcePoint>());
    }

    public void SelectPort()
    {
        Vector2 mousePosition = gameControls.Player.MousePosition.ReadValue<Vector2>();
        Ray ray = playerCamera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            if (hit.collider.CompareTag("port")) // Make sure ports have "port" tag
            {
                selectedPort = hit.collider.gameObject;

                // Visual feedback - change material or add outline
                HighlightPort(selectedPort);
            }
            else
            {
                DeselectPort();
            }
        }
    }

    void HighlightPort(GameObject port)
    {
        // Simple color change example
        SelectionIndicator renderer = port.GetComponent<SelectionIndicator>();
        renderer.Enable();
    }

    void DeselectPort()
    {
        if (selectedPort != null)
        {
            SelectionIndicator renderer = selectedPort.GetComponent<SelectionIndicator>();
            renderer.Disable();
            selectedPort = null;
        }
    }
}
