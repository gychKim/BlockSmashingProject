using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class RootView : MonoBehaviour, IUIView
{
    public GameObject MainUI => mainUI; // MainUI
    [SerializeField]
    private GameObject mainUI; // MainUI

    public GameObject RootObject => gameObject;
    public CompositeDisposable Disposables { get; } = new CompositeDisposable();

    private void Awake()
    {
        
    }

    ///// <summary>
    ///// 메인게임 입장
    ///// </summary>
    //public async void JoinMainGame()
    //{
        
    //}

    //public async void JoinMain()
    //{
        
    //}

    private void OnDestroy()
    {
        Disposables.Dispose(); // 해제
    }
}
