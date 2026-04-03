using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UniRx;
using UnityEngine;

public class ItemController
{
    private MainGameController controller;
    private ItemMediator itemMediator; // 아이템 중재자

    [HeaderColor("아이템 데이터", 171, 231, 158)]
    [ReadOnly] public IntReactiveProperty CurrentShieldCount = new IntReactiveProperty(); // 현재 실드 개수
    [ReadOnly] public FloatReactiveProperty CurrentFeverTimer = new FloatReactiveProperty(); // 피버 타이머
    [ReadOnly] public FloatReactiveProperty CurrentStopGameTimer = new FloatReactiveProperty(); // 게임 시간 정지 타이머
    [ReadOnly] public FloatReactiveProperty CurrentItemSpawnChanceTimer = new FloatReactiveProperty(); // 아이템 획득 확률 타이머

    private CancellationTokenSource feverCancelToken; // 피버 CancelToken
    private CancellationTokenSource itemSpawnChanceCancelToken; // 아이템 획득 확률 증가 CancelToken
    private CancellationTokenSource stopGameTimeCancelToken; // 게임정지Task의 CancelToken

    private FloatTimerAsync feverTimerAsync;
    private FloatTimerAsync itemSpawnChanceTimerAsync;
    private IntTimerAsync stopGameTimerAsync;

    private BoolReactiveProperty isPause => controller.isPause;
    public BoolReactiveProperty isFever = new BoolReactiveProperty(false); // 피버타임에 돌입했는지 > 블록 생성에 큰 영향을 준다.

    private IDisposable shieldDispoable; // 실드 전용 이벤트 구독 해제
    public float ItemSpawnChance => itemSpawnChance;
    private float itemSpawnChance = 10f;

    private Guid setScoreEventKey, addScoreMultiplierEventKey, setGameTimerEventKey,
        stopGameTimerEventKey, shieldEventKey, feverEventKey, itemSpawnChanceEventKey;
    public ItemController(MainGameController controller)
    {
        this.controller = controller;
        itemMediator = new ItemMediator();

        setScoreEventKey = EventManager.Instance.Subscribe<EffectType, int>(EffectType.Score, SetScore);
        addScoreMultiplierEventKey = EventManager.Instance.Subscribe<EffectType, float, float>(EffectType.ScoreMultiplier, AddScoreMultiplier);
        setGameTimerEventKey = EventManager.Instance.Subscribe<EffectType, int>(EffectType.Time, SetGameTimer);
        stopGameTimerEventKey = EventManager.Instance.Subscribe<EffectType, float>(EffectType.TimeStop, StopGameTimer);
        shieldEventKey = EventManager.Instance.Subscribe<EffectType, int>(EffectType.Shield, Shield);
        feverEventKey = EventManager.Instance.Subscribe<EffectType, float>(EffectType.Fever, ApplyFever);
        itemSpawnChanceEventKey = EventManager.Instance.Subscribe<EffectType, float, float>(EffectType.ItemSpawnChance, ApplyItemSpawnChance);

        FixedUpdateTask().Forget(); // FixedUpdate를 실행하는 Task 생성
    }

    private async UniTask FixedUpdateTask()
    {
        while (true)
        {
            if (!isPause.Value)
            {
                itemMediator.Update(Time.deltaTime);
            }
            await UniTask.WaitForFixedUpdate();
        }
    }

    public void Init()
    {
        stopGameTimeCancelToken = new CancellationTokenSource();
        feverCancelToken = new CancellationTokenSource();
        itemSpawnChanceCancelToken = new CancellationTokenSource();

        feverTimerAsync = new FloatTimerAsync(() => isPause.Value, feverCancelToken);
        itemSpawnChanceTimerAsync = new FloatTimerAsync(() => isPause.Value, itemSpawnChanceCancelToken);
        stopGameTimerAsync = new IntTimerAsync(() => isPause.Value, stopGameTimeCancelToken);

    }

    private void SetScore(int value)
    {
        if (value == 1)
            controller.AddScore();
        else
            controller.RemoveScore();
    }

    /// <summary>
    /// 점수배율 갱신
    /// </summary>
    private void UpdateScoreMultiplier()
    {
        var q = new ItemQuery(EffectType.ScoreMultiplier, 1);
        itemMediator.PerformQuery(this, q);
        controller.SetScoreMultiplier(q.value);
    }

