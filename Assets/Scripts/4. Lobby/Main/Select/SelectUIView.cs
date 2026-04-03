using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class SelectUIView : MonoBehaviour, IUIView
{
    #region UI

    public Button CharacterSelectButton => characterSelectButton; // 캐릭터 선택 버튼
    [SerializeField]
    private Button characterSelectButton; // 캐릭터 선택 버튼

    public Animator Animator => animator;
    [SerializeField]
    private Animator animator;

    public Button CharacterSkinSelectButton => characterSkinSelectButton; // 캐릭터 스킨 선택 버튼
    [SerializeField]
    private Button characterSkinSelectButton; // 캐릭터 스킨 선택 버튼

    public Button BlockSelectButton => blockSelectButton; // 대리석(블록) 스킨 선택 버튼
    [SerializeField]
    private Button blockSelectButton; // 대리석(블록) 스킨 선택 버튼

    public GameObject CharacterSelectObject => characterSelectObject; // 캐릭터 선택 UI
    [SerializeField]
    private GameObject characterSelectObject; // 캐릭터 선택 UI

    public GameObject CharacterSkinSelectObject => characterSkinSelectObject; // 캐릭터 스킨 선택 UI
    [SerializeField]
    private GameObject characterSkinSelectObject; // 캐릭터 스킨 선택 UI

    public GameObject BlockSelectObject => blockSelectObject;
    [SerializeField]
    private GameObject blockSelectObject; // 블록 스킨 선택 UI

    #endregion

    public GameObject RootObject => gameObject; // SelectUI
    public CompositeDisposable Disposables { get; } = new CompositeDisposable();

    private void OnDestroy()
    {
        Disposables.Dispose(); // 해제
    }
}
