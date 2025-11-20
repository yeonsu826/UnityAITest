# 🛠️ NPC 설정 가이드 (NPCInteractable)

## ⚙️ 단계별 설정 방법

### 1단계: 기존 컴포넌트 제거

NPC 오브젝트를 선택하고:
```
1. Inspector에서 "NPCInteraction" 컴포넌트 찾기
2. 컴포넌트 우클릭 → Remove Component
```

### 2단계: 새 컴포넌트 추가

**중요: 두 개의 컴포넌트를 모두 추가해야 합니다!**

```
NPC 오브젝트 선택 → Inspector → Add Component

1. "NPCInteractable" 검색 후 추가
2. "PlayerDetector" 검색 후 추가
```

### 3단계: NPCInteractable 설정

```
NPCInteractable 컴포넌트:

[상호작용 설정]
- Interaction Key: F
- Interaction Hint Text: "F키를 눌러 대화하기"
- Show Interaction Hint: ✅ (체크)
- Show Debug Logs: ✅ (체크) ← 디버깅용

[오디오]
- Interaction Sound: (선택사항)
- Sound Volume: 0.5

[UI]
- UI Manager: (자동으로 찾아짐)

[NPC 설정]
- NPC Data: [NPCData ScriptableObject 할당] ← 필수!

[자동 닫힘 설정]
- Auto Close Distance: 5
```

### 4단계: PlayerDetector 설정

```
PlayerDetector 컴포넌트:

[감지 방식]
- Detection Mode: Distance ← NPC는 거리 모드 추천!

[거리 기반 설정]
- Detection Range: 3.0
- Check Interval: 0.2

[디버그]
- Show Debug Logs: ✅ (체크) ← 문제 해결용
- Show Gizmos: ✅ (체크)
```

### 5단계: NPC Data 생성 (없는 경우)

```
1. Project 창에서 우클릭
2. Create → NPCData (또는 ScriptableObject → NPCData)
3. NPC 정보 입력:
   - NPC Name: "홍길동"
   - Description: "안녕하세요!"
4. NPCInteractable의 NPC Data 필드에 드래그 앤 드롭
```

### 6단계: 플레이어 태그 확인

```
Hierarchy에서 플레이어 오브젝트 선택
→ Inspector 상단
→ Tag: "Player" 설정

❗ 태그가 없으면 감지되지 않습니다!
```

### 7단계: UI Manager 확인

```
Scene에 NPCUIManager가 있어야 합니다!

확인 방법:
1. Hierarchy에서 "NPCUIManager" 검색
2. 없으면 생성:
   - 빈 GameObject 생성
   - NPCUIManager 컴포넌트 추가
   - Canvas와 UI 설정
```

### 8단계: 테스트

```
1. Play 버튼 누르기
2. 플레이어를 NPC 근처로 이동
3. Console 창 확인:
   - "[PlayerDetector] 플레이어 범위 진입" 로그 확인
   - "F키를 눌러 대화하기" UI 표시 확인
4. F키 누르기
5. Console 창 확인:
   - "[NPCInteractable] 상호작용 - [NPC이름]" 로그 확인
   - NPC 설명 UI 표시 확인
```

## 🐛 문제 해결

### 문제 1: "아무 반응이 없어요"

**원인:** PlayerDetector가 없거나 설정이 잘못됨

**해결:**
1. Inspector에서 PlayerDetector 컴포넌트가 있는지 확인
2. Detection Mode가 "Distance"인지 확인
3. Detection Range가 3.0 이상인지 확인

### 문제 2: "F키를 눌러도 UI가 안 떠요"

**가능한 원인:**

**A. NPC Data가 없음**
```
Console 에러: "NPC Data가 설정되지 않았습니다"
해결: NPC Data ScriptableObject를 생성하고 할당
```

**B. UI Manager가 없음**
```
Console 에러: "UI Manager가 없습니다"
해결: Scene에 NPCUIManager 추가
```

