using UnityEngine;

[System.Serializable]
public class ItemDetails : MonoBehaviour
{
    public int itemCode;
    public ItemType itemType;
    public string itemDescription;
    public Sprite itemSprite;
    public string itemLongDescription;
    public short itemUseGridRadius;
    public float itemUseRadius;
    public bool isStartingItem;
    public bool canPickedUp;
    public bool canBeDroped;
    public bool canBeEaten;
    public bool canBeCarried;

}
