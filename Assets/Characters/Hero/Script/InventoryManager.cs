using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<ItemBase> items = new List<ItemBase>();

    public void AddItem(ItemBase item)
    {
        items.Add(item);
        Debug.Log("Added item: " + item.itemName);
    }

    public void UseHealItem(Transform user, PlayerHealth health)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] is HealItemData healItem)
            {
                healItem.Use(user, health);
                items.RemoveAt(i);
                return;
            }
        }

        Debug.Log("No healing item found in inventory.");
    }
    public void UseManaItem(Transform user, PlayerMana playerMana)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] is ManaItem manaItem)
            {
                manaItem.Use(user, null); 
                items.RemoveAt(i);
                Debug.Log("Đã dùng bình mana");
                return;
            }
        }

        Debug.Log("Không có bình mana trong túi.");
    }
}
