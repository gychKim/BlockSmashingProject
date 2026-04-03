using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;

public interface IDataInitialize { }
public interface IDataAwake : IDataInitialize
{
    UniTask DataAwakeAsync(System.Threading.CancellationToken cancelToken);
}
public interface IDataStart : IDataInitialize
{
    UniTask DataStartAsync(System.Threading.CancellationToken cancelToken);
}
/// <summary>
/// 이벤트 인터페이스
/// </summary>
public interface IEvent
{
    public Guid EventID { get; } // 이벤트 고유 ID
    void Publish(params object[] args); // 이벤트 구독
}

/// <summary>
/// UIView 인터페이스
/// </summary>
public interface IUIView
{
    GameObject RootObject { get; } // 최상위 UI 오브젝트
    CompositeDisposable Disposables { get; } // Dispose를 묶은 객체
}

/// <summary>
/// Presenter 인터페이스
/// </summary>
/// <typeparam name="TView"></typeparam>
/// <typeparam name="TModel"></typeparam>
public interface IPresenter<TView, TModel> where TView : IUIView
{
    /// <summary>
    /// Bind,
    /// view와 model간 필요한 로직 및 데이터를 연결시켜준다.
    /// </summary>
    /// <param name="view"></param>
    /// <param name="model"></param>
    void Bind();

    /// <summary>
    /// Presenter 파괴 시
    /// </summary>
    void Destroy();
}

public interface IModel
{
    /// <summary>
    /// Start 시
    /// </summary>
    void Start();

    /// <summary>
    /// Model 파괴 시
    /// </summary>
    void Destroy();
}

/// <summary>
/// Scene 초기화
/// </summary>
public interface ISceneInitializer
{
    /// <summary>
    /// Scene 초기화(데이터 Load 등 작업)
    /// </summary>
    /// <returns></returns>
    UniTask InitAsync();

    /// <summary>
    /// Scene전환이 완료 시 호출(LoadingUI의 Close까지 끝났을 시)
    /// </summary>
    void LoadComplete();
}

public interface ISaveManager
{
    void Init();
    void Save(JsonData jsonData, bool isForce = false);
    void Load(JsonData jsonData);

}

public interface IPoolable
{
    /// <summary>
    /// 풀에서 꺼낼 때
    /// </summary>
    void Get();

    /// <summary>
    /// 풀에 집어넣을 때
    /// </summary>
    void Release();

    /// <summary>
    /// 파괴될 때
    /// </summary>
    void Destroy();
}

/// <summary>
/// 재화 인터페이스
/// </summary>
public interface ICurrencyService
{
    // 일단 따라하고, 나중에 CurrentGold의 ReactiveProperty를 구독하도록
    void AddGold(int amount);
    void AddDiamond(int amount);
}


/// <summary>
/// 인벤토리 인터페이스
/// </summary>
public interface IInventoryService
{
    // 일단 따라하고, 나중에 ReactiveProperty(??)를 구독하도록
    void AddItem(string itemId, int qty);
}

/// <summary>
/// 퀘스트 조건 인터페이스
/// </summary>
public interface IQuestCondition
{
    /// <summary>
    /// 퀘스트 시작 시
    /// </summary>
    void OnStart();

    /// <summary>
    /// 퀘스트 이벤트 데이터를 받아 각자 처리한다.
    /// </summary>
    /// <param name="qusetEvent"></param>
    void OnEvent(in QuestEvent qusetEvent);

    /// <summary>
    /// 퀘스트 진행도
    /// </summary>
    /// <returns></returns>
    int GetProgress();

    /// <summary>
    /// 퀘스트 완료 여부
    /// </summary>
    /// <param name="targetValue"></param>
    /// <returns></returns>
    bool IsCompleted(int targetValue);
}

public interface IQuestConditionRuntime
{
    void OnStart();
    void OnEvent(in QuestEvent e);
    int GetProgress();                // 0..target
    string GetItemName();
    bool IsCompleted(int targetValue);
    string SaveState();                // 선택(일일 지속 필요 시)
    void LoadState(string json);
}
