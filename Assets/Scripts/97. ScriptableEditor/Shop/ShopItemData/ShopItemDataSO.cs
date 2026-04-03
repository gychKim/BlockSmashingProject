using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "KGC/Shop/ItemData")]
public class ShopItemDataSO : ShopItemSO
{
    public ShopItemType ItemType; // 아이템 타입
}
