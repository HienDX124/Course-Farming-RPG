using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();
        if (item != null)
        {
            // Get item details
            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(item.ItemCode);

            // If item can be picked up
            if (itemDetails.canPickedUp)
            {
                // Add item to inventory
                InventoryManager.Instance.AddItem(InventoryLocation.player, item, collision.gameObject);
            }
        }
    }
}
