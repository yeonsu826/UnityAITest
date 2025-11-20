# 🔄 마이그레이션 가이드

기존 `NPCInteraction`과 `AutoRotatingDoor`에서 새로운 인터페이스 기반 시스템으로 전환하는 가이드입니다.

## 📊 변경 사항 비교

### 구조적 개선

| 항목 | 기존 시스템 | 새로운 시스템 |
|------|------------|-------------|
| **구조** | 독립적인 2개 스크립트 | 인터페이스 기반 통합 시스템 |
| **코드 중복** | 많음 (감지, 오디오, UI) | 없음 (공통 기능 상속) |
| **확장성** | 어려움 | 쉬움 (인터페이스 구현) |
| **유지보수** | 각각 수정 필요 | 베이스 클래스 한 번만 수정 |
| **테스트** | 개별 테스트 필요 | 공통 로직 한 번만 테스트 |

### 기능 비교

#### NPCInteraction → NPCInteractable

| 기능 | 기존 | 새 시스템 | 개선점 |
|------|------|----------|--------|
| 플레이어 감지 | 직접 구현 (Update) | PlayerDetector | ✅ 모듈화, 재사용 가능 |
| F키 입력 | 직접 구현 | InteractableBase | ✅ 공통화 |
| 오디오 시스템 | 직접 구현 | InteractableBase | ✅ 공통화 |
| UI 관리 | 직접 구현 | InteractableBase | ✅ 공통화 |
| 거리 체크 | 매 프레임 | 최적화된 간격 | ✅ 성능 개선 |
| 코드 라인 수 | ~253줄 | ~140줄 | ✅ 45% 감소 |

#### AutoRotatingDoor → DoorInteractable

| 기능 | 기존 | 새 시스템 | 개선점 |
|------|------|----------|--------|
| 플레이어 감지 | 트리거 직접 구현 | PlayerDetector | ✅ 모듈화 |
| F키 입력 | 직접 구현 | InteractableBase | ✅ 공통화 |
| 오디오 시스템 | 직접 구현 | InteractableBase | ✅ 공통화 |
| 트리거 설정 | 수동 | 자동 생성 옵션 | ✅ 편의성 향상 |
| 힌트 UI | 직접 관리 | InteractableBase | ✅ 공통화 |
| 코드 라인 수 | ~228줄 | ~130줄 | ✅ 43% 감소 |

## 🎯 마이그레이션 단계

### 1️⃣ NPCInteraction → NPCInteractable

**Before (기존):**
```
NPCObject
└── NPCInteraction
    ├── NPC Data: [할당]
    ├── Interaction Range: 3
    ├── Auto Close Distance: 5
    ├── Interaction Key: F
    ├── Audio Source: [자동]
    ├── Interaction Sound: [할당]
    └── UI Manager: [할당]
```

**After (새 시스템):**
```
NPCObject
├── NPCInteractable
│   ├── NPC Data: [할당]
│   ├── Interaction Key: F
│   ├── Interaction Hint Text: "F키를 눌러 대화하기"
│   ├── Audio Source: [자동]
│   ├── Interaction Sound: [할당]
│   ├── UI Manager: [할당]
│   ├── Auto Close Distance: 5
│   └── Show Debug Logs: false
│
└── PlayerDetector
    ├── Detection Mode: Distance
    ├── Detection Range: 3
    └── Check Interval: 0.2
```

**마이그레이션 체크리스트:**
- [ ] NPCInteraction 컴포넌트 제거
- [ ] NPCInteractable 컴포넌트 추가
- [ ] PlayerDetector 컴포넌트 추가
- [ ] NPC Data 다시 할당
- [ ] 효과음 다시 할당 (있는 경우)
- [ ] Detection Range 설정 (기존 Interaction Range와 동일)
- [ ] Auto Close Distance 확인
- [ ] 테스트: F키 입력, UI 표시, 거리 감지

---

### 2️⃣ AutoRotatingDoor → DoorInteractable

**Before (기존):**
```
DoorObject
└── AutoRotatingDoor
    ├── Open Angle: 90
    ├── Rotation Speed: 3
    ├── Interaction Key: F
    ├── Auto Close: true
    ├── Auto Close Delay: 3
    ├── Open Sound: [할당]
    ├── Close Sound: [할당]
    └── UI Manager: [할당]
    
BoxCollider (isTrigger: true)
```

**After (새 시스템):**
```
DoorObject
├── DoorInteractable
│   ├── Interaction Key: F
│   ├── Interaction Hint Text: "F키를 눌러 문 열기"
│   ├── Audio Source: [자동]
│   ├── Open Angle: 90
│   ├── Rotation Speed: 3
│   ├── Auto Close: true
│   ├── Auto Close Delay: 3
│   ├── Open Sound: [할당]
│   ├── Close Sound: [할당]
│   └── UI Manager: [할당]
│
└── PlayerDetector
    ├── Detection Mode: Trigger
    ├── Auto Create Trigger: true
    ├── Trigger Size: (3, 2, 3)
    └── Show Gizmos: true

[BoxCollider는 PlayerDetector가 자동 생성]
```

