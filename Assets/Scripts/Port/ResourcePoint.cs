using UnityEngine;

public class ResourcePoint : MonoBehaviour, IPort
{
    [SerializeField] private Transform port;
    [SerializeField] private int reward;
    [SerializeField] private SelectionIndicator selectionIndicator;

    public Transform Port => port;
    public int Reward => reward;

    public Transform Position => port;

    public Transform Offset => transform;

    public void DisableHighlight()
    {
        selectionIndicator?.Disable();
    }

    public void EnableHighlight()
    {
        selectionIndicator?.Enable();
    }
}
