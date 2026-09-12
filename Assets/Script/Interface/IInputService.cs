using UnityEngine;

public interface IInputService
{
    bool IsTapped { get; }
    bool IsPressed { get; } 
    Vector2 TouchPosition { get; }
    Vector2 TouchDelta { get; }
}
