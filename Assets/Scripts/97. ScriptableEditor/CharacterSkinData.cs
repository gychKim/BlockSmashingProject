using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSkinData", menuName = "KGC/SkinData/CharacterSkinData")]
public class CharacterSkinData : ScriptableObject
{
    public int characterID; // 캐릭터 ID > 지금은 사용 안하는데 혹시나 필요할 수 있으니까...
    public int index; // 스킨 Index
    public Sprite skinSprite; // 스킨 스프라이트
    public int price; // 스킨 가격

    // public GameObject breakEffect // 파괴 이펙트
}
