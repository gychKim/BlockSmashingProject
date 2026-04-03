using UniRx;
using UnityEngine;

public class OutGameView : MonoBehaviour, IUIView
{

    public GameObject GamePrevUI => gamePrevUI;
    [SerializeField]
    private GameObject gamePrevUI;

    public GameObject GameAfterUI => gameAfterUI;
    [SerializeField]
    private GameObject gameAfterUI;


    public GameObject RootObject => gameObject;

    public CompositeDisposable Disposables { get; } = new CompositeDisposable();

    private void OnDestroy()
    {
        Disposables.Dispose();
    }
}
