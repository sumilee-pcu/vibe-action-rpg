---
title: M1 개발일지 — 카메라 기준 플레이어 이동
date: 2026-07-10
milestone: M1
requirements:
  - FR-PLAYER-001
  - FR-PLAYER-003
  - NFR-MAINT-001
  - NFR-MAINT-002
  - NFR-DOC-001
status: complete
tags:
  - devlog
  - player-control
  - movement
---

# M1 개발일지 — 카메라 기준 플레이어 이동

## 목표

OpenSpec 작업 2.2에 따라 WASD 입력을 카메라의 수평 전방과 오른쪽 축으로 변환하고, 프레임레이트와 무관한 속도로 플레이어를 이동·회전시킨다.

## 시작 상태

- 기준 커밋: '67b9a25' — Define MVP input actions
- Gameplay/Move와 WASD 바인딩 정의 완료
- CombatSandbox에는 Main Camera, Directional Light, Global Volume만 존재
- 런타임 플레이어 코드와 실제 이동 오브젝트 없음
- 실제 Unity 프로젝트는 열려 있어 lock 파일이 존재

## 수행 내용

1. 카메라 기준 방향·거리·회전을 계산하는 'CameraRelativeMovement'를 추가했다.
2. InputActionAsset과 CharacterController를 연결하는 'PlayerMovementController'를 추가했다.
3. 이동에 'moveSpeed × deltaTime', 회전에 'degreesPerSecond × deltaTime'을 적용했다.
4. 카메라 pitch를 제거하고 대각선 입력을 단위 길이로 제한했다.
5. Ground, Player, Visual, Facing Marker와 고정 사선 카메라를 CombatSandbox에 구성했다.
6. Editor 도구로 샌드박스 구성을 반복 생성·검증할 수 있게 했다.
7. 이동 수학 EditMode 테스트 6개와 실제 W 입력 PlayMode 테스트 1개를 추가했다.
8. 열린 실제 프로젝트를 강제 종료하지 않고 임시 복사본에서 씬을 생성하고 전체 테스트를 실행했다.

## 변경된 자산

- 계산: 'CameraRelativeMovement.cs'
- Unity 어댑터: 'PlayerMovementController.cs'
- 씬 생성 도구: 'PlayerMovementSandboxTools.cs'
- 씬: 'CombatSandbox.unity'
- EditMode 테스트: 'CameraRelativeMovementTests.cs'
- PlayMode 테스트: 'PlayerMovementPlayModeTests.cs'
- 설명: [[08_PLAYER_MOVEMENT]]

## 검증 결과

| 항목 | 기대 결과 | 실제 결과 | 판정 |
|---|---|---|---|
| 카메라 yaw 변환 | W가 카메라 수평 전방 | 방향 오차 0.0001 미만 | 통과 |
| 카메라 pitch 제거 | 이동 Y=0 | 오차 0.0001 미만 | 통과 |
| 대각선 정규화 | 방향 크기 ≤ 1 | 1.0 | 통과 |
| 30fps 이동 | 6m/s, 2초 = 12m | 12m | 통과 |
| 120fps 이동 | 6m/s, 2초 = 12m | 12m | 통과 |
| 방향 회전 | 초당 각도 기반 회전 | 0.5초 뒤 잔여 45° | 통과 |
| 전체 EditMode | 기존 회귀 포함 | 13/13 passed | 통과 |
| 실제 W 입력 | 카메라 전방 이동·방향 회전 | PlayMode 시나리오 통과 | 통과 |
| 전체 PlayMode | 기존 회귀 포함 | 2/2 passed | 통과 |
| 컴파일·런타임 | 새 오류·예외 0개 | 0개 | 통과 |

Cinemachine 패키지 내부 HDRP 샘플 asmref 경고는 기존 패키지 경고이며 이번 변경으로 새 프로젝트 경고는 추가되지 않았다.

## 실패와 수정

### 고정 프레임 수로 회전 완료를 가정

