using System;
using UniRx;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockGameData", menuName = "KGC/BlockData/BlockGameData")]
public class BlockGameData : ScriptableObject
{
    public BlockType blockType;

    public BlockItemData result;
    public Sprite sprite;
    public Color backgroundColor;

    /// <summary>
    /// 블록이 사용될 때 블록이 하는 일
    /// </summary>
    public void Use() => result.Use();
}
