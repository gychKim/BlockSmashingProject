using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;

public class StartSceneView : MonoBehaviour, IUIView
{
    /// <summary>
    /// DOTween 애니메이션
    /// </summary>
    public DOTweenAnimation DOTweenAnim => dotweenAnim;
    [SerializeField]
    private DOTweenAnimation dotweenAnim;

    /// <summary>
    /// 스테이지 텍스트
    /// </summary>
    public TextMeshProUGUI StageNumberText => stageNumberText;
    [SerializeField]
    private TextMeshProUGUI stageNumberText;

    /// <summary>
    /// 스테이지 이름 텍스트
    /// </summary>
    public TextMeshProUGUI StageNameText => stageNameText;
    [SerializeField]
    private TextMeshProUGUI stageNameText;

    public GameObject RootObject => gameObject;

    public CompositeDisposable Disposables { get; } = new CompositeDisposable();

    /// <summary>
    /// 애니메이션 실행
    /// </summary>
    public void PlayAnimation()
    {
        dotweenAnim.DOPlay();
    }

    /// <summary>
    /// 스테이지 세팅
    /// </summary>
    /// <param name="num"></param>
    public void SetStage(int num) => stageNumberText.text = $"Stage {num.ToString()}";

    /// <summary>
    /// 스테이지 이름 세팅
    /// </summary>
    /// <param name="name"></param>
    public void SetStageName(string name) => stageNameText.text = name;

    public void ActiveUI(bool value)
    {
        RootObject.SetActive(value);
    }
}
