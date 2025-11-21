# 키 찾기 시스템 설정 가이드

키를 찾아서 문을 열 수 있는 시스템입니다.

## 📋 목차
1. [시스템 개요](#시스템-개요)
2. [USBKey 설정](#usbkey-설정)
3. [문(Door) 설정](#문door-설정)
4. [UI 설정](#ui-설정)
5. [테스트](#테스트)

---

## 🎮 시스템 개요

**작동 방식:**
1. 플레이어가 `USBKey_Yellow`에 접근
2. F키를 눌러 키 획득
3. 화면 중앙에 **"획득!"** 메시지 표시 (2초간, 노란색)
4. 키 오브젝트 사라짐
5. 문에 접근하면 F키로 열 수 있음

**키 없을 때:**
- 문에 접근하면 "키를 찾아야 문을 열 수 있습니다" 힌트 표시
- F키를 눌러도 문이 열리지 않음

---

## 🔑 USBKey 설정

### 1. USBKey_Yellow 오브젝트 생성

```
Hierarchy:
USBKey_Yellow
├─ 3D Model (USB 모델)
├─ Box Collider (Is Trigger ✓)
├─ KeyItem.cs
└─ PlayerDetector.cs
```

### 2. KeyItem 컴포넌트 설정

**Inspector 설정:**

#### 키 설정
- **Key Id**: `YellowKey` (고유 ID)
- **Key Display Name**: `노란색 USB 키`

#### 획득 효과
- **Destroy On Pickup**: ✓ (획득 후 오브젝트 제거)
- **Destroy Delay**: 0.1초

#### 회전 효과 (선택)
- **Rotate Key**: ✓ (키가 회전)
- **Rotation Speed**: 50
- **Rotation Axis**: (0, 1, 0) - Y축 회전

#### 떠다니는 효과 (선택)
- **Float Key**: ✓ (위아래로 떠다님)
- **Float Amplitude**: 0.2 (떠다니는 높이)
- **Float Speed**: 1 (떠다니는 속도)

#### 상호작용 설정 (InteractableBase)
- **Interaction Key**: F
- **Interaction Hint Text**: `F키를 눌러 노란색 USB 키 획득`
- **Show Interaction Hint**: ✓

#### 오디오 (선택)
- **Interaction Sound**: 키 획득 효과음 (선택)
- **Sound Volume**: 0.5

### 3. PlayerDetector 컴포넌트 설정

**Inspector 설정:**

#### 감지 방식
- **Detection Mode**: `Distance` 또는 `Both`

#### 거리 기반 설정
- **Detection Range**: 3 (플레이어 감지 거리)
- **Check Interval**: 0.2초

#### 트리거 기반 설정 (선택)
- **Auto Create Trigger**: ✓ (자동으로 Box Collider 생성)
- **Trigger Size**: (3, 2, 3)
- **Trigger Center**: (0, 0, 0)

---

## 🚪 문(Door) 설정

### 1. 문 구조 (기존 Door_Pivot 사용)

```
Hierarchy:
Door_Pivot
├─ DoorInteractable.cs
├─ PlayerDetector.cs
├─ Door (실제 문 모델)
└─ DoorTrigger (새로 만들기)
   └─ Box Collider (Is Trigger ✓)
```

### 2. DoorInteractable 컴포넌트 설정

**Inspector 설정:**

#### 키 필요 설정 ⭐ **중요**
- **Require Key**: ✓ (키가 필요함)
- **Required Key Id**: `YellowKey` (필요한 키 ID)
- **No Key Hint Text**: `키를 찾아야 문을 열 수 있습니다`

#### 문 설정
- **Open Angle**: 90 (열림 각도)
- **Rotation Speed**: 3 (회전 속도)

#### 트리거 설정 (선택 - 회전 문제 해결)
- **Separate Trigger**: DoorTrigger 드래그
- **Lock Trigger Rotation**: ✓

#### 자동 닫힘 설정
- **Auto Close**: ✓
- **Auto Close Delay**: 3초

#### 문 사운드 (선택)
- **Open Sound**: 문 열리는 소리
- **Close Sound**: 문 닫히는 소리
- **Locked Sound**: 잠긴 문 소리 (키 없을 때)

#### 상호작용 설정
- **Interaction Key**: F
- **Show Interaction Hint**: ✓

### 3. DoorTrigger 생성 (선택 - 회전 문제 해결)

문이 회전할 때 트리거도 같이 회전하는 문제를 해결하려면:

1. Door_Pivot의 자식으로 빈 오브젝트 생성
2. 이름: `DoorTrigger`
3. Box Collider 추가 → **Is Trigger** ✓
4. 크기와 위치 조정
5. DoorInteractable의 **Separate Trigger**에 DoorTrigger 드래그
6. PlayerDetector의 **External Trigger**에 DoorTrigger 드래그

---

## 📺 UI 설정

### 1. PlayerStatusUI 오브젝트 생성

```
Hierarchy:
Canvas
├─ PlayerStatusText (상태 텍스트)
│  └─ Text (UI Text Component)
└─ AcquisitionText (획득 메시지 텍스트)
   ├─ Text (UI Text Component)
   └─ PlayerStatusUI.cs
```

### 2. PlayerStatusText 설정 (상태 텍스트)

**Text 컴포넌트:**
- **Text**: `상태: 정상`
- **Font Size**: 20
- **Color**: White
- **Alignment**: Left, Top

**위치:**
- **Anchor**: Top-Left
- **Position**: (100, -50) 정도

### 3. AcquisitionText 설정 (획득 메시지) ⭐ **중요**

**Text 컴포넌트:**
- **Text**: (비워둠 - 코드에서 설정)
- **Font Size**: 40 (크게!)
- **Color**: Yellow
- **Alignment**: Center, Middle
- **Font Style**: Bold (선택)

**위치:**
- **Anchor**: Center
- **Position**: (0, 100) - 화면 중앙 위쪽
- **Width x Height**: 300 x 100

### 4. PlayerStatusUI 컴포넌트 설정

**Inspector 설정:**

#### UI 텍스트
- **Status Text**: PlayerStatusText의 Text 컴포넌트 드래그
- **Acquisition Text**: AcquisitionText의 Text 컴포넌트 드래그 ⭐

#### 텍스트 설정
- **Default Text**: `상태: 정상`
- **Normal Color**: White

#### 획득 메시지 설정 ⭐
- **Acquisition Message**: `획득!`
- **Acquisition Color**: Yellow
- **Acquisition Display Time**: 2초 (메시지 표시 시간)

#### 애니메이션 효과 (선택)
- **Animate Acquisition**: ✓ (획득 시 애니메이션)
- **Animation Duration**: 2초

---

## 🧪 테스트

### 1. 씬 실행 전 체크리스트

- [ ] USBKey_Yellow에 KeyItem.cs와 PlayerDetector.cs 추가
- [ ] KeyItem의 Key Id가 `YellowKey`로 설정됨
- [ ] Door_Pivot에 DoorInteractable.cs의 **Require Key** ✓
- [ ] DoorInteractable의 Required Key Id가 `YellowKey`로 설정됨
- [ ] Canvas에 PlayerStatusText와 AcquisitionText 생성 ⭐
- [ ] AcquisitionText에 PlayerStatusUI.cs 추가
- [ ] PlayerStatusUI의 Status Text와 Acquisition Text 연결됨 ⭐
- [ ] 플레이어에 "Player" 태그 설정됨

### 2. 실행 후 테스트

**시나리오 1: 키 없이 문 접근**
1. 게임 시작
2. 문에 접근
3. ✅ "키를 찾아야 문을 열 수 있습니다" 힌트 표시
4. F키 누름
5. ✅ 문이 열리지 않음 (잠긴 소리 재생)

**시나리오 2: 키 획득 후 문 열기**
1. USBKey_Yellow에 접근
2. ✅ "F키를 눌러 노란색 USB 키 획득" 힌트 표시
3. F키 누름
4. ✅ 키 오브젝트 사라짐
5. ✅ 화면 중앙에 **"획득!"** 메시지 표시 (노란색, 2초간) ⭐
6. ✅ 메시지가 펄스 애니메이션과 함께 서서히 사라짐
7. 문에 접근
8. ✅ "F키를 눌러 문 열기" 힌트 표시
9. F키 누름
10. ✅ 문이 열림

---

## 🐛 문제 해결

### 문제: 키를 주워도 "획득!" 메시지가 안뜸

**해결:**
- PlayerStatusUI가 씬에 있는지 확인
- PlayerStatusUI의 **Acquisition Text**가 제대로 연결되었는지 확인 ⭐
- AcquisitionText 오브젝트가 Canvas의 자식인지 확인
- Console에서 "[PlayerStatusUI] 획득 메시지 텍스트가 설정되지 않았습니다!" 경고 확인

### 문제: 키를 주워도 문이 안열림

**해결:**
- KeyItem의 **Key Id**와 DoorInteractable의 **Required Key Id**가 동일한지 확인
- 대소문자 구분됨! (`YellowKey` ≠ `yellowkey`)

### 문제: 문에 접근해도 힌트가 안뜸

**해결:**
- PlayerDetector가 Door_Pivot에 있는지 확인
- Detection Mode가 Distance 또는 Both로 설정되었는지 확인
- Detection Range가 충분히 큰지 확인 (최소 3)

### 문제: 문이 회전할 때 트리거도 같이 회전함

**해결:**
- DoorTrigger를 별도로 생성 (위 가이드 참조)
- DoorInteractable의 Separate Trigger에 DoorTrigger 연결
- PlayerDetector의 External Trigger에 DoorTrigger 연결

---

## 💡 추가 기능 확장

### 여러 개의 키 사용

다른 색상의 키를 추가하려면:

1. **새 키 오브젝트 생성** (예: `USBKey_Red`)
2. KeyItem의 **Key Id**를 다르게 설정 (예: `RedKey`)
3. 다른 문의 DoorInteractable에서 **Required Key Id**를 `RedKey`로 설정

### 키 제거 (일회용 키)

키를 사용하면 인벤토리에서 제거하고 싶다면:

```csharp
// DoorInteractable.cs의 OpenDoor() 메서드에 추가
KeyInventory.Instance.RemoveKey(requiredKeyId);
```

### 키 개수 표시

여러 키를 모으는 게임이라면 PlayerStatusUI를 수정:

```csharp
// PlayerStatusUI.cs 수정
statusText.text = $"키: {KeyInventory.Instance.GetAllKeys().Count}개";
```

---

## 📝 스크립트 목록

생성된 스크립트:
1. `KeyInventory.cs` - 키 인벤토리 싱글톤 (자동 생성)
2. `KeyItem.cs` - 주울 수 있는 키 아이템
3. `PlayerStatusUI.cs` - 플레이어 상태 UI
4. `DoorInteractable.cs` - 키 필요 기능 추가 (수정됨)

기존 스크립트:
- `InteractableBase.cs` - 상호작용 베이스 클래스
- `PlayerDetector.cs` - 플레이어 감지
- `PlayerMovement.cs` - 플레이어 이동

---

완성! 🎉

