using DG.Tweening;
using System;
using UniRx.Triggers;
using UnityEngine;

public class RootPresenter : IPresenter<RootView, RootModel>
{
    private RootView view;
    private RootModel model;

    private Guid joinGameScene;

    public RootPresenter(RootView view, RootModel model)
    {
        this.view = view;
        this.model = model;

        // 이벤트 구독
        joinGameScene = EventManager.Instance.Subscribe(LobbyEventType.JoinGameScene, JoinGameScene); // 게임Scene으로 전환
    }

    public void Bind()
    {
        
    }

    /// <summary>
    /// 게임Scene으로 전환
    /// </summary>
    private void JoinGameScene()
    {
        SceneManagerEX.Instance.LoadScene("GameScene");
        //view.JoinMainGame();
    }

    /// <summary>
    /// 메인게임 종료 시 호출
    /// </summary>
    private void GameExit()
    {
        //view.JoinMain();
    }

    public void Destroy()
    {
        EventManager.Instance.Unsubscribe(LobbyEventType.JoinGameScene, joinGameScene);
        //EventManager.Instance.Unsubscribe(MainGameEventType.GameExit, gameExitEventKey);
    }
}
