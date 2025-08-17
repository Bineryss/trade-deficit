using UnityEngine;

public class ResourcePoint : MonoBehaviour, IPort
{
    [SerializeField] private Transform port;
    [SerializeField] private int reward;

    public Transform Port => port;
    public int Reward => reward;

    public Transform Position => port;
}
