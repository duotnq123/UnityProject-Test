using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUseItemInput : MonoBehaviour
{
    [Header("References")]
    public InventoryManager inventoryManager;
    public PlayerHealth playerHealth;
    public PlayerMana playerMana;

    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        if (playerInput != null)
        {
            playerInput.actions["UseItemHealPotion"].performed += OnUseHealPerformed;
            playerInput.actions["UseItemManaPotion"].performed += OnUseManaPerformed;
        }
    }

    private void OnDestroy()
    {
        if (playerInput != null)
        {
            playerInput.actions["UseItemHealPotion"].performed -= OnUseHealPerformed;
            playerInput.actions["UseItemManaPotion"].performed -= OnUseManaPerformed;
        }
    }

    private void OnUseHealPerformed(InputAction.CallbackContext context)
    {
        if (inventoryManager != null && playerHealth != null)
        {
            inventoryManager.UseHealItem(transform, playerHealth);
        }
        else
        {
            Debug.LogWarning("InventoryManager hoặc PlayerHealth chưa được gán trong PlayerUseItemInput");
        }
    }
    private void OnUseManaPerformed(InputAction.CallbackContext context)
    {
        if (inventoryManager != null && playerMana != null)
        {
            inventoryManager.UseManaItem(transform, playerMana);
        }
        else
        {
            Debug.LogWarning("InventoryManager hoặc PlayerMana chưa được gán trong PlayerUseItemInput");
        }
    }
}
