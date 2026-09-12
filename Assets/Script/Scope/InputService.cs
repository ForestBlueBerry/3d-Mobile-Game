using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

public class InputService : IInputService,ITickable
{
    public bool IsTapped { get;  private set; }

    public bool IsPressed { get; private set; }

    public Vector2 TouchPosition { get; private set; }

    public Vector2 TouchDelta { get; private set; }

    public void Tick()
    {
        if (Pointer.current == null) return;

            IsTapped = Pointer.current.press.wasPressedThisFrame;
            IsPressed = Pointer.current.press.isPressed;
        if (IsPressed)
        {
            TouchPosition = Pointer.current.position.ReadValue();
            TouchDelta = Pointer.current.delta.ReadValue();
        }
      
       
    }
}