- 첫 PlayMode 테스트는 20프레임 뒤 방향 정렬을 기대했다.
- 배치 모드가 매우 높은 프레임레이트로 실행되어 실제 경과시간이 짧았고 방향 내적이 0.8926에 머물렀다.
- 프레임 수 대신 0.25초의 게임 시간을 진행하도록 수정했다.

### PlayerInput과 InputTestFixture 수명주기 충돌

- PlayerInput의 런타임 액션 복제와 장치 감시가 테스트 Fixture 복원 순서와 충돌했다.
- 씬 종료 시 'listenForUnpairedDeviceActivity' 카운터가 음수가 되는 예외가 발생했다.
- 로컬 싱글플레이 범위에 불필요한 PlayerInput을 제거하고 직렬화된 InputActionAsset의 Gameplay 맵을 Controller가 직접 관리하도록 단순화했다.

### UnityTest 입력은 다음 프레임에 처리됨

- InputTestFixture의 Press 직후 키 상태를 검사해 값이 0으로 나타났다.
- UnityTest에서는 이벤트가 Player Loop에 큐잉된다는 패키지 구현을 확인했다.
- 한 프레임 진행 후 키 상태와 Move 값을 확인하도록 수정했다.

상세 기록: [[../Troubleshooting/2026-07-10-input-system-playmode-test-lifecycle]]

## 사람의 판단

- 채택: 이동 계산과 MonoBehaviour 어댑터 분리
- 채택: CharacterController 기반 이동과 단순 중력
- 채택: 고정 사선 카메라로 카메라 기준 변환을 먼저 검증
- 수정: PlayerInput을 사용한 초기 구조를 직접 InputActionAsset 소비 구조로 축소
- 기각: 작업 2.2에서 Cinemachine 추적과 마우스 카메라까지 함께 구현하는 범위 확대
- 보류: 애니메이션 기반 루트 모션과 실제 캐릭터 에셋

## 기능 완료 체크

- [x] FR-PLAYER-001, FR-PLAYER-003과 OpenSpec 작업 2.2를 식별했다.
- [x] 포함·제외 범위와 사용자가 관찰할 수 있는 완료 조건을 기록했다.
- [x] 프레임마다 오브젝트 검색이나 컬렉션 할당을 만들지 않았다.
- [x] 런타임 계산과 Editor 씬 생성 도구를 어셈블리로 분리했다.
- [x] 카메라 기준 이동과 프레임 독립성을 자동 테스트했다.
- [x] CombatSandbox 실제 W 입력 시나리오를 PlayMode에서 검증했다.
- [x] 실패한 테스트 접근과 사람의 수정 판단을 기록했다.
- [ ] 마우스 카메라 조작 — 작업 2.3 범위
- [ ] macOS·Windows 빌드 — M1 통합 완료 시 검증

## 교육 포인트

- 프레임 독립성은 Update를 쓴다는 사실이 아니라 변화량에 deltaTime을 곱했는지로 결정된다.
- 카메라 기준 이동에서는 camera.forward의 Y 성분을 제거해야 경사 시점이 이동 높이를 바꾸지 않는다.
- PlayMode 테스트에서 “20프레임”과 “0.25초”는 같은 조건이 아니다.
- 범용 컴포넌트가 항상 좋은 선택은 아니다. PlayerInput의 다중 사용자 기능은 현재 싱글플레이 범위에 불필요했다.
- UnityTest의 가상 입력은 큐에 들어간 뒤 다음 Player Loop에서 적용될 수 있다.

## 연결

- PRD: [[01_PRD]]
- 이동 계약: [[08_PLAYER_MOVEMENT]]
- 입력 계약: [[07_INPUT_ACTIONS]]
- 프롬프트: [[../PromptLog/2026-07-10_M1_camera_relative_movement_v01]]
- 오류 기록: [[../Troubleshooting/2026-07-10-input-system-playmode-test-lifecycle]]
- OpenSpec tasks: [tasks.md](../../openspec/changes/build-action-rpg-vertical-slice/tasks.md)
