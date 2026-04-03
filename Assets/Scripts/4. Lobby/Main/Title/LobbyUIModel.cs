using UniRx;
using UnityEngine;

public class LobbyUIModel : IModel
{
    public IntReactiveProperty CurrentStage = new(1);

    public StringReactiveProperty StageName = new(); // 스테이지 이름
    public StringReactiveProperty StageDescription = new(); // 스테이지 설명
    public ReactiveProperty<Sprite> StageImage = new(); // 스테이지 이미지
    public void Start()
    {
        SetData();
    }

    private void SetData()
    {
        var stageData = DBManager.Instance.GetStageData(CurrentStage.Value);

        StageName.Value = stageData.name;
        StageDescription.Value = stageData.stageDescription;
        StageImage.Value = stageData.stageImage;
    }

    /// <summary>
    /// 이전 스테이지로 변경
    /// </summary>
    public void PrevStage()
    {
        CurrentStage.Value--;
        SetData();
    }

    /// <summary>
    /// 다음 스테이지로 변경
    /// </summary>
    public void NextStage()
    {
        CurrentStage.Value++;
        SetData();
    }

    /// <summary>
    /// 게임 시작
    /// </summary>
    public void GameStart()
    {
        GameManager.Instance.CurrentStage = CurrentStage.Value; // 게임Scene까지 살아갈 수 있는 매니저 중 이 친구가 그나마 괜찮을 것 같아서 진행 => 더 좋은 방안 찾으면 변경 예정
        EventManager.Instance.Publish(LobbyEventType.JoinGameScene);
    }

    /// <summary>
    /// 데이터 리셋
    /// </summary>
    public void Reset()
    {
        CurrentStage.Value = 1;
        StageName.Value = "";
        StageDescription.Value = "";
        StageImage.Value = null;
    }

    public void Destroy()
    {
        CurrentStage = null;

        StageName = null;
        StageDescription = null;
        StageImage = null;
    }
}
