
public enum LobbyEventType
{
    JoinGameScene,

}
/// <summary>
/// 메인게임 이벤트
/// </summary>
public enum MainGameEventType
{
    GamePause = 0,
    GameResum = 1,
    GameStart = 2, // 메인게임 시작 > 스테이지에 맞춰서 시작한다.
    GameExit = 3, // 메인게임종료

    NextStage = 4, // 다음 스테이지
    GameEnd = 5, // 게임 결과

    GetStage = 6,
    GameReStart = 7, // 메인게임 재시작 > 스테이지 1부터 다시 시작

    LeftClick = 8, // 좌측 버튼 클릭
    RightClick = 9, // 우측 버튼 클릭

    HitShield = 10, // 실드가 있는 상태에서 점수 깎였을 시

    SetCurrentStage, // 현재 스테이지
    SetGoalScore, // 목표점수 설정
    SetCurrentTime, // 현재 시간 설정
    SetCurrentScore, // 현재 점수 설정
    SetCombo, // 현재 콤보 설정
}


public enum LoadingEventType
{
    StartLoading,
    EndLoading,
}

/// <summary>
/// 게임Scene(오브젝트) 이벤트
/// </summary>
public enum GameSceneEventType
{
    StartScene = 1, // 스테이지 초기화

    StartPrevDirection = 2, // 게임 시작 전 연출 시작
    EndPrevDirection = 3, // 게임 시작 전 연출 종료

    StartAfterDirection = 4, // 게임 종료 후 연출 시작
    EndAfterDirection = 5, // 게임 종료 후 연출 종료

    BreakingBlock = 6, // 캐릭터가 블록을 부수기 시작했을 시(주먹or발이 블록에 닿였을 시)
    EndBreakingBlock = 7, // 마지막 블록이 파괴되었을 시
}

public enum UIEventType
{
    OpenGameExitUI = 0,

    ApplyCharacter = 1,
    ApplySkin = 2,
    ApplyBlock = 3,

    //SelectCharacter = 4,
    //SelectSkin = 5,
    //SelectBlock = 6,

    //ReturnCharacter = 7,
    //ReturnSkin = 8,
    //ReturnBlock = 9,

    PlayCharacterAnim = 10,

    SwapMainUI = 11,
}

public enum BlockType
{
    PowerUp,
    PowerDown,

    // 이하 아이템 블록
    //PowerMultiplier,
    TimeUp,
    //TimeDown,
    TimeStop,
    //Shield,
    //Swap,
    //Fever,
    //ItemChance,
    End,
}
public enum EffectType
{
    Score,
    //ScoreDown,
    ScoreMultiplier,
    //ScoreMultiplierDown,
    Time,
    TimeStop,
    Shield,
    SwapBlockList,
    Fever,
    ItemSpawnChance, // 아이템 스폰 확률
}

public enum ShopItemType
{
    None = -1,
    Shield = 2001,
    ItemSpawnChance = 2002,
    End
}

public enum PoolObjectType
{
    BlockObject,
    ShopItem,
    GameSelectItem,
    QuestItem
}
public enum PoolSystemType
{
    Query,
}

/// <summary>
/// 연출 타입
/// </summary>
public enum DirectionType
{
    Stage01,
    Stage02,
    Stage03,
    Stage04,
    Stage05,


}

public enum ShopType
{
    Character,
    Skin,
    Item,
}

/// <summary>
/// 퀘스트 타입
/// </summary>
public enum QuestConditionType
{
    ComboReach, // 콤보 도달
    ScoreReach, // 점수 도달
    DestoryBlock,   // 블록 파괴
    UseItem,    // 아이템 사용
    ClearWithoutWeak,   // Weak블록 사용하지 않고 클리어
    ClearWithoutItemBlock,  // 아이템 블록 사용하지 않고 클리어
}

/// <summary>
/// 퀘스트 이벤트 타입
/// </summary>
public enum QuestEventType
{
    ComboChanged,
    ScoreChanged,
    BlockDestory,
    ItemUse,
}

public enum ConditionLogic
{
    AND,
    OR,
}

public enum ProgressDisplayMode
{
    Min,
    Max,
    Sum,
    First
}