    /// <summary>
    /// 점수 배율 중재자 추가
    /// </summary>
    /// <param name="value"></param>
    /// <param name="duration"></param>
    public void AddScoreMultiplier(float value, float duration)
    {
        var modifier = new TimeItemModifier(EffectType.ScoreMultiplier, duration, v => v + value);

        itemMediator.AddModifier(modifier); // 먼저 추가해야, OnDispose할 때 Mediator가 Modifier 연결 리스트에서 해당 modifier 제거를 먼저 한다.

        // 이 부분이 좀 불안하다.
        // 지금은 Add이후에 이벤트를 구독해야함을 알고있고 순서도 2개밖에 없어 괜찮지만, 나중에 많아지면 감당할 수 없게 될것같다.
        // 
        modifier.OnDispose += (m) =>
        {
            UpdateScoreMultiplier(); // 제거되면 제거된 이후의 정보를 갱신한다.
        };

        UpdateScoreMultiplier();
    }

    /// <summary>
    /// 게임 시간 설정
    /// </summary>
    /// <param name="value"></param>
    public void SetGameTimer(int value)
    {
        controller.SetGameTimer(value);
    }

    /// <summary>
    /// 게임시간 정지
    /// </summary>
    /// <param name="duration"></param>
    public async void StopGameTimer(float duration)
    {
        controller.SetGameTimeSpeed(0);

        await StopGameTimerAsync(duration);

        controller.SetGameTimeSpeed(1);
    }

    /// <summary>
    /// 게임시간 정지
    /// </summary>
    /// <param name="duration"></param>
    private async UniTask StopGameTimerAsync(float duration)
    {
        // Token Cancel 실행
        stopGameTimeCancelToken?.Cancel();
        stopGameTimeCancelToken = new CancellationTokenSource();
        stopGameTimerAsync = new IntTimerAsync(() => isPause.Value, stopGameTimeCancelToken);

        CurrentStopGameTimer.Value = duration;

        try
        {
            var task = stopGameTimerAsync.Start(duration);

            stopGameTimerAsync.Value
                .Subscribe(value =>
                {
                    CurrentStopGameTimer.Value = value;
                });

            await task;
        }
        catch (OperationCanceledException)
        {
            DebugX.Log("StopGameTime Cancel");
            return;
        }

        CurrentStopGameTimer.Value = 0;
    }

    /// <summary>
    /// 피버 적용
    /// </summary>
    /// <param name="duration"></param>
    public async void ApplyFever(float duration)
    {
        // Token Cancel 실행
        feverCancelToken?.Cancel();
        feverCancelToken = new CancellationTokenSource();
        feverTimerAsync = new FloatTimerAsync(() => isPause.Value, feverCancelToken);

        isFever.Value = true;
        CurrentFeverTimer.Value = 1f; // UI 갱신

        controller.ApplyFever(); // isFever를 변환한 다음 실행해야한다.

        try
        {
            var task = feverTimerAsync.Start(duration);

            feverTimerAsync.Value
                .Subscribe(value =>
                {
                    CurrentFeverTimer.Value = value / duration;
                });

            await task;
        }
        catch (OperationCanceledException)
        {
            DebugX.Log("Fever Cancel");
            return;
        }

        CurrentFeverTimer.Value = 0;
        isFever.Value = false;
    }

    /// <summary>
    /// 실드 아이템 적용
    /// </summary>
    public void Shield(int value)
    {
        //model.Shield();

        CurrentShieldCount.Value += value;
        //CurrentShieldCount.Value = Mathf.Clamp(CurrentShieldCount.Value += value, 0, 10);

        if (shieldDispoable != null)
            shieldDispoable.Dispose();

        shieldDispoable = CurrentShieldCount
            .Subscribe(count =>
            {
                if (count <= 0)
                    RemoveShield();
            });
    }

    /// <summary>
    /// 실드 사용
    /// </summary>
    /// <returns></returns>
    public bool UseShield()
    {
        if(CurrentShieldCount.Value > 0)
        {
            CurrentShieldCount.Value--;
            EventManager.Instance.Publish(MainGameEventType.HitShield, CurrentShieldCount.Value);
            return true;
        }

        EventManager.Instance.Publish(MainGameEventType.HitShield, 0);
        return false;
    }

