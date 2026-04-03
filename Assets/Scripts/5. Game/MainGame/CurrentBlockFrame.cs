using UnityEngine;
using UnityEngine.UI;

public class CurrentBlockFrame : MonoBehaviour
{
    //[SerializeField]
    //private GameObject shieldEffect; // 실드 이펙트

    [SerializeField]
    private Transform currentBlockPlaceTrans; // 현재 블록 배치될 위치

    private BlockObject currentBlock; // 현재 블록

    private MainGameController controller; // 메인 게임 모델

    /// <summary>
    /// 초기화
    /// </summary>
    /// <param name="model"></param>
    public void Init(MainGameController controller)
    {
        this.controller = controller;
    }

    /// <summary>
    /// 블럭배치
    /// </summary>
    /// <param name="blockObj"></param>
    public void PlaceBlock(BlockObject blockObj)
    {
        currentBlock = blockObj;

        //currentBlock.SetParents(transform);

        currentBlock.NextMove(currentBlockPlaceTrans);

        //currentBlock.transform.SetParent(currentBlockPlaceTrans, false);
        //LayoutRebuilder.ForceRebuildLayoutImmediate(currentBlockPlaceTrans as RectTransform);
    }

    ///// <summary>
    ///// 실드 적용
    ///// </summary>
    //public void Shield()
    //{
    //    shieldEffect.SetActive(true);
    //}

    ///// <summary>
    ///// 실드 제거
    ///// </summary>
    //public void RemoveShield()
    //{
    //    shieldEffect.SetActive(false);
    //}

    /// <summary>
    /// 현재 배치된 블록 사용
    /// </summary>
    public void UseBlock()
    {
        currentBlock.Use();
    }

    /// <summary>
    /// 현재 배치된 블록 제거 > Pool로 Return
    /// </summary>
    public void ReturnBlock()
    {
        if (currentBlock != null)
        {
            currentBlock.Clear();
            PoolManager.Instance.Return(PoolObjectType.BlockObject, currentBlock);
        }
        //LayoutRebuilder.ForceRebuildLayoutImmediate(currentBlockPlaceTrans as RectTransform); // 필수

        currentBlock = null;
    }
}
