using UnityEngine;

[System.Serializable]
public class QuestSaveManager : ISaveManager
{
    public bool IsDirty => isDirty;
    private bool isDirty = false; // 변경되었는지 확인

    public bool IsLoadData => isLoadData;
    private bool isLoadData; // 데이터를 Load했는지 여부

    [SerializeField]
    private QuestInitDataSO questInitData; // 최초 게임 실행 시, 최초데이터를 저장하기 위해 사용하는 SO(Scriptable Object)
    public QuestSaveData QuestSaveData => questSaveData;
    private QuestSaveData questSaveData;

    /// <summary>
    /// 초기화
    /// </summary>
    public void Init()
    {
        questSaveData = questInitData.questSaveData;
    }

    /// <summary>
    /// Json데이터에 아이템 정보 저장
    /// isForce는 강제적으로 Save 시킬 때 사용, true면 반드시 저장
    /// </summary>
    /// <param name="jsonData"></param>
    /// <param name="isForce"></param>
    public void Save(JsonData jsonData, bool isForce = false)
    {
        // 데이터 변경이 되지 않았고, isForce가 false면 리턴
        if (!isDirty && !isForce)
            return;

        //QuestManager.Instance.Save(); // 아... 이게 맞나...?

        jsonData.questSaveData = questSaveData; // json데이터 변경

        isDirty = false; // 데이터 변경 여부 초기화
    }

    /// <summary>
    /// Json데이터를 받아와 데이터 갱신을 한다.
    /// </summary>
    /// <param name="jsonData"></param>
    public void Load(JsonData jsonData)
    {
        questSaveData = jsonData.questSaveData;
        isLoadData = true;
        //itemSaveDataList = jsonData.itemList;
        //itemSaveDataDict = itemSaveDataList.ToDictionary(x => x.itemID);

        //isLoadData = true;
    }

    /// <summary>
    /// 퀘스트 데이터 갱신
    /// </summary>
    public void UpdateData(QuestSaveData data)
    {
        questSaveData = data;
        isDirty = true;
    }

    //private const string KEY = "quest_daily_state_v1";

    //public static void Save(QuestDailyState state)
    //{
    //    var json = JsonUtility.ToJson(state);
    //    PlayerPrefs.SetString(KEY, json);
    //    PlayerPrefs.Save();
    //}
    //public static QuestDailyState Load()
    //{
    //    if (!PlayerPrefs.HasKey(KEY)) return null;
    //    var json = PlayerPrefs.GetString(KEY);
    //    if (string.IsNullOrEmpty(json)) return null;
    //    return JsonUtility.FromJson<QuestDailyState>(json);
    //}
    //public static void Clear()
    //{
    //    PlayerPrefs.DeleteKey(KEY);
    //}
}
