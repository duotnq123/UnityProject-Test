using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public ItemBase itemToPickup;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager inventory = FindObjectOfType<InventoryManager>();
            if (inventory != null && itemToPickup != null)
            {
                inventory.AddItem(itemToPickup);
                Debug.Log("Đã nhặt item: " + itemToPickup.itemName);
                Destroy(gameObject); // Xoá item khỏi scene
            }
        }
    }
}
