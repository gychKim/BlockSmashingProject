using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneManagerEX : Singleton<SceneManagerEX>
{
    // 현재 Scene 이름
    public string CurrentSceneName { get; private set; }
    private string targetSceneName;
    
    private UniTaskCompletionSource initComplete; // Scene 초기화 완료 플래그

    private ISceneInitializer targetSceneInit;
    /// <summary>
    /// 씬 로딩 시작
    /// </summary>
    /// <param name="sceneName"></param>
    ///
    private void Start()
    {
        initComplete = new UniTaskCompletionSource(); // Scene 초기화 완료 플래그 생성
    }

    public void LoadScene(string sceneName)
    {
        CurrentSceneName = SceneManager.GetActiveScene().name; // 현재Scene의 이름 등록 > 최초 실행하는 경우를 대비해서 작성
        targetSceneName = sceneName; // targetScene이름 등록
        initComplete = new UniTaskCompletionSource(); // Scene 초기화 완료 플래그 생성

        LoadTargetSceneAsync().Forget(); // SceneLoading시작
    }

    /// <summary>
    /// SceneLoading 작업
    /// </summary>
    /// <returns></returns>
    public async UniTask LoadTargetSceneAsync()
    {
        await SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Additive); // LoadingScene 호출

        var loadingManager = GameObject.FindFirstObjectByType<LoadingManager>(); // LoadingScene의 LoadingManager를 가져온다.

        await loadingManager.PlayOpenAnimation(); // LoadingUI 시작 애니메이션 대기 => 애니메이션이 종료되면 빠져나온다.

        await SceneManager.UnloadSceneAsync(CurrentSceneName); // 현재Scene 즉, target의 이전 Scene을 Unload시킨다.

        await SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive); // targetScene 호출

        await initComplete.Task; // targetScene의 초기화 작업 대기

        await loadingManager.PlayCloseAnimation(); // LoadingUI Close 애니메이션 대기 => 애니메이션이 종료되면 빠져나온다.

        await SceneManager.UnloadSceneAsync("LoadingScene"); // LoadingScene Unload시킨다.

        CurrentSceneName = SceneManager.GetActiveScene().name; // 현재Scene(targetScene)의 이름 등록 > 이후 외부에서 CurrentSceneName을 필요로 할 수 있기에 작성

        initComplete = null; // Scene 초기화 완료 플래그 처리

        if (targetSceneInit != null)
            targetSceneInit.LoadComplete();

        targetSceneInit = null;
    }

    /// <summary>
    /// Scene 초기화 완료 > targetScene에서 호출해야 한다.
    /// </summary>
    public void InitScene(ISceneInitializer sceneInit = null)
    {
        if(initComplete == null)
        {
            DebugX.LogError($"{this.name}의 initComplete가 없습니다.");
            return;
        }

        targetSceneInit = sceneInit;
        initComplete.TrySetResult();
    }
}
