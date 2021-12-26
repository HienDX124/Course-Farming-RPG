using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CropDetails
{
    [ItemCodeDescription]
    public int seedItemCode;  // This is the item code for the corresponding see
    public int[] growthDays;    // Day growth for each stage
    public int totalGrowthDays; // Total growth days
    public GameObject[] growthPrefab;   // Prefab to use when instantiating growth stages
    public Sprite[] growthSprite;   // Growth sprite
    public Season[] seasons;        // Growth season
    public Sprite harvestedSprite;  // Sprite used once harvested
    [ItemCodeDescription]
    public int harvestTransformItemCode;  // If the item transform into another item when harvested this item code will be populated
    public bool hideCropBeforeHarvestedAnimation;   // If the crop should be disabled before harvested animation
    public bool disableCropCollidersBeforeHarvestedAnimation;   // If colliders on crop should be disabled to avoid the harvested animation effecting any other game object
    public bool isHarvestedAnimation;   // true if harvested animation to be played on final growth stage prefab
    public bool isHarvestActionAtPlayerPosition = false;    // Flag to determine whether there is a harvest action effect
    public bool spawnCropProducedAtPlayerPosition;
    public HarvestActionEffect harvestActionEffect; // The harvest action effect for the crop

    [ItemCodeDescription]
    public int[] harvestToolItemCode; // Array of item codes for the tools that can harvest or 0 array elements if no tool required
    public int[] requiredHarvestActions;  // Number of harvest actions required for corresponding tool in harvest tool item code array
    [ItemCodeDescription]
    public int[] cropProducedItemCode;    // Array of item codes produced for the harvested crop
    public int[] cropProducedMinQuantity;   // Array of minimum quantities, produced for the harvested crop
    public int[] cropProducedMaxQuantity;   // If max quantity is > min quantity then a random number of crop between min and max are produced
    public int dayToRegrow; // Day to regrow new crop or -1 if a single crop

    /// <sumary>
    /// Returns true if the tool item code can be used to harvest this crop, else returns false
    /// </sumary>
    public bool CanUseToolToHarvestCrop(int toolItemCode)
    {
        if (requiredHarvestActionsForTool(toolItemCode) == -1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public int requiredHarvestActionsForTool(int toolItemCode)
    {
        for (int i = 0; i < harvestToolItemCode.Length; i++)
        {
            if (harvestToolItemCode[i] == toolItemCode)
            {
                return requiredHarvestActions[i];
            }
        }
        return -1;
    }
}
