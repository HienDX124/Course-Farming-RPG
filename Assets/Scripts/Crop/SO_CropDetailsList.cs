using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CropDetailsList", menuName = "Assets/Scriptable Objects/Crop/CropDetailsList.asset")]
public class SO_CropDetailsList : ScriptableObject
{
    [SerializeField] public List<CropDetails> cropDetails;

    public CropDetails GetCropDetails(int seedItemCode)
    {
        return cropDetails.Find(x => x.seedItemCode == seedItemCode);
    }
}