**C. 플레이어가 감지되지 않음**
```
Console에 "플레이어 범위 진입" 로그가 안 나옴
해결: 플레이어에 "Player" 태그 추가
```

### 문제 3: "NPCUIManager를 찾을 수 없습니다"

**해결:**
```
1. Hierarchy 우클릭 → Create Empty
2. 이름을 "NPCUIManager"로 변경
3. NPCUIManager 컴포넌트 추가
4. Canvas와 UI Panel 설정
   (기존 NPCUIManager 스크립트 참고)
```

### 문제 4: "Component를 찾을 수 없어요"

**원인:** namespace 문제

**해결:**
다른 스크립트에서 참조할 때:
```csharp
using UnityAI.Interaction;
```

### 문제 5: "거리가 맞는데도 반응이 없어요"

**해결:**
1. Show Debug Logs 활성화
2. Console 확인
3. Scene 뷰에서 Gizmos로 범위 확인:
   - 노란색/초록색 구체: Detection Range
   - 플레이어가 이 범위 안에 있는지 확인

## 📋 최종 체크리스트

설정이 완료되면 다음을 확인하세요:

- [ ] NPCInteractable 컴포넌트 추가됨
- [ ] PlayerDetector 컴포넌트 추가됨
- [ ] PlayerDetector의 Detection Mode = Distance
- [ ] PlayerDetector의 Detection Range = 3.0
- [ ] NPC Data ScriptableObject 생성 및 할당
- [ ] 플레이어에 "Player" 태그 설정
- [ ] Scene에 NPCUIManager 있음
- [ ] Play 모드에서 플레이어가 NPC에 접근하면 힌트 UI 표시됨
- [ ] F키를 누르면 NPC 설명 UI가 뜸
- [ ] Console에 디버그 로그가 나타남

## 🎮 정상 작동 시 Console 로그 예시

```
[PlayerDetector] Ok 플레이어 찾음: Player
[NPCInteractable] 초기화 완료 - NPC Data: ✓, UI Manager: ✓
[PlayerDetector] NPC_01: 플레이어 범위 진입
[NPCInteractable] NPC_01: 상호작용 - 홍길동
[NPCInteractable] UI 표시: 홍길동
```

## 💡 팁

### Gizmos로 범위 확인
Scene 뷰에서 NPC 오브젝트를 선택하면:
- 노란색/초록색 구체: Detection Range (플레이어 감지 범위)
- 빨간색 구체: Auto Close Distance (UI 자동 닫힘 범위)

### 감지 범위 조정
NPC와의 거리에 따라:
- 가까이서만 대화: Detection Range = 2.0
- 멀리서도 대화: Detection Range = 5.0
- 일반적인 경우: Detection Range = 3.0

### UI 자동 닫힘 조정
- Auto Close Distance > Detection Range 추천
- 예: Detection Range = 3, Auto Close Distance = 5

## 🔍 디버깅 팁

### 1. 플레이어 감지 확인
```
Show Debug Logs 체크 → Play 모드
플레이어를 NPC에 가까이 이동
Console에 "플레이어 범위 진입" 로그 확인
```

### 2. F키 입력 확인
```
플레이어가 범위 안에 있는 상태에서
F키를 누르면 "상호작용" 로그 확인
```

### 3. UI 표시 확인
```
"UI 표시" 로그 후 Canvas에 UI가 나타나는지 확인
안 나타나면 NPCUIManager 설정 확인
```

## 📸 Scene 뷰에서 보이는 것

정상 설정 시:
- 초록색 구체: 플레이어가 범위 안에 있을 때
- 노란색 구체: 플레이어가 범위 밖에 있을 때
- 빨간색 구체: UI 자동 닫힘 거리

## 🆘 여전히 안 되나요?

다음 정보를 확인해주세요:
1. Console 창의 전체 로그 (특히 에러 메시지)
2. Inspector 스크린샷 (NPCInteractable + PlayerDetector)
3. Hierarchy 구조 (NPC 오브젝트와 플레이어)
4. 플레이어가 NPC에 접근할 때 아무 일도 일어나지 않는지

