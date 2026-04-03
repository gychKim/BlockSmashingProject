using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public abstract class BaseView : MonoBehaviour, IUIView
{
    public GameObject RootObject => gameObject;
    public CompositeDisposable Disposables { get; } = new CompositeDisposable();

    private void OnDestroy()
    {
        Disposables.Dispose();
    }
}
