using UnityEngine;

public abstract class BaseBinder<TView, TPresenter, TModel> : MonoBehaviour
    where TView : IUIView
    where TModel : IModel
    where TPresenter : IPresenter<TView, TModel> // 제네릭 클래스가 새 인스턴스 형식을 만들때 new 사용가능
{
    [LabelColor("Binder 데이터", 135, 206, 235)]
    [SerializeField] TView view; // Inspector에서 주입

    protected TPresenter presenter;
    protected TModel model;

    /// <summary>
    /// 모델 생성, Presenter 생성, 이벤트 등록, 등
    /// </summary>
    protected virtual void Awake()
    {
        model = CreateModel(); // Model 생성
        presenter = CreatePresenter(view, model);
    }

    /// <summary>
    /// 데이터 Bind, 이벤트 호출 가능
    /// </summary>
    protected virtual void Start()
    {
        model.Start();
        presenter.Bind();
    }

    protected abstract TModel CreateModel(); // Model 생성
    protected abstract TPresenter CreatePresenter(TView view, TModel model); // Presenter 생성

    private void OnDestroy()
    {
        if (presenter != null)
        {
            presenter.Destroy();
        }

        if(model != null)
        {
            model.Destroy();
        }
    }
}
