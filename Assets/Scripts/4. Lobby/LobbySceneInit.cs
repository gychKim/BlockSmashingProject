using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class LobbySceneInit : MonoBehaviour, ISceneInitializer
{
    private async void Start()
    {
        await InitAsync(); // Scene 초기화 대기
    }

    public async UniTask InitAsync()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(3)); // 데이터 가져오기 등 진행 > 지금은 그냥 5초 대기로 임시구현

        SceneManagerEX.Instance.InitScene(this); // Scene 초기화 완료
    }

    public void LoadComplete()
    {
        //EventManager.Instance.Publish(GameSceneEventType.StartPrevDirection); // 연출 대기
    }
}
