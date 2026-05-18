using UnityEngine;
using UnityEngine.Purchasing;
using System.Collections.Generic;

[CreateAssetMenu(
    fileName = "IAPConfig",
    menuName = "Game Services/IAP Config")]
public class IAPConfig : ScriptableObject
{
    public bool enableIAP = true;

    public List<IAPProductData> products = new List<IAPProductData>();
}

[System.Serializable]
public class IAPProductData
{
    [Header("Common")]
    public string productName;

    [Header("Store IDs")]
    public string androidProductId;
    public string iosProductId;

    [Header("Product Type")]
    public ProductType productType;
}