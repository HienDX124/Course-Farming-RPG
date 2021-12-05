using System;
using System.Collections.Generic;
using UnityEngine;


public class InventoryManager : SingletonMonobehaviour<InventoryManager>
{

    private Dictionary<int, ItemDetails> itemDetailsDictionary;

    [SerializeField] private SO_ItemList itemList = null;

    protected override void Awake()
    {
        base.Awake();

        // Creat item details dictionary
        CreatItemDetailsDictionary();
    }

    /// <sumary>
    /// Populates the itemDetailsDictionary from the scriptable object items list
    /// </sumary>

    private void CreatItemDetailsDictionary()
    {
        itemDetailsDictionary = new Dictionary<int, ItemDetails>();
        foreach (ItemDetails itemDetails in itemList.itemDetails)
        {
            itemDetailsDictionary.Add(itemDetails.itemCode, itemDetails);
        }
    }
    /// <sumary>
    /// Return the itemDetails (from the SO_itemList) for the itemCode, or null if the itemCode doesn't exist
    /// </sumary>

    public ItemDetails GetItemDetails(int itemCode)
    {
        ItemDetails itemDetails;

        if (itemDetailsDictionary.TryGetValue(itemCode, out itemDetails))
        {
            return itemDetails;
        }
        else
        {
            return null;
        }
    }


}
