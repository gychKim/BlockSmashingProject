# BlockSmashing

> 좌우에 배치된 블록을 제한시간 내에 부수어 목표 점수를 달성하는 하이퍼캐주얼 모바일 게임

<!-- 플레이 GIF -->

<!-- 플레이 영상 링크 -->

---

## 📋 개요

| 항목 | 내용 |
|------|------|
| 개발 기간 | 2025.03 ~ 2025.09 (7개월) |
| 개발 인원 | 1인 |
| 플랫폼 | Android |
| 엔진 | Unity 6000.0.33f1 |

---

## 🛠 기술 스택

`Unity` `C#` `UniRx` `UniTask` `ScriptableObject` `MVP Pattern`

---

## 📐 아키텍처

**MVP + Binder 패턴**으로 UI와 비즈니스 로직을 분리했습니다.

```
Scene
└── Init (SceneInit)
    ├── RootBinder         ← Model/Presenter/View 생성 및 연결
    ├── RootModel          ← 데이터 상태 관리
    ├── RootPresenter      ← 비즈니스 로직
    └── RootView           ← UI 표현
```

씬 구성: `Title → Lobby → Game (GamePrev → MainGame → GameAfter)`

---

## ⚙️ 주요 구현

### 1. 타입 안전 EventManager

씬 간 결합도를 낮추기 위해 Generic Enum 기반의 EventManager를 직접 구현했습니다.  
각 이벤트는 `Guid`로 관리되어 구독 해제 시 정확한 대상만 제거됩니다.  
초기에는 이벤트 타입별로 Dictionary를 분리 관리했으나, 씬이 늘어날수록 관리 비용이 커져 `Subscribe<TEnum>` 형태의 Generic 구조로 리팩토링했습니다.

```csharp
// 구독
startGameKey = EventManager.Instance.Subscribe(MainGameEventType.GameStart, StartGame);

// 발행
EventManager.Instance.Publish(MainGameEventType.GameEnd);

// 해제 (OnDestroy)
EventManager.Instance.Unsubscribe(MainGameEventType.GameStart, startGameKey);
```

---

### 2. Mediator + Modifier 패턴 (아이템 효과 시스템)

아이템 효과(점수 배율, 피버, 방어막 등)를 `Modifier`로 표현하고, `Mediator`가 체인 형태로 관리합니다.  
효과가 중첩되거나 시간이 만료되어도 안전하게 제거됩니다.

```csharp
// 점수 배율 아이템 적용
itemController.AddScoreMultiplier(0.2f, duration: -1f); // 콤보 10회마다 +0.2x

// Mediator가 LinkedList로 Modifier 체인 관리
public void AddModifier(Modifier<T> modifier)
{
    modifiers.AddLast(modifier);
    Queries += modifier.Handle;
    modifier.OnDispose += (_) => { Queries -= modifier.Handle; };
}
```

---

### 3. AES + HMAC 세이브 시스템

세이브 파일 위변조를 방지하기 위해 AES-CBC 암호화와 HMAC 무결성 검증을 적용했습니다.  
`ISaveManager` 인터페이스로 Block / Character / Quest / Item / GameState 저장 책임을 분리했습니다.  
Dirty Flag 패턴으로 변경 사항이 없을 때는 저장을 생략합니다.

```csharp
// 저장 시: 변경된 데이터만 저장 (Dirty Flag)
public void Save(JsonData jsonData, bool isForce = false)
{
    if (!isDirty && !isForce) return;
    jsonData.blockList = blockSaveDataList;
    isDirty = false;
}

// 로드 시: 데이터가 준비될 때까지 비동기 대기
private async UniTask<BlockSaveData> Get(int blockID)
{
    while (!isLoadData) await UniTask.Yield();
    ...
}
```

---

### 4. 오브젝트 풀링

Generic + Interface 기반으로 타입에 관계없이 재사용 가능한 풀 시스템을 구현했습니다.

```csharp
public static class ClassPoolSystem<T> where T : class, IPoolable, new()
{
    public static readonly ObjectPool<T> dict = new ObjectPool<T>(
        createFunc:      () => new T(),
        actionOnGet:     poolable => poolable.Get(),
        actionOnRelease: poolable => poolable.Release(),
        actionOnDestroy: poolable => poolable.Destroy()
    );
}
```

---

### 5. 콤보 시스템

`UniTask` 기반 비동기 타이머로 콤보 지속시간을 관리합니다.  
10콤보마다 점수 배율이 0.2x씩 증가하며, 콤보가 끊기면 배율이 초기화됩니다.

```csharp
CurrentCombo.Subscribe(value =>
{
    if (value <= 0)
    {
        itemController.RemoveAllModifier(EffectType.ScoreMultiplier);
        return;
    }
    if (value % 10 == 0)
        itemController.AddScoreMultiplier(0.2f, -1f);
}).AddTo(gameObject);
```

---

## 🔧 트러블슈팅

### 1. 캐릭터 변경 시 이미지 미갱신 버그
- 문제: 캐릭터를 변경해도 이미지를 업데이트하는 이벤트가 데이터 변경 이전에 호출되는 순서 문제로 캐릭터 이미지가 갱신되지 않는 버그가 발생했습니다.
- 해결: 캐릭터 이미지 처리를 여러 곳에서 분산 처리하던 것을 한 곳에서만 담당하도록 변경하여 호출 순서에 의존하지 않는 구조로 수정했습니다.
---

## 💬 회고
- UniRx를 이용하여 반응형 프로그래밍에 대해 알게 되었습니다.
- UniTask를 이용하여 비동기 작업에 대해 더 잘 알게 되었습니다.
- MVP 패턴을 적용하여 View와 비즈니스 로직의 분리가 코드의 독립성과 가독성에 얼마나 중요한지 알게 되었습니다.
- 현재 구현한 MVP 프레임워크에 아쉬움이 있습니다. Model이 외부 시스템을 필요로 할 때 해당 시스템을 주입받을 수 없는 구조여서, 다음 프로젝트에서는 DI 컨테이너 도입을 고려하게 된 계기가 되었습니다.
