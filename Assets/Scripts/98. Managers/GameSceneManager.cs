using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class GameSceneManager : Singleton<GameSceneManager>
{
    protected override void Awake()
    {
        base.Awake();


    }

    /// <summary>
    /// 로딩 시작
    /// </summary>
    private void StartLoading()
    {
        //LoadingManager.Instance.SetCondition(LoadGameSceneDataAsync());
        //LoadingManager.Instance.StartLoading();
    }

    /// <summary>
    /// 게임 입장 시 호출 > Loading이 끝나면 호출
    /// </summary>
    private void JoinGame()
    {
        EventManager.Instance.Publish(GameSceneEventType.StartPrevDirection); // 게임 입장했음을 알림 > 연출 대기 호출
    }

    /// <summary>
    /// 게임Scene에 필요한 데이터를 가져오는 중
    /// </summary>
    /// <returns></returns>
    private UniTask LoadGameSceneDataAsync()
    {
        // 자식들 중 데이터를 Load하는 함수를 여기서 전부 await 시켜준다.
        return UniTask.Delay(TimeSpan.FromSeconds(5)); // 5초 대기
    }
}
