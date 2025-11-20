# NPC 시스템 설정 가이드

이 가이드는 미술관 NPC 시스템을 Unity 프로젝트에 설정하는 방법을 설명합니다.

## 📋 목차
1. [필수 구성 요소](#필수-구성-요소)
2. [NPC 설정하기](#npc-설정하기)
3. [UI 설정하기](#ui-설정하기)
4. [NPC 데이터 생성하기](#npc-데이터-생성하기)
5. [플레이어 설정](#플레이어-설정)
6. [테스트하기](#테스트하기)
7. [문제 해결](#문제-해결)

---

## 필수 구성 요소

### 스크립트
다음 스크립트들이 `Assets/UnityAI/MainAsset/Scripts/` 폴더에 있어야 합니다:
- `NPCInteraction.cs` - NPC 인터렉션 메인 로직
- `NPCData.cs` - NPC 데이터 ScriptableObject
- `NPCUIManager.cs` - UI 표시/숨김 관리

### Unity 패키지
- **TextMesh Pro**: UI에서 텍스트 표시를 위해 필요합니다.
  - Window > TextMeshPro > Import TMP Essential Resources

---

## NPC 설정하기

### 1단계: NPC GameObject 생성

1. Hierarchy에서 우클릭 > `Create Empty` (또는 기존 3D 모델 사용)
2. 이름을 `NPC_Guide` 등으로 변경

### 2단계: Collider 추가

1. NPC GameObject 선택
2. Add Component > `Sphere Collider`
3. Inspector에서 다음 설정:
   - **Is Trigger**: ✅ 체크
   - **Radius**: `3` (인터렉션 범위)
   - Center를 적절히 조정

### 3단계: NPCInteraction 스크립트 추가

1. NPC GameObject 선택
2. Add Component > `NPCInteraction`
3. Inspector에서 설정:
   - **Interaction Range**: `3` (F키 입력 가능 거리)
   - **Auto Close Distance**: `5` (UI 자동 닫힘 거리)
   - **Interaction Key**: `F`
   - **Interaction Sound**: 효과음 AudioClip 할당 (선택사항)
   - **Sound Volume**: `0.5`
   - **UI Manager**: 나중에 설정

---

## UI 설정하기

### 1단계: Canvas 생성

1. Hierarchy에서 우클릭 > `UI > Canvas`
2. Canvas 설정:
   - **Render Mode**: `Screen Space - Overlay`
   - **Canvas Scaler** 설정:
     - UI Scale Mode: `Scale With Screen Size`
     - Reference Resolution: `1920 x 1080`

### 2단계: Description Panel 생성

1. Canvas 하위에 우클릭 > `UI > Panel`
2. 이름을 `DescriptionPanel`로 변경
3. RectTransform 설정:
   - **Width**: `800`
   - **Height**: `600`
   - **Anchors**: Center
4. Image 컴포넌트 설정:
   - **Color**: 반투명 검은색 (R:26, G:26, B:26, A:230)

### 3단계: UI 요소들 추가

#### A. 제목 텍스트
1. DescriptionPanel 하위에 우클릭 > `UI > Text - TextMeshPro`
2. 이름: `TitleText`
3. 설정:
   - **Text**: "작품 제목"
   - **Font Size**: `32`
   - **Alignment**: Center
   - **Position**: 상단에 배치

#### B. 설명 텍스트
1. DescriptionPanel 하위에 우클릭 > `UI > Text - TextMeshPro`
2. 이름: `DescriptionText`
3. 설정:
   - **Text**: "작품 설명이 여기에 표시됩니다."
   - **Font Size**: `20`
   - **Alignment**: Top Left
   - **Position**: 중앙 영역에 배치
   - **Auto Size**: Off

#### C. 작가명 텍스트
1. DescriptionPanel 하위에 우클릭 > `UI > Text - TextMeshPro`
2. 이름: `ArtistText`
3. 설정:
   - **Text**: "작가: "
   - **Font Size**: `18`
   - **Alignment**: Left

#### D. 제작 연도 텍스트
1. DescriptionPanel 하위에 우클릭 > `UI > Text - TextMeshPro`
2. 이름: `YearText`
3. 설정:
   - **Text**: "제작 연도: "
   - **Font Size**: `18`

#### E. 이미지 (선택사항)
1. DescriptionPanel 하위에 우클릭 > `UI > Image`
2. 이름: `ExhibitImage`
3. 설정:
   - **Width**: `300`
   - **Height**: `300`
   - 적절한 위치에 배치

#### F. 닫기 버튼
1. DescriptionPanel 하위에 우클릭 > `UI > Button - TextMeshPro`
2. 이름: `CloseButton`
3. 설정:
   - **Text**: "닫기 (ESC)"
   - **Position**: 우측 상단 또는 하단에 배치

### 4단계: NPCUIManager 스크립트 추가

1. Canvas GameObject 선택
2. Add Component > `NPCUIManager`
3. Inspector에서 연결:
   - **Description Panel**: DescriptionPanel 드래그
   - **Title Text**: TitleText 드래그
   - **Description Text**: DescriptionText 드래그
   - **Artist Text**: ArtistText 드래그
   - **Year Text**: YearText 드래그
   - **Exhibit Image**: ExhibitImage 드래그 (있는 경우)
   - **Close Button**: CloseButton 드래그
   - **Use Fade Animation**: ✅ 체크
   - **Fade Duration**: `0.3`

### 5단계: NPC와 UI Manager 연결

1. NPC GameObject 선택
2. Inspector의 NPCInteraction 컴포넌트에서:
   - **UI Manager**: Canvas를 드래그하거나 NPCUIManager 컴포넌트 할당

---

## NPC 데이터 생성하기

### 1단계: ScriptableObject 생성

1. Project 창에서 `Assets/UnityAI/MainAsset/` 폴더에 우클릭
2. `Create > UnityAI > NPC Data`
3. 파일명: 예) `NPC_Artwork_01`

### 2단계: 데이터 입력

생성된 NPC Data 파일을 선택하고 Inspector에서 입력:

```
NPC Name: 미술관 가이드
Exhibit Title: 모나리자
Description: 레오나르도 다 빈치의 대표작으로...
Artist Name: 레오나르도 다 빈치
Year Created: 1503-1519
Exhibit Image: [이미지 Sprite 할당]
Font Size: 18
Background Color: (26, 26, 26, 230)
```

### 3단계: NPC에 데이터 할당

1. NPC GameObject 선택
2. Inspector의 NPCInteraction 컴포넌트에서:
   - **Npc Data**: 생성한 NPC Data 파일을 드래그

---

## 플레이어 설정

### 방법 1: Tag 사용 (권장)

1. 플레이어 GameObject 선택
2. Inspector 상단의 **Tag** 드롭다운 클릭
3. `Player` 태그 선택 (없으면 Add Tag로 생성)

### 방법 2: PlayerMovement 스크립트

- 플레이어에 `PlayerMovement` 스크립트가 있으면 자동으로 찾습니다.

---

## 테스트하기

### 1단계: 플레이 모드 실행

1. Play 버튼 클릭
2. 플레이어를 NPC 근처로 이동

### 2단계: 인터렉션 테스트

1. NPC에 가까이 가기 (3m 이내)
2. **F키** 입력
3. ✅ 효과음이 재생되어야 함
4. ✅ 설명 UI가 표시되어야 함

### 3단계: 거리 테스트

1. UI가 표시된 상태에서 NPC에서 멀어지기
2. ✅ 5m 이상 떨어지면 UI가 자동으로 닫혀야 함

### 4단계: Scene View에서 범위 확인

1. Scene View에서 NPC 선택
2. ✅ 초록색 원 (인터렉션 범위 3m)
3. ✅ 빨간색 원 (자동 닫힘 거리 5m)

---

## 문제 해결

### UI가 표시되지 않음

- [ ] Canvas에 NPCUIManager가 추가되었는지 확인
- [ ] NPC의 UI Manager 필드에 Canvas가 할당되었는지 확인
- [ ] DescriptionPanel과 모든 Text 오브젝트가 연결되었는지 확인
- [ ] NPC Data가 NPC에 할당되었는지 확인

### F키를 눌러도 반응 없음

- [ ] 플레이어가 NPC로부터 3m 이내에 있는지 확인
- [ ] 플레이어에 "Player" Tag가 설정되었는지 확인
- [ ] Console에 에러 메시지가 없는지 확인

### 효과음이 나지 않음

- [ ] NPC에 AudioSource가 있는지 확인 (자동 생성되어야 함)
- [ ] Interaction Sound에 AudioClip이 할당되었는지 확인
- [ ] AudioListener가 Scene에 있는지 확인 (보통 Main Camera에 있음)

### UI가 자동으로 닫히지 않음

- [ ] Auto Close Distance가 Interaction Range보다 큰지 확인
- [ ] 플레이어 Transform이 올바르게 감지되는지 확인

---

## 고급 설정

### 여러 NPC 만들기

1. NPC GameObject를 복제 (Ctrl+D)
2. 새로운 NPC Data ScriptableObject 생성
3. 복제된 NPC에 새 데이터 할당

### 힌트 UI 추가하기

NPCInteraction.cs의 `OnPlayerEnterRange()` 메서드에서:
- "F키를 눌러 대화하기" 같은 힌트 UI 표시 로직 추가

### 애니메이션 추가하기

- DescriptionPanel에 Animator 추가
- Scale 애니메이션 또는 Slide 애니메이션 적용

### 다국어 지원

- NPCData에 언어별 description 필드 추가
- 현재 언어 설정에 따라 적절한 텍스트 표시

---

## 📝 요약

1. ✅ NPC GameObject에 NPCInteraction 스크립트 추가
2. ✅ Canvas에 UI 요소들 생성 및 NPCUIManager 추가
3. ✅ NPC Data ScriptableObject 생성 및 내용 입력
4. ✅ 모든 참조 연결
5. ✅ 플레이어에 Tag 설정
6. ✅ 테스트!

문제가 발생하면 Console 창을 확인하세요. 대부분의 경우 유용한 에러 메시지가 표시됩니다.

