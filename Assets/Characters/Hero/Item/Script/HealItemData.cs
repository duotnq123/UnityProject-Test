using UnityEngine;

[CreateAssetMenu(fileName = "New Heal Item", menuName = "Item/Heal Item")]
public class HealItemData : ItemBase
{
    public int healAmount = 50;
    public GameObject healVFX;

   public override void Use(Transform user, PlayerHealth health)
    {
        if (health != null && health.CurrentHealth < health.MaxHealth)
        {
            health.Heal(healAmount);

            if (healVFX != null)
            {
                GameObject vfx = GameObject.Instantiate(healVFX, user.position, Quaternion.identity);
                GameObject.Destroy(vfx, 2f);
            }

            Debug.Log("Used heal item: " + itemName);
        }
        else
        {
            Debug.Log("Health is full. Cannot use heal item.");
        }
    }
}
