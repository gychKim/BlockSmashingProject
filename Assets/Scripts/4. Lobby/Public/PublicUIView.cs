using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PublicUIView : MonoBehaviour, IUIView
{
    public TextMeshProUGUI CurrentGoldText => currentGoldText; // 현재 골드
    [SerializeField]
    private TextMeshProUGUI currentGoldText;

    public TextMeshProUGUI CurrentDiaText => currentDiaText; // 현재 다이아
    [SerializeField]
    private TextMeshProUGUI currentDiaText;

    public GameObject RootObject => gameObject;

    public CompositeDisposable Disposables { get; } = new CompositeDisposable();
    private void OnDestroy()
    {
        Disposables.Dispose(); // 해제
    }
}
