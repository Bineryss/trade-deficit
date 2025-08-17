using UnityEngine;

public interface IPort
{
    Transform Position { get; }
    Transform Offset { get; }

    void EnableHighlight();
    void DisableHighlight();
}
