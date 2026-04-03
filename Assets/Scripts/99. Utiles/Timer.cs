using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class Timer
{
    protected float initialTime;
    public float Time { get; set; }
    public bool IsRunning { get; protected set; }

    public float Progress => Time / initialTime;

    public Action OnTimerStart = delegate { };
    public Action OnTimerStop = delegate { };

    protected Timer(float value)
    {
        initialTime = value;
        IsRunning = false;
    }

    public void Start()
    {
        Time = initialTime;
        if (!IsRunning)
        {
            IsRunning = true;
            OnTimerStart.Invoke();
        }
    }

    public void Stop()
    {
        if (IsRunning)
        {
            IsRunning = false;
            OnTimerStop.Invoke();
        }
    }

    public void Resume() => IsRunning = true;
    public void Pause() => IsRunning = false;

    public abstract void Tick(float deltaTime);
}

/// <summary>
/// Task로 동작하는 카운트 다운 타이머
/// </summary>
public abstract class TimerTask
{
    protected float initialTime; // 최초 시간
    public float Time { get; set; } // 현재 시간
    public bool IsRunning { get; protected set; } // 동작 여부

    public float Progress => Time / initialTime; // 진행도

    public Action OnTimerStart = delegate { }; // 시작 이벤트
    public Action OnTimerStop = delegate { }; // 종료 이벤트

    private CancellationTokenSource cancelToken; // 종료 토큰
    protected TimerTask(float value)
    {
        initialTime = value;
        IsRunning = false;
    }

    public void Start()
    {
        Time = initialTime;
        if (!IsRunning)
        {
            cancelToken = new CancellationTokenSource();
            IsRunning = true;
            OnTimerStart.Invoke();
            Run().Forget();
        }
    }

    public void Stop()
    {
        if (IsRunning)
        {
            cancelToken.Cancel(true);
            IsRunning = false;
            OnTimerStop.Invoke();
        }
    }

    public void Resume() => IsRunning = true;
    public void Pause() => IsRunning = false;

    private async UniTask Run()
    {
        try
        {
            while (Time > 0)
            {
                if (!IsRunning)
                {
                    Time -= UnityEngine.Time.deltaTime;
                }

                await UniTask.Yield(PlayerLoopTiming.Update, cancelToken.Token);
            }
        }
        catch (OperationCanceledException)
        {
            Stop();
            DebugX.Log("TimerTask Cancel");
            throw;
        }
        Stop();
    }
}

/// <summary>
/// ReactiveProperty전용 타이머
/// </summary>
public abstract class TimerAsync
{
    public bool IsRunning { get; protected set; } // 진행중인가

    protected Func<bool> isPauseFunc; // 게임정지 시 콜백처리

    protected CancellationTokenSource cancelToken; // 정지 토큰

    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="isPauseFunc"></param>
    /// <param name="cancelToken"></param>
    public TimerAsync(Func<bool> isPauseFunc, CancellationTokenSource cancelToken)
    {
        this.isPauseFunc = isPauseFunc;
        this.cancelToken = cancelToken;
    }

    /// <summary>
    /// 지속 시간 갱신
    /// </summary>
    /// <param name="duration"></param>
    public abstract void UpdateDuration(float duration);
    /// <summary>
    /// 토큰 갱신 > 토큰 사용하면 반드시 갱신해야한다.
    /// </summary>
    /// <param name="cancelToken"></param>
    public abstract void UpdateCancelToken(CancellationTokenSource cancelToken);

    /// <summary>
    /// 타이머 시작
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    public abstract UniTask Start(float duration);
}

public class CountdownTimer : Timer
{
    public CountdownTimer(float value) : base(value) { }

    public override void Tick(float deltaTime)
    {
        if (IsRunning && Time > 0)
        {
            Time -= deltaTime;
        }

        if (IsRunning && Time <= 0 && Time > -1)
        {
            Stop();
        }
    }

    public bool IsFinished => Time <= 0;

    public void Reset() => Time = initialTime;

    public void Reset(float newTime)
    {
        initialTime = newTime;
        Reset();
    }
}

public class CountdownTimerTask : TimerTask
{
    public CountdownTimerTask(float value) : base(value) { }

    public bool IsFinished => Time <= 0;

    public void Reset() => Time = initialTime;

    public void Reset(float newTime)
    {
        initialTime = newTime;
        Reset();
    }
}

public class StopwatchTimer : Timer
{
    public StopwatchTimer() : base(0) { }

    public override void Tick(float deltaTime)
    {
        if (IsRunning)
        {
            Time += deltaTime;
        }
    }

    public void Reset() => Time = 0;

