using NUnit.Framework.Interfaces;
using UnityEngine;

public class InventoryItem
{
    public BaseItemSO item;

    public InventoryItem(BaseItemSO item)
    {
        this.item = item;
    }

    public bool Use(GameObject player)
    {
        return item.Use(player);
    }
}
