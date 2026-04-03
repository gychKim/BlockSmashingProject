using System;
using System.Collections.Generic;
using UnityEngine;

public class BlockContainer : MonoBehaviour
{
    [SerializeField]
    private List<Transform> placeList; // 블록 배치 포인트 리스트

    public Queue<BlockObject> BlockQueue => blockQueue;
    private Queue<BlockObject> blockQueue = new Queue<BlockObject>(); // 블록 큐

    [SerializeField]
    private CurrentBlockFrame blockFrame; // 블록 프레임

    //private Guid leftClickEventKey, rightClickEventKey;
    private void Awake()
    {
        //leftClickEventKey = EventManager.Instance.Subscribe(MainGameEventType.LeftClick);
        //rightClickEventKey = EventManager.Instance.Subscribe(MainGameEventType.RightClick);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(MainGameController controller)
    {
        blockFrame.Init(controller);
    }

    public void UseBlock()
    {
        blockFrame.UseBlock();
    }

    /// <summary>
    /// 블록 등록
    /// </summary>
    /// <param name="block"></param>
    public void SetBlock(BlockObject newBlock)
    {
        newBlock.SetIndex(blockQueue.Count);
        blockQueue.Enqueue(newBlock);

        newBlock.SetParents(transform);
        newBlock.SetPoisiton(placeList[newBlock.index]);
    }

    public void MoveBlock()
    {
        foreach (var block in blockQueue)
        {
            var targetPos = placeList[block.index];
            block.NextMove(targetPos);
        }
    }

    /// <summary>
    /// 가장 앞에 있는 블록을 현재 블록으로 전달
    /// </summary>
    public void ReplaceBlock()
    {
        blockFrame.PlaceBlock(blockQueue.Dequeue());
    }

    /// <summary>
    /// 블록 정렬
    /// </summary>
    public void AlignBlock()
    {
        int i = 0;
        foreach (var block in blockQueue)
        {
            block.NextMove(placeList[i++]);
        }
    }

    /// <summary>
    /// Frame의 블록을 Pool에 넣는다.
    /// </summary>
    public void ReturnCurrentBlock()
    {
        blockFrame.ReturnBlock();
    }

    public void ReturnAllBlock()
    {
        foreach (var block in blockQueue)
        {
            if (block != null)
            {
                block.Clear();
                PoolManager.Instance.Return(PoolObjectType.BlockObject, block);
            }
        }
    }

    public void Clear()
    {
        blockQueue.Clear();
    }

    private void OnDestroy()
    {
        //EventManager.Instance.Unsubscribe(MainGameEventType.LeftClick, leftClickEventKey);
        //EventManager.Instance.Unsubscribe(MainGameEventType.RightClick, rightClickEventKey);
    }
}