    public float GetTime() => Time;
}



/// <summary>
/// Int타입 ReactiveProperty의 값을 1초당 1씩 줄이는 타이머
/// </summary>
public class IntTimerAsync : TimerAsync
{
    public IObservable<int> Value => reactiveProperty.AsObservable(); // 시간 체크 반응

    public int RemainingTime => reactiveProperty.Value; // 남은 시간

    private IntReactiveProperty reactiveProperty;

    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="reactiveProperty"></param>
    /// <param name="isPauseFunc"></param>
    /// <param name="cancelToken"></param>
    public IntTimerAsync(Func<bool> isPauseFunc, CancellationTokenSource cancelToken) : base(isPauseFunc, cancelToken)
    {
        reactiveProperty = new IntReactiveProperty();
    }

    /// <summary>
    /// 토큰 갱신 > 토큰 사용하면 반드시 갱신해야한다.
    /// </summary>
    /// <param name="cancelToken"></param>
    public override void UpdateCancelToken(CancellationTokenSource cancelToken)
    {
        this.cancelToken = cancelToken;
    }

    /// <summary>
    /// 남은 시간 갱신
    /// </summary>
    /// <param name="duration"></param>
    public override void UpdateDuration(float duration)
    {
        reactiveProperty.Value = (int)duration;
    }

    /// <summary>
    /// 시작
    /// duration동안 실행된다.
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    public override async UniTask Start(float time)
    {
        float count = 1f;
        IsRunning = true;
        reactiveProperty.Value = (int)time;

        try
        {
            while (reactiveProperty.Value > 0)
            {
                if (!isPauseFunc())
                {
                    count -= Time.deltaTime;

                    if (count <= 0 && reactiveProperty != null)
                    {
                        reactiveProperty.Value--;
                        count = 1f;
                    }
                }

                await UniTask.Yield(PlayerLoopTiming.Update, cancelToken.Token);

                // cancelToken.Token.ThrowIfCancellationRequested();
                // 이런식으로 취소토큰을 사용하면, 아이템 재사용인 경우엔
                // await에서 대기(Timer) > 취소(Model) > 새로운 토큰 받음(Model > Timer) > await종료(Timer) > ThrowIfCancellationRequested확인(Timer) > 새로운 Token으로 대체했으니 cancel이 false임 > 계속 진행
                // 위와 같이 되버려서, 제대로 종료가 안된다.
                // 그래서 취소 토큰을 받으면 즉시 취소가 되도록, 대기할 때 취소토큰을 적용시켜야 한다.
            }
        }
        catch(OperationCanceledException)
        {
            reactiveProperty.Dispose();
            IsRunning = false;
            DebugX.Log("Int Timer Cancel");
            throw;
        }
    }
}

/// <summary>
/// Float타입 ReactiveProperty의 값을 1초당 1씩 줄이는 타이머
/// </summary>
public class FloatTimerAsync : TimerAsync
{
    public IObservable<float> Value => reactiveProperty.AsObservable(); // 시간 체크 반응

    public float RemainingTime => reactiveProperty.Value; // 남은 시간

    private FloatReactiveProperty reactiveProperty;
    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="reactiveProperty"></param>
    /// <param name="isPauseFunc"></param>
    /// <param name="cancelToken"></param>
    public FloatTimerAsync(Func<bool> isPauseFunc, CancellationTokenSource cancelToken) : base(isPauseFunc, cancelToken)
    {
        reactiveProperty = new FloatReactiveProperty();
    }

    /// <summary>
    /// 토큰 갱신 > 토큰 사용하면 반드시 갱신해야한다.
    /// </summary>
    /// <param name="cancelToken"></param>
    public override void UpdateCancelToken(CancellationTokenSource cancelToken)
    {
        this.cancelToken = cancelToken;
    }

    /// <summary>
    /// 남은 시간 갱신
    /// </summary>
    /// <param name="duration"></param>
    public override void UpdateDuration(float duration)
    {
        reactiveProperty.Value = duration;
    }

    /// <summary>
    /// 시작
    /// duration동안 실행된다.
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    public override async UniTask Start(float time)
    {
        IsRunning = true;
        reactiveProperty.Value = time;

        try
        {
            while (reactiveProperty.Value > 0)
            {
                if (!isPauseFunc())
                {
                    reactiveProperty.Value -= Time.deltaTime;
                }

                await UniTask.Yield(PlayerLoopTiming.Update, cancelToken.Token);
            }
        }
        catch(OperationCanceledException)
        {
            reactiveProperty.Dispose();
            IsRunning = false;
            DebugX.Log("Float Timer Cancel");
            throw;
        }
    }
}