    /// <summary>
    /// 실드 제거
    /// </summary>
    private void RemoveShield()
    {
        //model.RemoveShield();

        if(shieldDispoable != null)
        {
            shieldDispoable?.Dispose();
            shieldDispoable = null;
        }
    }

    /// <summary>
    /// 아이템 스폰 확률
    /// </summary>
    /// <param name="value"></param>
    /// <param name="duration"></param>
    public async void ApplyItemSpawnChance(float value, float duration)
    {
        // 지속시간이 -1보다 낮으면 해당 게임동안 지속이라는 뜻
        if(duration <= -1)
        {
            itemSpawnChance += value; // 아이템 습득 확률 변경
            CurrentItemSpawnChanceTimer.Value = 1f; // UI 갱신
            return;
        }

        // Token Cancel 실행
        itemSpawnChanceCancelToken?.Cancel();
        itemSpawnChanceCancelToken = new CancellationTokenSource();
        itemSpawnChanceTimerAsync = new FloatTimerAsync(() => isPause.Value, itemSpawnChanceCancelToken);

        itemSpawnChance += value; // 아이템 습득 확률 변경

        CurrentItemSpawnChanceTimer.Value = 1f; // UI 갱신

        try
        {
            var task = itemSpawnChanceTimerAsync.Start(duration);

            itemSpawnChanceTimerAsync.Value
                .Subscribe(value =>
                {
                    CurrentItemSpawnChanceTimer.Value = value / duration;
                });

            await task;
        }
        catch (OperationCanceledException)
        {
            DebugX.Log("ItemSpawnChance Cancel");
            return;
        }

        itemSpawnChance -= value;
    }

    public void ApplyShopItem(BlockItemData item)
    {
        foreach(var effect in item.effectConfigList)
        {
            effect.effectData.Execute(effect);
        }
    }

    public void RemoveAllModifier(EffectType type)
    {
        itemMediator.RemoveAllModifier(type);
    }

    public void Clear(bool isDispose = false)
    {
        stopGameTimeCancelToken?.Cancel();
        feverCancelToken?.Cancel();
        itemSpawnChanceCancelToken?.Cancel();

        if(isDispose)
        { 
            stopGameTimeCancelToken = null;
            feverCancelToken = null;
            itemSpawnChanceCancelToken = null;
            feverTimerAsync = null;
            stopGameTimerAsync = null;

            EventManager.Instance.Unsubscribe(EffectType.Score, setScoreEventKey);
            EventManager.Instance.Unsubscribe(EffectType.ScoreMultiplier, addScoreMultiplierEventKey);
            EventManager.Instance.Unsubscribe(EffectType.Time, setGameTimerEventKey);
            EventManager.Instance.Unsubscribe(EffectType.TimeStop, stopGameTimerEventKey);
            EventManager.Instance.Unsubscribe(EffectType.Shield, shieldEventKey);
            EventManager.Instance.Unsubscribe(EffectType.Fever, feverEventKey);
            EventManager.Instance.Unsubscribe(EffectType.ItemSpawnChance, itemSpawnChanceEventKey);
        }
        else
        {
            stopGameTimeCancelToken = new CancellationTokenSource();
            feverCancelToken = new CancellationTokenSource();
            itemSpawnChanceCancelToken = new CancellationTokenSource();

            feverTimerAsync = new FloatTimerAsync(() => isPause.Value, feverCancelToken);
            itemSpawnChanceTimerAsync = new FloatTimerAsync(() => isPause.Value, itemSpawnChanceCancelToken);
            stopGameTimerAsync = new IntTimerAsync(() => isPause.Value, stopGameTimeCancelToken);

        }

        CurrentShieldCount.Value = 0;
        EventManager.Instance.Publish(MainGameEventType.HitShield, 0);

        CurrentFeverTimer.Value = 0;
        CurrentStopGameTimer.Value = 0;
        CurrentItemSpawnChanceTimer.Value = 0;

        itemSpawnChance = 10f;

        isFever.Value = false;

        itemMediator.Clear();
    }
}
