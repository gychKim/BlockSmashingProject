using System;
using UnityEngine;

public class GameAfterModel : IModel
{
    //private Guid endAfterDirectionEventKey;

    public void Start()
    {
        //endAfterDirectionEventKey = EventManager.Instance.Subscribe(GameSceneEventType.EndAfterDirection, EndDirection);
    }

    /// <summary>
    /// 블록 파괴 연출 종료 시
    /// </summary>
    /// <param name="result"></param>
    public void EndDirection()
    {
        // 성공했다면
        if(MainGameManager.Instance.CurrentGameResult)
        {
            int currentStage = MainGameManager.Instance.CurrentStage.Value;
            var stageReward = DBManager.Instance.GetStageData(currentStage).reward;

            if(stageReward.gold > 0)
            {
                GameManager.Instance.AddGold(stageReward.gold + currentStage * MainGameManager.Instance.CurrentGameTime);
            }
                

            if (stageReward.dia > 0)
                GameManager.Instance.AddDia(stageReward.dia);

        }
    }

    public void Destroy()
    {
        //EventManager.Instance.Unsubscribe(GameSceneEventType.EndAfterDirection, endAfterDirectionEventKey);
    }
}
