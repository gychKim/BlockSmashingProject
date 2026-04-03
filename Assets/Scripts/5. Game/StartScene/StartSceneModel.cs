
using UniRx;

public class StartSceneModel : IModel
{
    public IntReactiveProperty CurrentStage = new();
    public StringReactiveProperty StageName = new();
    public void Start()
    {
        CurrentStage.Value = GameManager.Instance.CurrentStage;

        var stageData = DBManager.Instance.GetStageData(CurrentStage.Value);
        StageName.Value = stageData.stageName;
    }

    public void Destroy()
    {
        
    }
}
