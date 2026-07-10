---
title: 플레이어 회피·무적 계약
status: active
updated: 2026-07-11
requirements:
  - FR-PLAYER-005
  - AC-004
  - NFR-MAINT-002
  - NFR-TEST-001
tags:
  - unity
  - player
  - dodge
  - invulnerability
---

# 플레이어 회피·무적 계약

OpenSpec 2.6에서 구현한 프레임 독립 회피 이동과 설정 가능한 무적 구간을 정의한다. 이번 단계는 이동과 무적 상태 계약까지 완료하며, 실제 피해 거부 연결은 체력·피해 규칙을 만드는 OpenSpec 3.1에서 수행한다.

## 기본값

| 항목 | 값 | 의미 |
|---|---:|---|
| 입력 | Space | Gameplay/Dodge |
| 총 이동 거리 | 4m | 회피 한 번의 수평 이동량 |
| 지속시간 | 0.35초 | 회피 시작부터 종료까지 |
| 무적 시작 | 0.05초 | 시작 직후 준비 구간 이후 |
| 무적 종료 | 0.25초 | 종료 0.10초 전 |

무적 구간은 시작을 포함하고 종료를 포함하지 않는 `[0.05, 0.25)` 규칙이다. 모든 값은 `PlayerMovementController`의 Inspector에서 조정할 수 있다.

## 방향 규칙

1. 이동 입력이 있으면 카메라 기준 이동 방향으로 회피한다.
2. 이동 입력이 없으면 캐릭터가 바라보는 방향으로 회피한다.
3. 회피 중 일반 이동 입력은 회피 경로를 바꾸지 않는다.
4. 활성 회피가 끝나기 전 새 회피 입력은 거부한다.

## 구조

- `PlayerDodgeState`: 방향, 경과시간, 무적 구간, 프레임별 이동량을 계산하는 순수 상태 객체
- `PlayerMovementController`: Input System, CharacterController, 중력과 회피 상태를 연결
- `IsInvulnerable`: 이후 피해 시스템이 읽을 공개 계약

`PlayerDodgeState.Step`은 마지막 프레임의 초과 시간을 잘라내므로 30fps와 120fps 모두 정확히 4m를 이동한다.

## 입력 상태 연동

- Gameplay 맵이 꺼지면 진행 중 회피를 즉시 취소한다.
- Paused, Victory, Defeat에서는 Dodge 액션이 비활성이다.
- 취소와 회피 종료 후 `IsInvulnerable`은 즉시 false가 된다.

## 자동 검증

### EditMode 32/32

- 무적 구간 시작·종료 경계
- 활성 회피 중 재시작 거부
- 30fps와 120fps에서 총 4m 이동
- 취소 후 이동과 무적 종료
- 기존 입력·이동·카메라 규칙 전체 회귀

### PlayMode 11/11

- Space 실제 입력으로 회피 시작
- 0.1초 시점에 회피 중이며 무적 상태
- 종료 후 약 4m 이동하고 무적 해제
- Pause가 진행 중 회피를 취소
- Pause 중 새 Space 입력으로 이동하지 않음
- 기존 이동·카메라·가림·입력 차단 회귀

## 수동 확인

1. CombatSandbox에서 Play한다.
2. WASD를 누른 상태로 Space를 눌러 카메라 기준 방향 회피를 확인한다.
3. 이동 입력 없이 Space를 눌러 전방 회피를 확인한다.
4. 회피 도중 Space를 반복해 거리가 중첩되지 않는지 확인한다.
5. 회피 도중 Pause로 전환해 이동이 즉시 멈추는지 확인한다.

## 연결

- PRD: [[01_PRD]]
- 입력 차단: [[11_GAMEPLAY_INPUT_GATING]]
- 개발일지: [[DevLog/2026-07-11_M1-player-dodge]]
- 프롬프트: [[PromptLog/2026-07-11_M1_player_dodge_v01]]
- OpenSpec: [player-control spec](../openspec/changes/build-action-rpg-vertical-slice/specs/player-control/spec.md)
