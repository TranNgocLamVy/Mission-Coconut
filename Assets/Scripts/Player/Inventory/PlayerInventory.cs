using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] public List<InventoryItem> inventoryItems = new List<InventoryItem>();
    [SerializeField] private TextMeshProUGUI inventoryText;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponentInParent<AudioSource>();
    }

    public void AddItem(InventoryItem item)
    {
        inventoryItems.Add(item);
        if (inventoryText != null)
        {
            inventoryText.text = "x" + inventoryItems.Count;
        }
    }

    public void RemoveItem(InventoryItem item)
    {
        inventoryItems.Remove(item);
        if (inventoryText != null)
        {
            inventoryText.text = "x" + inventoryItems.Count;
        }
    }

    public void UseItem()
    {
        if (inventoryItems.Count > 0)
        {
            InventoryItem item = inventoryItems[0];
            bool useItemSuccess = item.Use(gameObject);
            if (useItemSuccess)
            {
                audioSource.PlayOneShot(item.item.useSound);
                RemoveItem(item);
            }
        }
    }
}
