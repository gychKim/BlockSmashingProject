using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
public class LoadingManager : MonoBehaviour
{
    [SerializeField]
    private RectTransform loadingUI;

    [SerializeField]
    private List<DOTweenAnimation> textAnimation;

    private Vector2 startPosition = new Vector2(1080, 0);
    private Vector2 centerPosition = Vector2.zero;
    private Vector2 exitPosition = new Vector2(-1080, 0);

    private void Start()
    {
        loadingUI.anchoredPosition = startPosition;
    }

    public async UniTask PlayOpenAnimation()
    {
        await loadingUI.DOAnchorPos(centerPosition, 0.4f)
            .SetEase(Ease.OutBack)
            .ToUniTask();

        textAnimation[0].DORestart();
    }

    public async UniTask PlayCloseAnimation()
    {
        await loadingUI.DOAnchorPos(exitPosition, 0.4f)
            .SetEase(Ease.InBack)
            .ToUniTask();

        textAnimation.ForEach(x => x.DORewind());
    }

    private void OnDestroy()
    {
        
    }
}
