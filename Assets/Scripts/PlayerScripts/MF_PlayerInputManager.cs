using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MF_PlayerInputManager : MonoBehaviour
{
    [SerializeField]
    private InventoryManager inventoryManager;
    
    private PlayerInput _playerInput;

    private void Start()
    {
        // Get the PlayerInput component
        _playerInput = GetComponent<PlayerInput>();
        if (_playerInput == null)
        {
            Debug.LogError("PlayerInput component missing!");
            return;
        }

        // Register input actions
        var openInventoryAction = _playerInput.actions["OpenInventory"];
        openInventoryAction.performed += OnOpenInventory;

        var useItemSlot1 = _playerInput.actions["UseItemSlot1"]; // Handles arrow key navigation
        useItemSlot1.performed += OnUseItemSlot1;
        
        var useItemSlot2 = _playerInput.actions["UseItemSlot2"]; // Handles arrow key navigation
        useItemSlot2.performed += OnUseItemSlot2;
    }

    private void OnUseItemSlot1(InputAction.CallbackContext obj)
    {
        inventoryManager.UseSlot(0);
    }
    private void OnUseItemSlot2(InputAction.CallbackContext obj)
    {
        inventoryManager.UseSlot(1);
    }


    private void OnOpenInventory(InputAction.CallbackContext obj)
    {
        inventoryManager.OpenCloseInventory();
    }
}
