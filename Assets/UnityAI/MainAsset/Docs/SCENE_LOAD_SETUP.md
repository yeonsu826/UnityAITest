# 씬 자동 로드 설정 가이드

Core 씬 시작 시 Lobby 씬을 자동으로 로드하는 설정입니다.

## 📋 목차
1. [빠른 설정](#빠른-설정)
2. [Build Settings 설정](#build-settings-설정)
3. [SceneLoadManager 설정](#sceneloadmanager-설정)
4. [테스트](#테스트)
5. [문제 해결](#문제-해결)

---

## ⚡ 빠른 설정

### 1단계: Build Settings에 씬 추가

1. Unity 메뉴: `File > Build Settings`
2. **Core** 씬을 드래그하여 "Scenes In Build"에 추가 (Index 0)
3. **Lobby** 씬을 드래그하여 "Scenes In Build"에 추가 (Index 1)

```
Scenes In Build:
☑ [0] Core
☑ [1] Lobby
```

### 2단계: SceneLoadManager 추가

1. Core 씬 열기
2. Hierarchy에서 빈 오브젝트 생성 → 이름: `SceneLoadManager`
3. `SceneLoadManager.cs` 스크립트 추가

### 3단계: Inspector 설정

```
SceneLoadManager
├─ Scenes To Load
│  └─ Size: 1
│      └─ Element 0: "Lobby"
├─ Load On Start: ✓
├─ Use Additive Mode: ✓
└─ Show Debug Logs: ✓
```

**완료!** 이제 Core 씬을 실행하면 Lobby 씬이 자동으로 로드됩니다.

---

## 🎮 Build Settings 설정

### 씬 추가 방법

#### 방법 1: 드래그 앤 드롭
1. `File > Build Settings` 열기
2. Project 창에서 씬 파일 찾기:
   - `Assets/UnityAI/MainAsset/Scenes/Core.unity`
   - `Assets/UnityAI/MainAsset/Scenes/Lobby.unity`
3. "Scenes In Build" 영역으로 드래그

#### 방법 2: Add Open Scenes
1. Core 씬 열기
2. `File > Build Settings`
3. `Add Open Scenes` 버튼 클릭
4. Lobby 씬 열기
5. 다시 `Add Open Scenes` 버튼 클릭

### 씬 순서

```
✓ 중요: Core 씬이 첫 번째(Index 0)에 있어야 합니다!

Scenes In Build:
☑ [0] Assets/UnityAI/MainAsset/Scenes/Core.unity
☑ [1] Assets/UnityAI/MainAsset/Scenes/Lobby.unity
```

### 씬 순서 변경

- 드래그하여 순서 변경
- Index 0이 빌드 시 첫 번째로 로드되는 씬입니다

---

## 🔧 SceneLoadManager 설정

### Inspector 옵션 설명

#### 자동 로드 설정

**Scenes To Load**
- 자동으로 로드할 씬 이름 목록
- 씬 파일 이름만 입력 (확장자 제외)
- 예: `"Lobby"` (Lobby.unity 파일)

```
여러 씬 로드 예제:
Size: 3
Element 0: "Lobby"
Element 1: "GameUI"
Element 2: "AudioManager"
```

#### 로드 옵션

**Load On Start**
- ✓: Core 씬 시작 시 자동 로드
- ☐: 수동으로 로드 (스크립트로 호출)

**Use Additive Mode**
- ✓: Additive 모드 (씬 겹치기) - **권장**
  - Core 씬 + Lobby 씬이 동시에 활성화
  - 두 씬의 오브젝트가 모두 보임
- ☐: Single 모드 (씬 교체)
  - Core 씬이 언로드되고 Lobby 씬만 남음

#### 디버그

**Show Debug Logs**
- ✓: 콘솔에 로딩 과정 출력
- ☐: 로그 없이 조용히 로드

---

## 🎯 작동 방식

### Additive 모드 (권장)

```
게임 시작
  └─> Core 씬 로드 (Index 0)
       └─> SceneLoadManager.Start() 호출
            └─> Lobby 씬 Additive 로드
                 └─> Core + Lobby 씬 동시 활성화
```

**결과:**
- Core 씬의 오브젝트 (플레이어, 카메라 등)
- Lobby 씬의 오브젝트 (UI, 배경 등)
- 모두 동시에 보임

### Single 모드

```
게임 시작
  └─> Core 씬 로드
       └─> Lobby 씬 Single 로드
            └─> Core 씬 언로드
                 └─> Lobby 씬만 활성화
```

---

## ✅ 테스트

### 1. Unity 에디터에서 테스트

1. Core 씬 열기
2. Play 버튼 클릭
3. Console 확인:

```
[SceneLoadManager] 'Lobby' 씬 로딩 시작... (모드: Additive)
[SceneLoadManager] 'Lobby' 씬 로딩 완료!
```

4. Hierarchy 확인:
   - Core 씬 오브젝트들
   - Lobby 씬 오브젝트들 (모두 보여야 함)

### 2. 빌드 테스트

1. `File > Build Settings`
2. 씬이 올바르게 추가되었는지 확인
3. `Build` 버튼 클릭
4. 빌드된 게임 실행
5. Core + Lobby 씬이 모두 보이는지 확인

---

## 🐛 문제 해결

### 문제: Lobby 씬이 로드되지 않음

**원인 1: Build Settings에 씬이 없음**

```
에러: Unable to load scene 'Lobby'. 
Make sure it has been added to the build settings.
```

**해결:**
- `File > Build Settings` 열기
- Lobby 씬이 "Scenes In Build"에 있는지 확인
- 없으면 드래그하여 추가
- 체크박스가 ✓ 되어 있는지 확인

---

**원인 2: 씬 이름이 다름**

SceneLoadManager의 `Scenes To Load`에 입력한 이름과 실제 씬 파일 이름이 다를 수 있습니다.

**확인:**
- Project 창에서 씬 파일 이름 확인
- 정확히 `Lobby.unity`인가요?
- 대소문자 구분됨! (`lobby` ≠ `Lobby`)

**해결:**
- SceneLoadManager Inspector에서 `Scenes To Load > Element 0`에 정확한 이름 입력

---

**원인 3: SceneLoadManager가 없음**

**해결:**
- Core 씬에 SceneLoadManager 오브젝트가 있는지 확인
- SceneLoadManager.cs 스크립트가 추가되어 있는지 확인

---

### 문제: Core 씬 오브젝트가 사라짐

**원인: Single 모드로 설정됨**

SceneLoadManager의 `Use Additive Mode`가 체크 해제되어 있습니다.

**해결:**
- SceneLoadManager Inspector
- `Use Additive Mode` ✓ 체크

---

### 문제: 씬이 중복 로드됨

Play를 중지했다가 다시 실행하면 Lobby 씬이 계속 추가됩니다.

**원인:** 이미 로드된 씬을 다시 로드

**해결:**
SceneLoadManager는 자동으로 중복 체크를 합니다. 콘솔 확인:

```
[SceneLoadManager] 'Lobby' 씬은 이미 로드되어 있습니다.
```

문제가 계속되면:
1. Unity 에디터 완전 재시작
2. Play Mode 상태 확인

---

### 문제: 빌드에서는 작동하지만 에디터에서 안됨

**해결:**
1. Core 씬을 열고 다시 저장 (`Ctrl + S`)
2. Lobby 씬도 열고 저장
3. Unity 재시작

---

## 📱 고급 기능

### 여러 씬 동시 로드

```
Scenes To Load:
Size: 3
Element 0: "Lobby"
Element 1: "GameUI"
Element 2: "AudioSystem"
```

### 코드에서 씬 로드

```csharp
// SceneLoadManager 참조
SceneLoadManager manager = FindFirstObjectByType<SceneLoadManager>();

// 씬 로드
manager.LoadScene("AnotherScene");

// 씬 언로드
manager.UnloadScene("Lobby");

// 비동기 로드 (로딩 화면)
manager.LoadSceneAsync("HeavyScene", 
    onProgress: (progress) => {
        Debug.Log($"로딩: {progress * 100}%");
    },
    onComplete: () => {
        Debug.Log("로딩 완료!");
    }
);
```

### 현재 로드된 씬 확인

SceneLoadManager를 우클릭 → `현재 로드된 씬 목록 출력`

콘솔 출력:
```
[SceneLoadManager] 현재 로드된 씬 개수: 2
  [0] Core (로드됨: True)
  [1] Lobby (로드됨: True)
```

---

## 🎨 씬 구조 권장 사항

### Core 씬 (필수 시스템)
```
Core
├─ Player
├─ Camera
├─ SceneLoadManager
├─ UIManager
└─ GameManagers
```

### Lobby 씬 (UI/환경)
```
Lobby
├─ UI Canvas
├─ Environment
├─ Lighting
└─ Audio
```

### 장점
- **모듈화**: 각 씬의 역할 분리
- **협업**: 여러 명이 동시에 작업 가능
- **성능**: 필요한 씬만 로드/언로드
- **관리**: 씬 파일 작고 빠름

---

## 📝 체크리스트

빌드 전 확인:

- [ ] `File > Build Settings`에 Core 씬 추가 (Index 0)
- [ ] `File > Build Settings`에 Lobby 씬 추가 (Index 1)
- [ ] 두 씬 모두 체크박스 ✓
- [ ] Core 씬에 SceneLoadManager 오브젝트 추가
- [ ] SceneLoadManager의 `Scenes To Load`에 "Lobby" 입력
- [ ] `Load On Start` ✓
- [ ] `Use Additive Mode` ✓
- [ ] Unity 에디터에서 테스트 완료
- [ ] 빌드해서 테스트 완료

---

완성! 🎉

이제 Core 씬을 실행하면 Lobby 씬이 자동으로 함께 로드됩니다.

