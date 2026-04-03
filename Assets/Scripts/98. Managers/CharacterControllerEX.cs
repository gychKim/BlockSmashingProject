using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;

public class CharacterControllerEX : MonoBehaviour
{
    /// <summary>
    /// 캐릭터 시작 위치
    /// </summary>
    [SerializeField]
    private Transform characterStartPos;

    /// <summary>
    /// 캐릭터 대기 위치
    /// </summary>
    [SerializeField]
    private Transform characterWaitingPos;

    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Animator animator;

    private string characterName;
    private int characterIndex;

    private void Awake()
    {
        // 현재 캐릭터 ID와, 그 데이터를 가져온 후 적용
        //var characterData = DBManager.Instance.GetCharacterData(GameManager.Instance.CurrentCharacterID);

        //characterName = characterData.characterName;

        //var data = await SaveManager.Instance.CharacterSaveManager.GetReadOnly(GameManager.Instance.CurrentCharacterID);
        //characterIndex = GameManager.Instance.CurrentCharacterID + data.currentSkinIndex;

        //animator.runtimeAnimatorController = characterData.animatorController;

        PlayCharacterAnim("Idle"); // 최초상태는 Idle 상태
    }

    /// <summary>
    /// 스테이지 초기화 시
    /// </summary>
    public void SetStage(Transform startPos)
    {
        // Idle 유지
        PlayCharacterAnim("Idle");
    }

    /// <summary>
    /// 게임 재시작 시
    /// </summary>
    public void GameReStart()
    {
        transform.position = characterStartPos.transform.position;
        PlayCharacterAnim("Idle");
    }

    /// <summary>
    /// 게임 시작 전 연출 시작 시
    /// </summary>
    public async void StartPrevDirection()
    {
        PlayCharacterAnim("Move");

        await transform.DOMove(characterWaitingPos.position, 2f).SetEase(Ease.Linear);

        EventManager.Instance.Publish(GameSceneEventType.EndPrevDirection);
    }

    /// <summary>
    /// 시작 연출 종료 시
    /// </summary>
    public void EndPrevDirection()
    {
        PlayCharacterAnim("Idle");
    }

    /// <summary>
    /// 게임 끝난 후
    /// </summary>
    public void GameEnd()
    {
        // Breaking의 끝 부분에는 Event가 달려있어야 한다.
        // 그 이벤트를 받은 후, GameSceneEventType.BreakingBlock을 publish
    }

    /// <summary>
    /// 게임이 끝난 후 연출 시작 시
    /// </summary>
    public void StartAfterDirection()
    {
        PlayCharacterAnim("Breaking");
    }

    /// <summary>
    /// Breaking 애니메이션의 이벤트
    /// </summary>
    public void StartBreaking()
    {
        EventManager.Instance.Publish(GameSceneEventType.BreakingBlock);
    }

    /// <summary>
    /// 블록 파괴 연출 시작 시
    /// </summary>
    public void BreakingBlock()
    {
        
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
    /// animName의 애니메이션을 실행한다.
    /// </summary>
    /// <param name="animName"></param>
    public void PlayCharacterAnim(string animName)
    {
        //animator.Play($"{characterName}_{characterIndex}_{animName}");
        animator.Play(animName);
    }

    private void OnDestroy()
    {
        
    }
}
