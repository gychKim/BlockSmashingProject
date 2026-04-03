#pragma warning disable UDR0001 // UDR0001 에러 무시 > static 필드 RuntimeInitializeOnLoadMethod으로 초기화 하라는 것 같은데, 처리하면 빌드 시 IL2CPP에서 이를 처리하지 않아 버그 발생해서 우선 에러 메시지만 비활성화

using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;
    private static bool isQuit;
    protected static bool IsDontDestroyOnLoad = true;
    public static T Instance
    {
        get
        {
            if (_instance == null && !isQuit)
            {
                _instance = FindAnyObjectByType<T>();
                if (_instance == null)
                {
                    DebugX.Log($"싱글톤이 존재하지 않습니다.");
                    //GameObject singletonObject = new GameObject(typeof(T).Name);
                    //_instance = singletonObject.AddComponent<T>();
                    //if(IsDontDestroyOnLoad)
                    //    DontDestroyOnLoad(singletonObject);
                }
                else
                {
                    if (IsDontDestroyOnLoad)
                        DontDestroyOnLoad(_instance.gameObject);
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            if(IsDontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    private void OnApplicationQuit()
    {
        isQuit = true;
    }
}
