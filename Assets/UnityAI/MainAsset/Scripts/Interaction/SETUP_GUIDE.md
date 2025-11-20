# 🛠️ 문 설정 가이드 (DoorInteractable)

## ⚙️ 단계별 설정 방법

### 1단계: 기존 컴포넌트 제거

문 오브젝트를 선택하고:
```
1. Inspector에서 "AutoRotatingDoor" 컴포넌트 찾기
2. 컴포넌트 우클릭 → Remove Component
```

### 2단계: 새 컴포넌트 추가

**중요: 두 개의 컴포넌트를 모두 추가해야 합니다!**

```
문 오브젝트 선택 → Inspector → Add Component

1. "DoorInteractable" 검색 후 추가
2. "PlayerDetector" 검색 후 추가
```

### 3단계: DoorInteractable 설정

```
DoorInteractable 컴포넌트:

[상호작용 설정]
- Interaction Key: F
- Interaction Hint Text: "F키를 눌러 문 열기"
- Show Interaction Hint: ✅ (체크)
- Show Debug Logs: ✅ (체크) ← 디버깅용

[오디오]
- Interaction Sound: (선택사항)
- Sound Volume: 0.5

[UI]
- UI Manager: (자동으로 찾아짐)

[문 설정]
- Open Angle: 90
- Rotation Speed: 3

[자동 닫힘 설정]
- Auto Close: ✅ (체크)
- Auto Close Delay: 3

[문 사운드]
- Open Sound: (선택사항)
- Close Sound: (선택사항)
```

### 4단계: PlayerDetector 설정

```
PlayerDetector 컴포넌트:

[감지 방식]
- Detection Mode: Trigger ← 문은 트리거 모드 추천!

[트리거 기반 설정]
- Auto Create Trigger: ✅ (체크) ← 자동으로 BoxCollider 생성
- Trigger Size: 
  X: 3
  Y: 2
  Z: 3
- Trigger Center:
  X: 0
  Y: 0
  Z: 0

[디버그]
- Show Debug Logs: ✅ (체크) ← 문제 해결용
- Show Gizmos: ✅ (체크)
```

### 5단계: 플레이어 태그 확인

```
Hierarchy에서 플레이어 오브젝트 선택
→ Inspector 상단
→ Tag: "Player" 설정

❗ 태그가 없으면 감지되지 않습니다!
```

### 6단계: 테스트

```
1. Play 버튼 누르기
2. 플레이어를 문 근처로 이동
3. Console 창 확인:
   - "[PlayerDetector] 플레이어 범위 진입" 로그 확인
   - "F키를 눌러 문 열기" UI 표시 확인
4. F키 누르기
5. Console 창 확인:
   - "[DoorInteractable] 문 열림" 로그 확인
```

## 🐛 문제 해결

### 문제 1: "아무 반응이 없어요"

**원인:** PlayerDetector가 없거나 설정이 잘못됨

**해결:**
1. Inspector에서 PlayerDetector 컴포넌트가 있는지 확인
2. Detection Mode가 "Trigger"인지 확인
3. Auto Create Trigger가 체크되어 있는지 확인

### 문제 2: "F키를 눌러도 안 열려요"

**원인:** 플레이어가 감지되지 않음

**해결:**
1. Console 창에 "[PlayerDetector] 플레이어 범위 진입" 로그가 나오는지 확인
2. 플레이어에 "Player" 태그가 있는지 확인
3. Show Debug Logs를 체크해서 로그 확인

### 문제 3: "Component를 찾을 수 없어요"

**원인:** namespace 문제 또는 컴파일 에러

**해결:**
1. Unity Editor 재시작
2. Assets → Reimport All
3. Console에 컴파일 에러가 있는지 확인

### 문제 4: "BoxCollider가 두 개 생겨요"

**원인:** 기존 BoxCollider가 있었는데 PlayerDetector가 또 만듦

**해결:**
1. 기존 BoxCollider 제거
2. PlayerDetector의 Auto Create Trigger 다시 체크
   또는
1. Auto Create Trigger 체크 해제
2. 기존 BoxCollider의 Is Trigger를 체크

### 문제 5: "Scene 뷰에서 범위가 안 보여요"

**해결:**
1. 문 오브젝트 선택 (Hierarchy에서 클릭)
2. Scene 뷰에서 Gizmos 버튼 확인 (켜져있는지)
3. PlayerDetector의 Show Gizmos 체크

## 📋 최종 체크리스트

설정이 완료되면 다음을 확인하세요:

- [ ] DoorInteractable 컴포넌트 추가됨
- [ ] PlayerDetector 컴포넌트 추가됨
- [ ] PlayerDetector의 Detection Mode = Trigger
- [ ] PlayerDetector의 Auto Create Trigger = 체크
- [ ] 플레이어에 "Player" 태그 설정
- [ ] Play 모드에서 플레이어가 문에 접근하면 힌트 UI 표시됨
- [ ] F키를 누르면 문이 열림
- [ ] Console에 디버그 로그가 나타남 (Show Debug Logs 체크 시)

## 🎮 정상 작동 시 Console 로그 예시

```
[PlayerDetector] Door: 플레이어 범위 진입
[DoorInteractable] Door: 문 열림 (각도: 90°)
[DoorInteractable] Door: 문 닫힘
[PlayerDetector] Door: 플레이어 범위 퇴장
```

## 💡 팁

### Gizmos로 범위 확인
Scene 뷰에서 문 오브젝트를 선택하면:
- 파란색/청록색 박스: 트리거 범위
- 빨간색/초록색 화살표: 문의 방향

### 트리거 크기 조정
문의 크기에 따라 Trigger Size 조정:
- 큰 문: (4, 3, 4)
- 작은 문: (2, 2, 2)
- 긴 복도: (5, 2, 2)

