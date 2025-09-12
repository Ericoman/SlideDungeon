using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

public class DeviceDetector : MonoBehaviour
{
    private bool usingKeyboard = true;

    void OnEnable()
    {
        InputSystem.onEvent += OnInputEvent;
    }

    void OnDisable()
    {
        InputSystem.onEvent -= OnInputEvent;
    }

    private void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
    {
        // Ignore noise / null
        if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>())
            return;

        UpdateDevice(device);
    }

    private void UpdateDevice(InputDevice device)
    {
        if (device is Keyboard || device is Mouse)
        {
            usingKeyboard = true;
        }
        else if (device is Gamepad)
        {
            usingKeyboard = false;
        }
    }

    public bool IsUsingKeyboard()
    {
        return usingKeyboard;
    }
}
