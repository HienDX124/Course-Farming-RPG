using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryBar : MonoBehaviour
{
    [SerializeField] private Sprite blank16x16sprite = null;
    [SerializeField] private UIInventorySlot[] inventorySlots = null;

    private RectTransform rectTransform;
    private bool _isInventoryBarPositionBottom = true;

    public bool IsInventoryBarPositionBottom { get => _isInventoryBarPositionBottom; set => _isInventoryBarPositionBottom = value; }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnDisable()
    {
        EventHandler.InventoryUpdateEvent -= InventoryUpdated;
    }

    private void OnEnable()
    {
        EventHandler.InventoryUpdateEvent += InventoryUpdated;
    }

    private void InventoryUpdated(InventoryLocation inventoryLocation, List<InventoryItem> inventoryList)
    {
        if (inventoryLocation == InventoryLocation.player)
        {
            ClearInventorySlots();



            if (inventorySlots.Length > 0 && inventoryList.Count > 0)
            {
                //  Loop through inventory slots and update with corresponding inventory list item
                for (int i = 0; i < inventorySlots.Length; i++)
                {
                    if (i < inventoryList.Count)
                    {
                        int itemCode = inventoryList[i].itemCode;

                        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);

                        if (itemDetails != null)
                        {
                            // Add images and details to inventory item slot
                            inventorySlots[i].inventorySlotImage.sprite = itemDetails.itemSprite;
                            inventorySlots[i].textMeshProUGUI.text = inventoryList[i].itemQuantity.ToString();
                            inventorySlots[i].itemDetails = itemDetails;
                            inventorySlots[i].itemQuantity = inventoryList[i].itemQuantity;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }

    private void ClearInventorySlots()
    {
        if (inventorySlots.Length > 0)
        {
            // Loop through inventory slots and update with blank sprite
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                inventorySlots[i].inventorySlotImage.sprite = blank16x16sprite;
                inventorySlots[i].textMeshProUGUI.text = "";
                inventorySlots[i].itemDetails = null;
                inventorySlots[i].itemQuantity = 0;
            }
        }
    }

    private void Update()
    {
        // Switch inventory bar position depending on player position
        SwitchInventoryBarPosition();
    }



    private void SwitchInventoryBarPosition()
    {
        Vector3 playerViewportPosition = Player.Instance.GetPlayerViewportPosition();
        if (playerViewportPosition.y > 0.3f && IsInventoryBarPositionBottom == false)
        {
            // transform.position = new Vector3(transform.position.x, 7.5f, 0f); // this was changed to control the recttransform see below
            rectTransform.pivot = new Vector2(0.5f, 0f);
            rectTransform.anchorMin = new Vector2(0.5f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.anchoredPosition = new Vector2(0f, 2.5f);

            IsInventoryBarPositionBottom = true;
        }
        else if (playerViewportPosition.y <= 0.3f && IsInventoryBarPositionBottom == true)
        {
            // transform.position = new Vector3(transform.position.x, mainCamera.pixelHeight - 120f, 0f); // this was changed to control the recttransform see below
            rectTransform.pivot = new Vector2(0.5f, 1f);
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rectTransform.anchoredPosition = new Vector2(0f, -2.5f);

            IsInventoryBarPositionBottom = false;
        }

    }
}
