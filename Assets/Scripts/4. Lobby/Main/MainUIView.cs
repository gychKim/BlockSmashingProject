using UniRx;
using UnityEngine;

public class MainUIView : MonoBehaviour, IUIView
{
    public GameObject RootObject => gameObject;

    public CompositeDisposable Disposables { get; } = new CompositeDisposable();
    private void OnDestroy()
    {
        Disposables.Dispose(); // 해제
    }
}
