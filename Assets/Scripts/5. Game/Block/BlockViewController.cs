using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class BlockViewController : MonoBehaviour
{
    /// <summary>
    /// 현재 파괴되는 블록 위치
    /// </summary>
    [SerializeField]
    private Transform breakingPoint;

    /// <summary>
    /// 블록 리스트
    /// </summary>
    [SerializeField]
    private List<BlockView> blockList;

    /// <summary>
    /// 게임 재시작 시
    /// </summary>
    public void GameReStart()
    {
        foreach (var block in blockList)
        {
            block.PlayIdleAnim();
        }

        breakingPoint.position = blockList[0].transform.position + Vector3.up * 5f;
    }

    /// <summary>
    /// 시작 연출 실행 시
    /// </summary>
    public void StartPrevDirection()
    {
        GameReStart();
        // Background 변경
    }

    /// <summary>
    /// 시작 연출 종료 시
    /// </summary>
    public void EndPrevDirection()
    {
        
    }

    public void GameEnd()
    {

    }

    public void StartAfterDirection()
    {
        
    }


    /// <summary>
    /// 블록 파괴 연출 시작 시
    /// </summary>
    public void BreakingBlock()
    {
        BreakingBlockAsync(); // 블록 파괴 연출 종료 대기
    }

    /// <summary>
    /// 마지막 블록 파괴 시
    /// </summary>
    public void EndBreakingBlock()
    {

    }

    /// <summary>
    /// 게임이 끝난 후 연출 종료 시
    /// </summary>
    public void EndAfterDirection()
    {
        
    }


    /// <summary>
    /// 블록 파괴 연출 로직
    /// </summary>
    /// <returns></returns>
    private void BreakingBlockAsync()
    {
        Sequence seq = DOTween.Sequence();
        float totalTime = 2.5f;

        int totalBlockCount = blockList.Count;
        int destoryedBlcokCount = (int)(totalBlockCount * (MainGameManager.Instance.CurrentScore / MainGameManager.Instance.CurrentGoalScore));

        float interval = totalTime / destoryedBlcokCount;
        for (int i = 0; i < destoryedBlcokCount; i++)
        {
            float delay = interval * i;

            int index = i; // 클로저 캡처 방지
            seq.Insert(delay, DOVirtual.DelayedCall(0f, () =>
            {
                blockList[index].PlayBreakingAnim();
                breakingPoint.position = blockList[index].transform.position + Vector3.up * 5f;

                if (index == destoryedBlcokCount - 1)
                {
                    EventManager.Instance.Publish(GameSceneEventType.EndBreakingBlock);
                }
            }));
        }

        seq.OnComplete(() =>
        {
            EventManager.Instance.Publish(GameSceneEventType.EndAfterDirection);
        });

        seq.Play();
    }
}
