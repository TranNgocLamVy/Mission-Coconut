using UnityEngine;

[CreateAssetMenu(fileName = "FirstAid", menuName = "Scriptable Objects/PickableItems/FirstAid")]
public class FirstAid : BaseItemSO
{
    [Header("Item Information")]
    [SerializeField] public override string itemName => "First Aid";
    [SerializeField] private int healAmount = 30;

    public override bool Pickup(GameObject player)
    {
        PlayerInventory playerInventory = player.GetComponent<PlayerInventory>();
        if (playerInventory != null)
        {
            playerInventory.AddItem(new InventoryItem(this));
            return true;
        }

        return false;
    }

    public override bool Use(GameObject player)
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            if (playerHealth.Heal(healAmount)) return true;
        }
        return false;
    }
}
