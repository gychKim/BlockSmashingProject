using UnityEngine;

[CreateAssetMenu(fileName = "CharacterShopData", menuName = "KGC/Shop/CharacterShopData")]
public class CharacterShopData : ScriptableObject
{
    public int characterID; // 캐릭터 ID
    public Sprite characterUISprite; // 캐릭터 UI 스프라이트

    public int price; // 캐릭터 가격
}
