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

            // Print item description to console window
            Debug.Log(itemDetails.itemDescription);
        }
    }
}
