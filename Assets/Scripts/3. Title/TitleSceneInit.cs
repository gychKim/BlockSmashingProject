using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

// TitleScene은 게임 시작하자마자 나오는 Scene이라 Scene전환이 없음 > 그래서 ISceneInit을 상속받지 않는것 > 후에 LogoScene나오면 TitleScene도 ISceneInit을 상속받아야 한다.
public class TitleSceneInit : MonoBehaviour
{
    public ReactiveProperty<float> LoadValue = new(); // 총 비동기 로딩 개수와 완료된 비동기 개수의 비율

    [SerializeField]
    private List<GameObject> dataManagerList;

    private List<IDataAwake> awakeDataList = new(); // Awake 필요한 매니저 들
    private List<IDataStart> startDataList = new(); // Start 필요한 매니저 들

    private async void Awake()
    {
        var cancelToken = this.GetCancellationTokenOnDestroy(); // 현 객체 파괴시 취소 토큰

        try
        {
            foreach (var obj in dataManagerList)
            {
                if (obj == null)
                    continue;

                var data = obj.GetComponent<IDataInitialize>();

                if (data == null)
                    continue;

                if (data is IDataAwake awake)
                    awakeDataList.Add(awake);
                if (data is IDataStart start)
                    startDataList.Add(start);
            }

            await LoadAwakeDataAsync(cancelToken);

            await LoadStartDataAsync(cancelToken);

            DebugX.CyanLog($"{name}이 모든 매니저 초기화 완료");
        }
        catch (OperationCanceledException)
        {
            Debug.LogWarning($"{this.name} canceled.");
        }
        catch (Exception e)
        {
            Debug.LogError($"{this.name} init failed: {e}");
            throw;
        }
    }

    /// <summary>
    /// Awake처리
    /// </summary>
    /// <returns></returns>
    private async UniTask LoadAwakeDataAsync(System.Threading.CancellationToken cancelToken)
    {
        var taskList = new List<UniTask>(awakeDataList.Count);

        foreach (var x in awakeDataList)
            taskList.Add(x.DataAwakeAsync(cancelToken));

        await UniTask.WhenAll(taskList);

        LoadValue.Value = 0.5f;
    }

    /// <summary>
    /// Start 처리
    /// </summary>
    /// <returns></returns>
    private async UniTask LoadStartDataAsync(System.Threading.CancellationToken cancelToken)
    {
        var taskList = new List<UniTask>(startDataList.Count);

        foreach (var x in startDataList)
            taskList.Add(x.DataStartAsync(cancelToken));

        await UniTask.WhenAll(taskList);

        LoadValue.Value = 1f;
    }
}
