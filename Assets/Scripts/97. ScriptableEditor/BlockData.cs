using UnityEngine;

[CreateAssetMenu(fileName = "BlockData", menuName = "KGC/BlockData/BlockData")]
public class BlockData : ScriptableObject
{
    public int blockID; // 블록 ID

    public Sprite blockSprite; // 블록 스프라이트

    public RuntimeAnimatorController animController; // 블록 애니메이션
    //public BlockShopData blockShopData;

    public int price; // 블록 가격
}
