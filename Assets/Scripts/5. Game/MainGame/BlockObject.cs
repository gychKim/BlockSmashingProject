using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class BlockObject : MonoBehaviour
{
    [SerializeField]
    private Image backgroundImage; // 블록 뒷배경 렌더러
    [SerializeField]
    private Image blockImage; // 블록 이미지 렌더러

    private BlockGameData blockGameData; // 스크립터블

    public int index; // 디버그용

    /// <summary>
    /// 블록 데이터 초기화
    /// </summary>
    /// <param name="blockGameData"></param>
    public void Init(BlockGameData blockGameData)
    {
        this.blockGameData = blockGameData;
        blockImage.sprite = blockGameData.sprite;
        backgroundImage.color = blockGameData.backgroundColor;
        gameObject.name = "Block_" + blockGameData.name;
    }

    /// <summary>
    /// 위치 변환 > Flash
    /// </summary>
    /// <param name="trans"></param>
    public void SetPoisiton(Transform target)
    {
        transform.position = target.position;
    }

    public void SetParents(Transform target, bool worldPositionStays = false)
    {
        transform.SetParent(target, worldPositionStays);
    }

    /// <summary>
    /// 블록 사용
    /// </summary>
    public void Use()
    {
        blockGameData.Use();

        EventManager.Instance.Publish(QuestEventType.BlockDestory, blockGameData.blockType); // 퀘스트 갱신
    }

    /// <summary>
    /// 다음 칸으로 이동
    /// </summary>
    public void NextMove(Transform target)
    {
        if (index >= 0)
            index--;

        // DOTween으로 부드럽게 이동
        transform.DOMove(target.position, 0.1f)
            .SetEase(Ease.Linear);

        transform.DOScale(target.localScale, 0.1f)
            .SetEase(Ease.Linear);
    }
    /// <summary>
    /// 다음 칸으로 이동
    /// </summary>
    public void NextMove(Vector3 pos)
    {
        if (index > 0)
            index--;

        // DOTween으로 부드럽게 이동
        transform.DOMove(pos, 0.2f)
            .SetEase(Ease.Linear);
    }

    /// <summary>
    /// 데이터 초기화
    /// </summary>
    public void Clear()
    {
        blockGameData = null;
        blockImage.sprite = null;
        backgroundImage.color = Color.white;
    }

    public string GetSpriteName() => blockImage.sprite.name; // 디버그용


    public void SetIndex(int idx) => index = idx; // 디버그용
}
