using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelManager : MonoBehaviour, IDataInitialize
{
    /// <summary>
    /// 뒷배경 스프라이트
    /// </summary>
    [SerializeField]
    private List<SpriteRenderer> backgroundSpriteList;

    /// <summary>
    /// 캐릭터
    /// </summary>
    [SerializeField]
    private CharacterControllerEX characterController;

    [SerializeField]
    private BlockViewController blockViewController;

    private Guid reStartGameEventKey, StartPrevDirectionEventKey, endPrevDirectionEventKey, gameEndEventKey, startAfterDirectionEventKey, endAfterDirectionEventKey, breakingBlockEventKey, endBreakingBlockEventKey;

    public UniTaskCompletionSource LoadToken { get; } = new UniTaskCompletionSource();

    private void Awake()
    {
        reStartGameEventKey = EventManager.Instance.Subscribe(MainGameEventType.GameReStart, GameReStart);
        StartPrevDirectionEventKey = EventManager.Instance.Subscribe(GameSceneEventType.StartPrevDirection, StartPrevDirection);
        endPrevDirectionEventKey = EventManager.Instance.Subscribe(GameSceneEventType.EndPrevDirection, EndPrevDirection);

        gameEndEventKey = EventManager.Instance.Subscribe(MainGameEventType.GameEnd, GameEnd);

        startAfterDirectionEventKey = EventManager.Instance.Subscribe(GameSceneEventType.StartAfterDirection, StartAfterDirection);

        breakingBlockEventKey = EventManager.Instance.Subscribe(GameSceneEventType.BreakingBlock, BreakingBlock);
        endBreakingBlockEventKey = EventManager.Instance.Subscribe(GameSceneEventType.EndBreakingBlock, EndBreakingBlock);

        endAfterDirectionEventKey = EventManager.Instance.Subscribe(GameSceneEventType.EndAfterDirection, EndAfterDirection);
    }

    private void Start()
    {
        var stageData = DBManager.Instance.GetStageData(MainGameManager.Instance.CurrentStage.Value);
        foreach (var renderer in backgroundSpriteList)
        {
            renderer.sprite = stageData.stageImage;
        }

        LoadToken.TrySetResult();
    }

    /// <summary>
    /// 게임 재시작 시
    /// </summary>
    private void GameReStart()
    {
        blockViewController.GameReStart();
        characterController.GameReStart();
    }

    /// <summary>
    /// 시작 연출 실행 시
    /// </summary>
    private void StartPrevDirection()
    {
        blockViewController.StartPrevDirection();
        characterController.StartPrevDirection();
        // Background 변경
    }

    /// <summary>
    /// 시작 연출 종료 시
    /// </summary>
    private void EndPrevDirection()
    {
        blockViewController.EndPrevDirection();
        characterController.EndPrevDirection();
    }

    /// <summary>
    /// 게임 종료 시
    /// </summary>
    private void GameEnd()
    {
        EventManager.Instance.Publish(GameSceneEventType.StartAfterDirection); // Level매니저가 StartAfterDirection을 호출
    }

    /// <summary>
    /// 게임 종료 후 연출 시작 시
    /// </summary>
    private void StartAfterDirection()
    {
        blockViewController.StartAfterDirection();
        characterController.StartAfterDirection();
    }

    /// <summary>
    /// 블록 파괴 연출 시작 시
    /// </summary>
    private void BreakingBlock()
    {
        blockViewController.BreakingBlock();
        characterController.BreakingBlock();
    }

    /// <summary>
    /// 마지막 블록 파괴 시
    /// </summary>
    private void EndBreakingBlock()
    {
        blockViewController.EndBreakingBlock();
        characterController.EndBreakingBlock();
    }

    /// <summary>
    /// 게임 종료 후 연출 종료 시
    /// </summary>
    private void EndAfterDirection()
    {
        blockViewController.EndAfterDirection();
        characterController.EndAfterDirection();
    }

    private void OnDestroy()
    {
        EventManager.Instance.Unsubscribe(MainGameEventType.GameReStart, reStartGameEventKey);

        EventManager.Instance.Unsubscribe(GameSceneEventType.StartPrevDirection, StartPrevDirectionEventKey);
        EventManager.Instance.Unsubscribe(GameSceneEventType.EndPrevDirection, endPrevDirectionEventKey);
        EventManager.Instance.Unsubscribe(MainGameEventType.GameEnd, gameEndEventKey);
        EventManager.Instance.Unsubscribe(GameSceneEventType.StartAfterDirection, startAfterDirectionEventKey);
        EventManager.Instance.Unsubscribe(GameSceneEventType.EndAfterDirection, endAfterDirectionEventKey);
        EventManager.Instance.Unsubscribe(GameSceneEventType.EndBreakingBlock, endBreakingBlockEventKey);
        EventManager.Instance.Unsubscribe(GameSceneEventType.BreakingBlock, breakingBlockEventKey);
    }
}
