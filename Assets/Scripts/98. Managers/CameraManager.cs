using System.Collections.Generic;
using System;
using Unity.Cinemachine;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using System.Threading;

public enum CameraType
{
    StartScene,
    Character,
    FollowBlock
}

public enum CameraEffectType
{
    CrescendoShake, // 점차 강하게
    DiminuendoShake, // 점차 약하게
    ZoomIn, // 확대
    ZoomOut, // 축소
}
public class CameraManager : Singleton<CameraManager>, IDataAwake, IDataStart
{
    /// <summary>
    /// 시네머신 Brain
    /// </summary>
    [SerializeField]
    private CinemachineBrain brain;

    [SerializeField]
    private CinemachineCamera startSceneCam;
    [SerializeField]
    private CinemachineCamera characterCam;
    [SerializeField]
    private CinemachineCamera followBlockCam;

    [SerializeField]
    private CinemachineImpulseSource impulseSource;

    //private float camYOffset = 4f;
    private Guid reStartEventKey, startSceneEventKey, StartPrevDirectionEventKey, endPrevDirectionEventKey, breakingBlockEventKey, endBreakingBlockEventKey;
    protected override void Awake()
    {
        IsDontDestroyOnLoad = false;
        base.Awake();
    }


    public UniTask DataAwakeAsync(CancellationToken cancelToken)
    {
        reStartEventKey = EventManager.Instance.Subscribe(MainGameEventType.GameReStart, GameReStart);
        startSceneEventKey = EventManager.Instance.Subscribe(GameSceneEventType.StartScene, () => { StartScene().Forget(); });
        StartPrevDirectionEventKey = EventManager.Instance.Subscribe(GameSceneEventType.StartPrevDirection, StartPrevDirection);
        endPrevDirectionEventKey = EventManager.Instance.Subscribe(GameSceneEventType.EndPrevDirection, EndPrevDirection);

        breakingBlockEventKey = EventManager.Instance.Subscribe(GameSceneEventType.BreakingBlock, BreakingBlock);

        endBreakingBlockEventKey = EventManager.Instance.Subscribe(GameSceneEventType.EndBreakingBlock, EndBreakingBlock);

        return UniTask.CompletedTask;
    }

    public UniTask DataStartAsync(CancellationToken cancelToken)
    {
        ActiveCamera(CameraType.StartScene);

        return UniTask.CompletedTask;
    }

    /// <summary>
    /// 최초 스테이지 시작 시
    /// </summary>
    private async UniTask StartScene()
    {
        ActiveCamera(CameraType.Character);

        await UniTask.Delay(TimeSpan.FromSeconds(3.5f));

        EventManager.Instance.Publish(GameSceneEventType.StartPrevDirection);
    }

    /// <summary>
    /// 게임 재시작 시 카메라가 해야 할 일
    /// </summary>
    private void GameReStart()
    {
        ActiveCamera(CameraType.Character);
        //characterCam.transform.position = cameraBottomPos.position;
    }

    /// <summary>
    /// 게임 시작 전 연출 시작 시
    /// </summary>
    private void StartPrevDirection()
    {
        ActiveCamera(CameraType.Character);
    }

    /// <summary>
    /// 게임 시작 전 연출 종료 시
    /// </summary>
    private void EndPrevDirection()
    {
        
    }

    /// <summary>
    /// 블록파괴 연출 시작 시
    /// </summary>
    private void BreakingBlock()
    {
        ActiveCamera(CameraType.FollowBlock);
    }

    /// <summary>
    /// 마지막 블록 파괴될 시
    /// </summary>
    private void EndBreakingBlock()
    {
        ShakeCamera(1f, 2.5f, CameraEffectType.DiminuendoShake);
    }

    public void ActiveCamera(CameraType type)
    {
        switch(type)
        {
            case CameraType.StartScene:
                startSceneCam.Priority = 1;
                characterCam.Priority = 0;
                followBlockCam.Priority = 0;
                break;
            case CameraType.Character:
                startSceneCam.Priority = 0;
                characterCam.Priority = 1;
                followBlockCam.Priority = 0;
                break;
            case CameraType.FollowBlock:
                startSceneCam.Priority = 0;
                characterCam.Priority = 0;
                followBlockCam.Priority = 1;
                break;
        }
    }

    public void ShakeCamera(float amplitude, float duration, CameraEffectType effectType)
    {
        Sequence seq = DOTween.Sequence();

        switch (effectType)
        {
            case CameraEffectType.CrescendoShake:
                seq.AppendCallback(() => impulseSource.GenerateImpulse(0));
                seq.AppendInterval(duration / 2f);
                seq.AppendCallback(() => impulseSource.GenerateImpulse(Mathf.Lerp(0, amplitude, duration / 2f)));
                seq.AppendInterval(duration / 2f);
                seq.AppendCallback(() => impulseSource.GenerateImpulse(amplitude));
                break;
            case CameraEffectType.DiminuendoShake:
                seq.AppendCallback(() => impulseSource.GenerateImpulse(amplitude));
                seq.AppendInterval(duration);
                seq.AppendCallback(() => impulseSource.GenerateImpulse(0));
                break;
        }

        seq.Play();
    }

    private void OnDestroy()
    {
        EventManager.Instance.Unsubscribe(MainGameEventType.GameReStart, reStartEventKey);
        EventManager.Instance.Unsubscribe(GameSceneEventType.StartScene, startSceneEventKey);
        EventManager.Instance.Unsubscribe(GameSceneEventType.StartPrevDirection, StartPrevDirectionEventKey);
        EventManager.Instance.Unsubscribe(GameSceneEventType.EndPrevDirection, endPrevDirectionEventKey);
        EventManager.Instance.Unsubscribe(GameSceneEventType.BreakingBlock, breakingBlockEventKey);
        EventManager.Instance.Unsubscribe(GameSceneEventType.EndBreakingBlock, endBreakingBlockEventKey);
    }
}
