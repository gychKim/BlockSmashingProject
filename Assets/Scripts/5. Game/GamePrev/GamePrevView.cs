using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class GamePrevView : BaseView
{
    public GameObject GamePrevUI => gamePrevUI;
    [SerializeField]
    private GameObject gamePrevUI;
    public Button GameStartButton => gameStartButton;
    [SerializeField]
    private Button gameStartButton;

    /// <summary>
    /// PrevUI를 활성화한다.
    /// </summary>
    public void OpenPrevUI()
    {
        gamePrevUI.SetActive(true);
    }

    /// <summary>
    /// 메인게임 시작 시 > 모든 UI를 닫는다.
    /// </summary>
    public void ClosePrevUI()
    {
        gamePrevUI.SetActive(false);
    }

    private void OnDestroy()
    {
        Disposables.Dispose();
    }
}
