using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image iconImage;        // <- kéo Image từ Inspector vào
    public TextMeshProUGUI nameText; // nếu có

    private ItemBase currentItem;

    public void SetItem(ItemBase item)
    {
        currentItem = item;

        // Lấy icon từ ItemBase gán cho UI
        iconImage.sprite = item.icon;
        nameText.text = item.itemName;
    }
}
