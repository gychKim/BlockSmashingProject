using UnityEngine;

public class BlockView : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    private int blockID;

    private void Awake()
    {
        // 애니메이터가 등록되어 있지 않다면, 가져온다.
        if(animator == null)
            animator = GetComponent<Animator>();

        blockID = GameManager.Instance.CurrentBlockID; // 현재 선택중인 블록의 ID를 가져온다.

        // 블록 ID에 해당하는 블록 데이터를 적용시킨다.
        animator.runtimeAnimatorController = DBManager.Instance.GetBlockData(blockID).animController;

        PlayIdleAnim(); // Idle 애니메이션 실행
    }

    /// <summary>
    /// 스테이지가 초기화 되어 블록을 초기화 한다.
    /// </summary>
    public void InitStage()
    {
        gameObject.SetActive(true);

        PlayIdleAnim();
    }

    /// <summary>
    /// Idle 애니메이션 실행
    /// </summary>
    public void PlayIdleAnim()
    {
        animator.Play($"Block_{blockID}_Idle");
    }

    /// <summary>
    /// Breaking 애니메이션 실행
    /// </summary>
    public void PlayBreakingAnim()
    {
        animator.Play($"Block_{blockID}_Breaking");
    }
}
