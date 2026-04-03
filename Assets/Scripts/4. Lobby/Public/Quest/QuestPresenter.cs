using UniRx;
using UnityEngine;

public class QuestPresenter : BasePresenter<QuestView, QuestModel>
{
    public QuestPresenter(QuestView view, QuestModel model) : base(view, model)
    {
    }

    public override void Bind()
    {
        // 기존에 넣어져 있던 데이터 Binding
        foreach (var item in model.Items)
        {
            ItemBinding(item);
        }

        // 퀘스트 아이콘 버튼
        view.QuestButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                view.OpenQuestUI();
            }).AddTo(view.Disposables);

        // 닫기 버튼
        view.CloseButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                view.CloseQuestUI();
            }).AddTo(view.Disposables);
        // 퀘스트 생성 및 추가될 시 > 이런 일이 어지간 하면 없지만 혹시나 모르니까
        model.Items
            .ObserveAdd()
            .Subscribe(item =>
            {
                ItemBinding(item.Value);

            }).AddTo(view.Disposables);
    }

    /// <summary>
    /// Item의 데이터 Binding
    /// </summary>
    /// <param name="item"></param>
    private void ItemBinding(QuestItem item)
    {
        item.transform.SetParent(view.ContentTrans); // 퀘스트를 Content에 넣는다.

        item.transform.localScale = Vector3.one; // 퀘스트Panel 스케일 조정

        item.CompleteButton
            .OnClickAsObservable()
            .Subscribe(async _ =>
            {
                await QuestManager.Instance.PayReward(item.ID);
                item.QuestEnd();
            }).AddTo(view.Disposables);
    }

    public override void Destroy()
    {
        
    }
}
