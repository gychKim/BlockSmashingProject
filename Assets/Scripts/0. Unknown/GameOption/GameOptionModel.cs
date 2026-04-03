using System;
using UnityEngine;

public class GameOptionModel : BaseModel
{
    private Guid openGameExitUIEventKey;
    public override void Start()
    {
        // 여기서 뒤로가기 누를 시 게임 종료 UI Open하는 로직 구현
        // 이벤트 구독
        //openGameExitUIEventKey = EventManager.Instance.Subscribe(UIEventType.OpenGameExitUI, OpenGameExitUI, null); // 게임종료 UI Open이벤트 구독
    }

    public override void Destroy()
    {
        EventManager.Instance.Unsubscribe(UIEventType.OpenGameExitUI, openGameExitUIEventKey); // 게임종료 UI Open이벤트 구독해제
    }
}
