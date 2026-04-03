using UnityEngine;

public class QuestBinder : BaseBinder<QuestView, QuestPresenter, QuestModel>
{
    protected override QuestModel CreateModel()
    {
        return new QuestModel();
    }

    protected override QuestPresenter CreatePresenter(QuestView view, QuestModel model)
    {
        return new QuestPresenter(view, model);
    }
}