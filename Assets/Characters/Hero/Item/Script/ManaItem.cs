using UnityEngine;

[CreateAssetMenu(fileName = "New Mana Item", menuName = "Item/Mana Item")]
public class ManaItem : ItemBase
{
    [Header("Mana Settings")]
    public int manaAmount = 30;
    public GameObject manaVFX;

    public override void Use(Transform user, PlayerHealth playerHealth)
    {
        Debug.Log("ManaItem Used!");

        var mana = user.GetComponent<PlayerMana>();
        if (mana == null)
        {
            Debug.LogWarning("PlayerMana component not found on " + user.name);
            return;
        }

        // Optional: dùng điều kiện nếu không muốn dùng khi đầy mana
        if (mana.CurrentMana >= mana.MaxMana)
        {
            Debug.Log("Mana is already full.");
            return;
        }

        mana.RestoreMana(manaAmount);

        if (manaVFX != null)
        {
            GameObject vfx = Instantiate(manaVFX, user.position, Quaternion.identity);
            Destroy(vfx, 2f); 
        }
    }
}
