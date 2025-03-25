using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControllerManu : MonoBehaviour
{
    [SerializeField] private CinemachineCamera playerCamera; // Cámara de Cinemachine
    [SerializeField] private int defaultPriority = 10; // Prioridad normal
    [SerializeField] private int talkPriority = -1; // Prioridad al hablar
    [SerializeField] private InputActionReference talkAction; // Acción para hablar con NPC
    [SerializeField] PlayerMovementManu playerMovementManu;

    private void Update()
    {
        if (talkAction.action.WasPressedThisFrame())
        {
            ToggleCameraPriority();
        }
    }

    private void ToggleCameraPriority()
    {
        if (playerCamera != null)
        {
            bool isTalking = playerCamera.Priority == defaultPriority;

            playerCamera.Priority = isTalking ? talkPriority : defaultPriority;
            playerMovementManu.canMove = !isTalking;
        }
    }
}
