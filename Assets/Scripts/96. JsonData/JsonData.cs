using System;
using System.Collections.Generic;

[Serializable]
public class JsonData
{
    public List<CharacterSaveData> characterList = new();
    public List<BlockSaveData> blockList = new();
    public List<ItemSaveData> itemList = new();
    public GameStateSaveData gameState = new();
    public QuestSaveData questSaveData = new();
}
