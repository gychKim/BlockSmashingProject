using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopSO", menuName = "KGC/Shop/Shop")]
public class ShopSO : ScriptableObject
{
    public ShopType ShopType;

    public string ShopName;

    public string ShopDescription;

    public List<int> ItemIDList;
}
