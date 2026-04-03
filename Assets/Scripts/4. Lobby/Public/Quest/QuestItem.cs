using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestItem : MonoBehaviour, IPoolable
{
    public Image IconImage => iconImage;
    [SerializeField]
    private Image iconImage;

    public int ID => id;
    private int id;

    public TextMeshProUGUI QuestNameText => questNameText;
    [SerializeField]
    private TextMeshProUGUI questNameText;

    public TextMeshProUGUI DescriptionText => descriptionText;
    [SerializeField]
    private TextMeshProUGUI descriptionText;

    public TextMeshProUGUI[] ValueTextArr => valueTextArr;
    [SerializeField]
    private TextMeshProUGUI[] valueTextArr;

    //public TextMeshProUGUI ValueText => valueText;
    //[SerializeField]
    //private TextMeshProUGUI valueText;

    //public TextMeshProUGUI ValueText01 => valueText01;
    //[SerializeField]
    //private TextMeshProUGUI valueText01;

    //public TextMeshProUGUI ValueText02 => valueText02;
    //[SerializeField]
    //private TextMeshProUGUI valueText02;

    //public QuestConditionType QuestType => questType;
    //private QuestConditionType questType;

    public GameObject CompleteUI => completeUI;
    [SerializeField]
    private GameObject completeUI;

    public GameObject EndUI => endUI;
    [SerializeField]
    private GameObject endUI;

    public Button CompleteButton => completeButton;
    [SerializeField]
    private Button completeButton;

    public TextMeshProUGUI RewardValueText => rewardValueText;
    [SerializeField]
    public TextMeshProUGUI rewardValueText;


    private bool isInit;
    private void OnEnable()
    {
        if (!isInit)
            return;

        // EndUI가 활성화라는 것은 이미 완료되었다는 뜻이므로 그대로 리턴
        if (endUI.activeSelf)
            return;

        // 활성화 되었을 때, 현재 달성량 수치 업데이트
        var questProgress = QuestManager.Instance.ActiveProgresses.FirstOrDefault(progress => progress.questID == id);

        var data = QuestManager.Instance.QuestDataList.FirstOrDefault(data => data.id == id);

        var baseData = data.baseData;

        for (int i = 0; i < data.ItemCount; i++)
        {
            valueTextArr[i].gameObject.SetActive(true); // 필요한 부분만 활성화
            valueTextArr[i].text = $"{data.GetItemName()[i]} : {data.GetProgress(i)} / {baseData.targetValue}";
        }

        //valueText.text = $"{questProgress.progress.ToString()} / {data.targetValue.ToString()}";

        // 클리어라면 완료표시 활성화
        if (questProgress.completed)
        {
            completeUI.SetActive(true);
        }
    }

    public void Init(QuestProgress progress)
    {
        id = progress.questID;

        //var dataSO = QuestManager.Instance.pool.questList.FirstOrDefault(data => data.id == id);
        var data = QuestManager.Instance.QuestDataList.FirstOrDefault(data => data.id == id);

        var baseData = data.baseData;

        questNameText.text = baseData.title;
        descriptionText.text = baseData.desc;

        // 일단 비활성으로 초기화
        foreach (var item in valueTextArr)
        {
            item.gameObject.SetActive(false);
        }

        for (int i = 0; i < data.ItemCount; i++)
        {
            valueTextArr[i].gameObject.SetActive(true); // 필요한 부분만 활성화
            valueTextArr[i].text = $"{data.GetItemName()[i]} : {data.GetProgress(i)} / {baseData.targetValue}";
        }

        rewardValueText.text = baseData.reward.gold.ToString();

        if(progress.claimed)
        {
            QuestEnd();
        }
        else if(progress.completed)
        {
            QuestClear();
        }
        isInit = true;
    }

    /// <summary>
    /// 퀘스트 완료 가능할 시
    /// </summary>
    public void QuestClear()
    {
        completeUI.SetActive(true);
    }

    /// <summary>
    /// 퀘스트 완료 시
    /// </summary>
    public void QuestEnd()
    {
        completeUI.SetActive(false);
        endUI.SetActive(true);
    }

    public void Get()
    {
        if (iconImage == null)
            iconImage = GetComponent<Image>();

        if (completeButton == null)
            completeButton = GetComponent<Button>();
    }

    public void Release()
    {
        iconImage.sprite = null;
        id = -1;
        questNameText.text = "";
        descriptionText.text = "";

        foreach (var item in valueTextArr)
        {
            item.text = "";
        }

        rewardValueText.text = "";
    }

    public void Destroy()
    {
        Release();
        iconImage = null;
        completeUI = null;
        endUI = null;
        completeButton = null;
    }
}
