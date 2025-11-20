# 🎮 Unity 상호작용 시스템 (Interaction System)

인터페이스 기반의 확장 가능한 상호작용 시스템입니다.

## 📁 파일 구조

```
Interaction/
├── IInteractable.cs           # 상호작용 인터페이스
├── InteractableBase.cs        # 공통 기능 베이스 클래스
├── PlayerDetector.cs          # 플레이어 감지 시스템
├── NPCInteractable.cs         # NPC 상호작용 구현
└── DoorInteractable.cs        # 문 상호작용 구현
```

## 🏗️ 아키텍처

```
IInteractable (인터페이스)
    ↓
InteractableBase (추상 클래스)
    ↓
    ├── NPCInteractable
    ├── DoorInteractable
    └── [커스텀 Interactable...]
```

## ✨ 주요 기능

### 1. **IInteractable 인터페이스**
모든 상호작용 가능한 오브젝트가 구현해야 하는 계약입니다.

```csharp
public interface IInteractable
{
    KeyCode InteractionKey { get; }
    bool CanInteract { get; }
    void Interact();
    void OnPlayerEnterRange(Transform player);
    void OnPlayerExitRange(Transform player);
    string GetInteractionHintText();
}
```

### 2. **InteractableBase 추상 클래스**
공통 기능을 제공하는 베이스 클래스입니다.

**제공 기능:**
- ✅ F키 입력 감지
- ✅ 오디오 재생 시스템
- ✅ UI 힌트 표시/숨김
- ✅ 플레이어 범위 관리
- ✅ 디버그 로깅

### 3. **PlayerDetector 컴포넌트**
플레이어 감지를 담당하는 독립적인 컴포넌트입니다.

**감지 방식:**
- 🎯 **Distance** (거리 기반): 일정 거리 내 플레이어 감지
- 🎯 **Trigger** (물리 기반): 콜라이더 트리거로 감지
- 🎯 **Both** (둘 다): 두 방식 모두 사용

## 🚀 사용 방법

### 📌 방법 1: NPC 상호작용 추가하기

1. **빈 GameObject에 컴포넌트 추가:**
   ```
   GameObject
   ├── NPCInteractable
   └── PlayerDetector
   ```

2. **NPCInteractable 설정:**
   - `NPC Data`: NPCData ScriptableObject 할당
   - `Interaction Key`: F (기본값)
   - `Interaction Sound`: 상호작용 효과음 (선택)
   - `Auto Close Distance`: UI 자동 닫힘 거리 (기본 5m)

3. **PlayerDetector 설정:**
   - `Detection Mode`: Distance (거리 기반 추천)
   - `Detection Range`: 3.0 (감지 범위)

### 📌 방법 2: 문 상호작용 추가하기

1. **문 GameObject에 컴포넌트 추가:**
   ```
   DoorObject
   ├── DoorInteractable
   └── PlayerDetector
   ```

2. **DoorInteractable 설정:**
   - `Open Angle`: 90 (열릴 각도)
   - `Rotation Speed`: 3 (회전 속도)
   - `Auto Close`: true (자동 닫힘 여부)
   - `Auto Close Delay`: 3 (자동 닫힘 시간)
   - `Open Sound`: 문 열리는 소리
   - `Close Sound`: 문 닫히는 소리

3. **PlayerDetector 설정:**
   - `Detection Mode`: Trigger (트리거 기반 추천)
   - `Auto Create Trigger`: true
   - `Trigger Size`: (3, 2, 3)

### 📌 방법 3: 커스텀 상호작용 만들기

새로운 상호작용을 만들려면 `InteractableBase`를 상속받으세요:

```csharp
using UnityEngine;
using UnityAI.Interaction;

public class CustomInteractable : InteractableBase
{
    [Header("커스텀 설정")]
    [SerializeField] private bool isActive = true;
    
    // 상호작용 가능 여부
    public override bool CanInteract => isActive && playerInRange;
    
    // 상호작용 실행
    public override void Interact()
    {
        // F키를 눌렀을 때 실행될 로직
        Debug.Log("커스텀 상호작용 실행!");
        PlayInteractionSound();
        
        // 여기에 원하는 동작 구현
        // 예: 아이템 획득, 퍼즐 해결, 스위치 토글 등
    }
    
    // 플레이어 진입 시 추가 로직 (선택)
    protected override void OnPlayerEnter(Transform player)
    {
        base.OnPlayerEnter(player);
        Debug.Log("플레이어가 가까이 왔습니다!");
    }
    
    // 힌트 텍스트 커스터마이징 (선택)
    public override string GetInteractionHintText()
    {
        return "F키를 눌러 상호작용하기";
    }
}
```