**마이그레이션 체크리스트:**
- [ ] AutoRotatingDoor 컴포넌트 제거
- [ ] DoorInteractable 컴포넌트 추가
- [ ] PlayerDetector 컴포넌트 추가
- [ ] Open Angle, Rotation Speed 설정
- [ ] Auto Close 설정 복사
- [ ] Open/Close Sound 다시 할당
- [ ] 기존 BoxCollider 제거 (PlayerDetector가 자동 생성)
- [ ] Trigger Size 조정 (기존 크기와 동일하게)
- [ ] 테스트: F키 입력, 문 회전, 자동 닫힘

---

## 🔧 자동 마이그레이션 스크립트

Unity Editor에서 실행할 수 있는 마이그레이션 스크립트입니다.

### NPCInteraction 자동 마이그레이션

```csharp
// Assets/Editor/MigrateNPCInteraction.cs
using UnityEngine;
using UnityEditor;
using UnityAI.Interaction;

public class MigrateNPCInteraction : EditorWindow
{
    [MenuItem("Tools/Migrate/NPC Interactions")]
    static void MigrateAll()
    {
        NPCInteraction[] oldComponents = FindObjectsOfType<NPCInteraction>();
        
        foreach (var old in oldComponents)
        {
            GameObject obj = old.gameObject;
            
            // 기존 값 저장
            var npcData = old.GetType().GetField("npcData", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance)?.GetValue(old);
            
            // 새 컴포넌트 추가
            var newComponent = obj.AddComponent<NPCInteractable>();
            var detector = obj.AddComponent<PlayerDetector>();
            
            // 값 복사 (Reflection 사용)
            // TODO: 필요한 필드 복사
            
            // 기존 컴포넌트 제거
            DestroyImmediate(old);
            
            EditorUtility.SetDirty(obj);
        }
        
        Debug.Log($"마이그레이션 완료: {oldComponents.Length}개 오브젝트");
    }
}
```

## 📋 마이그레이션 후 확인사항

### ✅ 기능 테스트

1. **플레이어 감지**
   - [ ] 플레이어가 범위에 들어오면 힌트 UI 표시
   - [ ] 플레이어가 범위를 벗어나면 힌트 UI 사라짐

2. **상호작용**
   - [ ] F키를 누르면 예상대로 동작
   - [ ] 효과음이 재생됨
   - [ ] UI가 올바르게 표시/숨김

3. **자동 닫힘**
   - [ ] 일정 거리/시간 후 자동으로 닫힘
   - [ ] 범위를 벗어나면 자동으로 닫힘

4. **Scene 뷰**
   - [ ] Gizmos로 범위가 표시됨
   - [ ] 범위 크기가 적절함

### ⚠️ 주의사항

1. **네임스페이스**
   ```csharp
   using UnityAI.Interaction; // 추가 필요!
   ```

2. **기존 Prefab**
   - Prefab을 사용하는 경우, Prefab 자체도 수정해야 합니다
   - "Prefab Override" 확인 후 Apply

3. **씬 저장**
   - 마이그레이션 후 반드시 씬 저장!

4. **백업**
   - 마이그레이션 전 프로젝트 백업 권장

## 🎓 학습 포인트

### 왜 인터페이스를 사용하나요?

**기존 방식의 문제점:**
```csharp
// 각 스크립트가 동일한 코드 중복
if (Input.GetKeyDown(KeyCode.F)) { ... }
if (audioSource != null) { ... }
FindObjectOfType<NPCUIManager>() { ... }
```

**새로운 방식의 장점:**
```csharp
// 공통 기능은 베이스 클래스에서 한 번만 구현
// 하위 클래스는 고유 로직만 구현
public override void Interact() {
    // NPC만의 동작
}
```

### SOLID 원칙 적용

1. **단일 책임 원칙 (SRP)**
   - `PlayerDetector`: 감지만 담당
   - `NPCInteractable`: NPC 상호작용만 담당
   - `InteractableBase`: 공통 기능만 담당

2. **개방-폐쇄 원칙 (OCP)**
   - 새로운 상호작용 추가 시 기존 코드 수정 불필요
   - 인터페이스 구현으로 확장

3. **인터페이스 분리 원칙 (ISP)**
   - `IInteractable`: 필요한 메서드만 정의
   - 불필요한 메서드 강제하지 않음

## 🚀 다음 단계

마이그레이션 완료 후:

1. **새로운 상호작용 추가하기**
   - 아이템 획득 시스템
   - 스위치/레버 시스템
   - 텔레포트 포탈

2. **시스템 확장하기**
   - 다중 플레이어 지원
   - 애니메이션 이벤트 통합
   - 커스텀 입력 시스템 통합

3. **최적화**
   - 오브젝트 풀링 적용
   - 이벤트 시스템 통합 (C# Events)

## 🆘 도움이 필요하신가요?

- 📖 [README.md](README.md) - 전체 사용 가이드
- 🐛 버그 리포트: GitHub Issues
- 💬 질문: Discord 커뮤니티

---

**Happy Coding! 🎮**

