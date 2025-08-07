using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryInputHandler : MonoBehaviour
{
    public GameObject inventoryUI; // Gán GameObject UI inventory trong inspector
    private InputSystem_Actions inputActions;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.UI.ToggleInventory.performed += OnToggleInventory;
    }

    private void OnDisable()
    {
        inputActions.UI.ToggleInventory.performed -= OnToggleInventory;
        inputActions.Disable();
    }

    private void OnToggleInventory(InputAction.CallbackContext context)
    {
        if (inventoryUI == null) return;

        inventoryUI.SetActive(!inventoryUI.activeSelf);
        Debug.Log("Toggled Inventory: " + inventoryUI.activeSelf);
    }
}
