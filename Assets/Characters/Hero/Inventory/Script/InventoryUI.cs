using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject itemSlotPrefab; // Prefab của UI slot cho 1 item
    public Transform itemSlotParent;  // Grid layout chứa các item slot
    public InventoryManager inventoryManager;

    private void OnEnable()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        // Xóa toàn bộ slot cũ
        foreach (Transform child in itemSlotParent)
        {
            Destroy(child.gameObject);
        }

        // Tạo slot mới theo danh sách item hiện tại
        foreach (var item in inventoryManager.items)
        {
            GameObject slotGO = Instantiate(itemSlotPrefab, itemSlotParent);
            ItemSlotUI slotUI = slotGO.GetComponent<ItemSlotUI>();
            slotUI.SetItem(item);
        }
    }
}
