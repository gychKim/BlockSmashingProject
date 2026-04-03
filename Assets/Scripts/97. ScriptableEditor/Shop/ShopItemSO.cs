using UnityEngine;

public abstract class ShopItemSO : ScriptableObject
{
    public int ID;

    public string Name;

    public string Description;

    public Sprite Sprite;

    //public PriceType PriceType;

    public int Price;
}
