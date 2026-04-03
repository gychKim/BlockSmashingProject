using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneInit : MonoBehaviour, ISceneInitializer
{
    [SerializeField]
    private List<GameObject> dataManagerList;

    private List<IDataAwake> awakeDataList = new();
    private List<IDataStart> startDataList = new();

    private void Awake()
    {
        InitAsync().Forget();
    }

    public async UniTask InitAsync()
    {
        var cancelToken = this.GetCancellationTokenOnDestroy(); // 현 객체 파괴시 취소 토큰

        try
        {
            foreach(var obj in dataManagerList)
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

            SceneManagerEX.Instance.InitScene(this); // Scene 초기화 완료
        }
        catch(OperationCanceledException)
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
        await UniTask.WhenAll(awakeDataList.Select(awake => awake.DataAwakeAsync(cancelToken)));
    }

    /// <summary>
    /// Start 처리
    /// </summary>
    /// <returns></returns>
    private async UniTask LoadStartDataAsync(System.Threading.CancellationToken cancelToken)
    {
        await UniTask.WhenAll(awakeDataList.Select(awake => awake.DataAwakeAsync(cancelToken)));
    }

    /// <summary>
    /// Scene전환 완료 시 호출
    /// </summary>
    public void LoadComplete()
    {
        EventManager.Instance.Publish(GameSceneEventType.StartScene); // 최초 연출 시작
    }
}