## 🎯 PlayerDetector 선택 가이드

| 상황 | 추천 모드 | 이유 |
|------|----------|------|
| **NPC, 아이템** | Distance | 정확한 거리 제어, 부드러운 감지 |
| **문, 스위치** | Trigger | 명확한 범위, 물리적 경계 |
| **엘리베이터** | Both | 버튼은 트리거, 전체 공간은 거리 |
| **넓은 범위** | Trigger | 성능 최적화 (Update 불필요) |

## ⚙️ 고급 설정

### Distance Mode 최적화
```csharp
[SerializeField] private float checkInterval = 0.2f; // 0.2초마다 체크
```
- 거리 체크 간격을 조절하여 성능 최적화
- 기본값 0.2초 (충분히 빠름)

### 디버그 시각화
- **PlayerDetector**: Scene 뷰에서 감지 범위 표시
  - Distance: 노란색/초록색 구체
  - Trigger: 파란색/청록색 박스
- **Gizmos 활성화**: PlayerDetector의 `Show Gizmos` 체크

### 오디오 설정
```csharp
[Header("오디오")]
[SerializeField] protected AudioSource audioSource;       // 자동 생성됨
[SerializeField] protected AudioClip interactionSound;    // 효과음
[SerializeField] protected float soundVolume = 0.5f;      // 볼륨
```

## 🔄 기존 코드 마이그레이션

### NPCInteraction → NPCInteractable
```
1. NPCInteraction 컴포넌트 제거
2. NPCInteractable 컴포넌트 추가
3. PlayerDetector 컴포넌트 추가 (Detection Mode: Distance)
4. 설정값 동일하게 복사
```

### AutoRotatingDoor → DoorInteractable
```
1. AutoRotatingDoor 컴포넌트 제거
2. DoorInteractable 컴포넌트 추가
3. PlayerDetector 컴포넌트 추가 (Detection Mode: Trigger)
4. 설정값 동일하게 복사
```

## 🎨 확장 아이디어

이 시스템으로 구현 가능한 것들:

- ✅ **아이템 획득**: 아이템을 F키로 줍기
- ✅ **스위치**: 불을 켜고 끄기
- ✅ **퍼즐**: 상호작용 기반 퍼즐
- ✅ **상점**: NPC 상점 열기
- ✅ **탈것**: 차량/말 타기/내리기
- ✅ **크래프팅**: 제작대 사용
- ✅ **텔레포트**: 포탈 사용

## 📝 인터페이스 분리 원칙 (SOLID)

이 시스템은 SOLID 원칙을 따릅니다:
- **S** (Single Responsibility): 각 클래스가 하나의 책임만 가짐
- **O** (Open/Closed): 확장에는 열려있고 수정에는 닫혀있음
- **L** (Liskov Substitution): 파생 클래스는 기본 클래스를 대체 가능
- **I** (Interface Segregation): 작고 구체적인 인터페이스
- **D** (Dependency Inversion): 추상화에 의존

## 🐛 문제 해결

### 문제: "플레이어를 찾을 수 없습니다"
**해결책:**
1. 플레이어 GameObject에 "Player" 태그 추가
2. 또는 PlayerMovement 컴포넌트 추가

### 문제: "UI Manager가 없습니다"
**해결책:**
1. Scene에 NPCUIManager 추가
2. 또는 수동으로 UI Manager 할당

### 문제: 트리거가 작동하지 않음
**해결책:**
1. PlayerDetector의 Detection Mode 확인
2. 플레이어에 Rigidbody 또는 CharacterController 있는지 확인
3. 플레이어 Collider가 Trigger가 아닌지 확인

## 📚 참고 사항

- **네임스페이스**: `UnityAI.Interaction`
- **최소 Unity 버전**: Unity 2021.3+
- **의존성**: NPCUIManager (UI 표시용, 선택사항)

## 🎓 학습 자료

Unity의 인터페이스 패턴에 대해 더 알아보기:
- [Unity C# 인터페이스](https://docs.unity3d.com/Manual/script-Interfaces.html)
- [SOLID 원칙](https://unity.com/how-to/solid-principles-single-responsibility-principle)

---

**Made with ❤️ by UnityAI Team**

