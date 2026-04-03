using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class QuestView : BaseView
{
    /// <summary>
    /// Content의 Transform
    /// </summary>
    public Transform ContentTrans => contentTrans;
    [SerializeField]
    private Transform contentTrans;

    /// <summary>
    /// 퀘스트 UI
    /// </summary>
    public GameObject QuestUI => questUI;
    [SerializeField]
    private GameObject questUI;

    /// <summary>
    /// 퀘스트 버튼
    /// </summary>
    public Button QuestButton => questButton;
    [SerializeField]
    private Button questButton;

    /// <summary>
    /// 닫기 버튼
    /// </summary>
    public Button CloseButton => closeButton;
    [SerializeField]
    private Button closeButton;

    /// <summary>
    /// 퀘스트 UI 열기
    /// </summary>
    public void OpenQuestUI()
    {
        questUI.SetActive(true);
    }

    /// <summary>
    /// 퀘스트 UI 닫기
    /// </summary>
    public void CloseQuestUI()
    {
        questUI.SetActive(false);
    }

}
