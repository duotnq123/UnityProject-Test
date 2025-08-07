using UnityEngine;

public abstract class ItemBase : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public string description;

    public abstract void Use(Transform user, PlayerHealth health);
}
