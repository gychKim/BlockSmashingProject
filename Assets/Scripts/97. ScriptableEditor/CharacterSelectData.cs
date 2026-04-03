using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSelectData", menuName = "KGC/Select/CharacterSelectData")]
public class CharacterSelectData : ScriptableObject
{
    public int characterID; // 캐릭터 ID
    public string characterName; // 캐릭터 이름

    public Sprite characterSprite; // 캐릭터 스프라이트
    // 애니메이터
}
