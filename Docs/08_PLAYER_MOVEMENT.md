---
title: 카메라 기준 플레이어 이동 계약
status: active
updated: 2026-07-10
requirements:
  - FR-PLAYER-001
  - FR-PLAYER-003
  - NFR-MAINT-001
  - NFR-MAINT-002
tags:
  - unity
  - player-control
  - movement
  - character-controller
---

# 카메라 기준 플레이어 이동 계약

이 문서는 OpenSpec 작업 2.2의 이동 계산, Unity 연결 구조와 검증 기준을 설명한다.

## 범위

### 포함

- WASD Vector2 입력을 카메라 수평 축 기준 월드 방향으로 변환
- 대각선 입력 속도 제한
- 'speed × deltaTime' 기반 프레임 독립 이동
- 이동 방향을 향한 초당 각도 기반 회전
- CharacterController 충돌과 단순 중력
- CombatSandbox의 프로토타입 플레이어와 바닥

### 제외

- 마우스 카메라 회전과 추적: 작업 2.3
- 카메라 장애물 회피: 작업 2.4
- 세션 상태별 입력 차단: 작업 2.5
- 회피 이동과 무적: 작업 2.6
- 애니메이션과 실제 캐릭터 에셋: 후속 전투·폴리싱 작업

## 이동 계산

카메라의 위아래 기울기는 이동 높이에 영향을 주면 안 된다. 따라서 먼저 카메라 전방을 바닥 평면에 투영한다.

~~~text
planarForward = ProjectOnPlane(camera.forward, worldUp).normalized
planarRight   = Cross(worldUp, planarForward)
worldMove     = ClampMagnitude(
                  planarRight * input.x + planarForward * input.y,
                  1)
displacement  = worldMove * moveSpeed * deltaTime
~~~

카메라가 거의 수직을 바라봐 수평 전방을 만들 수 없으면 월드 전방을 안전한 대체값으로 사용한다.

## 방향 회전

입력이 0이 아니면 캐릭터는 이동 방향을 목표 회전으로 사용한다. 한 프레임의 최대 회전량은 다음과 같다.

~~~text
maximumDegreesDelta = turnSpeedDegreesPerSecond * deltaTime
rotation = RotateTowards(current, target, maximumDegreesDelta)
~~~

입력이 0이면 마지막 방향을 유지한다. 프로토타입 캡슐은 앞뒤 구분이 어려워 앞쪽에 작은 'Facing Marker' 큐브를 배치했다.

## 런타임 구조

| 구성 | 책임 |
|---|---|
| 'CameraRelativeMovement' | 방향 변환, 이동 거리, 목표 방향 회전 계산 |
| 'PlayerMovementController' | Input System, 카메라 Transform, CharacterController 연결 |
| 'TinyVanguardInput' | Gameplay/Move 액션과 WASD 바인딩 |
| 'CharacterController' | 바닥과 장애물 충돌 이동 |

'CameraRelativeMovement'는 MonoBehaviour가 아닌 정적 계산 코드라 EditMode에서 직접 검증할 수 있다. 'PlayerMovementController'는 매 프레임 오브젝트 검색을 하지 않고 Awake에서 참조를 확보한다.

## 기본 조정값

| 항목 | 값 | 의미 |
|---|---:|---|
| Move Speed | 5 | 초당 수평 이동 거리 |
| Turn Speed | 720°/s | 이동 방향을 향한 최대 회전 속도 |
| Gravity | -20 | 단순 수직 가속도 |
| Grounded Vertical Velocity | -2 | 접지 상태 유지용 하향 속도 |

## CombatSandbox 구성

- 'Ground': 20×1×20 크기의 충돌 바닥
- 'Player': 원점에 배치된 CharacterController 루트
- 'Visual': 플레이어 자식 캡슐
- 'Facing Marker': 플레이어의 앞 방향 표시
- 'Main Camera': (-7, 7, -7)의 고정 사선 카메라

카메라가 월드 축과 다른 방향을 보도록 배치했기 때문에 W 입력이 단순 월드 +Z가 아니라 카메라가 바라보는 수평 방향으로 이동하는지 눈으로 확인할 수 있다.

## 자동 검증

### EditMode

- 카메라 yaw가 월드 이동 방향에 반영됨
- 카메라 pitch가 수직 이동을 만들지 않음
- 대각선 입력 크기가 1을 넘지 않음
- 30fps와 120fps에서 2초간 6m/s 이동 결과가 모두 12m
- 초당 회전량 적용
- 입력 0에서 방향 유지

전체 EditMode 결과: 13/13 passed.

### PlayMode

가상 키보드 W 입력을 CombatSandbox에 주입하고 다음을 확인했다.

1. 수평 이동 거리가 0보다 크다.
2. 이동 방향과 카메라 수평 전방의 내적이 0.98보다 크다.
3. 캐릭터 전방과 실제 이동 방향의 내적이 0.95보다 크다.

전체 PlayMode 결과: 2/2 passed.

## 수동 확인 시나리오

1. CombatSandbox 씬을 연다.
2. Play를 누른다.
3. W를 누르면 화면 안쪽의 카메라 수평 전방으로 이동한다.
4. A, S, D를 각각 눌러 카메라 기준 좌·후·우 이동을 확인한다.
5. 입력 방향이 바뀌면 Facing Marker가 이동 방향을 향한다.
6. 키를 놓으면 캐릭터가 마지막 방향을 유지한다.

## 연결

- 입력 계약: [[07_INPUT_ACTIONS]]
- 개발일지: [[DevLog/2026-07-10_M1-camera-relative-movement]]
- 프롬프트: [[PromptLog/2026-07-10_M1_camera_relative_movement_v01]]
- 오류 기록: [[Troubleshooting/2026-07-10-input-system-playmode-test-lifecycle]]
- OpenSpec: [player-control spec](../openspec/changes/build-action-rpg-vertical-slice/specs/player-control/spec.md)
