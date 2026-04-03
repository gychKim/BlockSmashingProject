using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "KGC/CharacterData")]
public class CharacterData : ScriptableObject
{
    public int characterID; // 캐릭터 ID

    public string characterName; // 캐릭터 이름

    public Sprite characterSprite; // 캐릭터 전신 스프라이트
    public Sprite characterUISprite; // 캐릭터 UI 스프라이트

    public List<CharacterSkinData> skinList; // 스킨 데이터 리스트 > Index는 CharacterSaveData가 지니고 있다.

    public RuntimeAnimatorController uiAnimatorController; // 캐릭터 UI 애니메이터 컨트롤러
    public RuntimeAnimatorController animatorController; // 캐릭터 애니메이터 컨트롤러

    public int price; // 가격
}
