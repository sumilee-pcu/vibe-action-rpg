---
title: M1 개발일지 — 플레이어 회피·무적
date: 2026-07-11
milestone: M1
requirements:
  - FR-PLAYER-005
  - AC-004
  - NFR-MAINT-002
status: complete
tags:
  - devlog
  - dodge
  - invulnerability
---

# M1 개발일지 — 플레이어 회피·무적

## 목표

OpenSpec 2.6에 따라 Space 입력으로 짧은 회피 이동을 실행하고, 설정 가능한 시간 구간 동안 무적 상태를 공개한다.

## 시작 상태

- 기준 커밋: `31222a6` — Implement gameplay input gating
- OpenSpec: 11/64
- Gameplay/Dodge와 Space 바인딩은 이미 정의됨
- 체력·피해 시스템은 아직 없음

## 수행 내용

1. `PlayerDodgeState`에 프레임 독립 이동과 무적 시간 규칙을 구현했다.
2. `PlayerMovementController`가 Dodge 입력, 방향 선택, CharacterController 이동을 연결하도록 확장했다.
3. 4m·0.35초·무적 0.05~0.25초를 Inspector 기본값으로 설정했다.
4. Pause 등 Gameplay 맵 비활성 시 회피를 취소하도록 2.5와 연결했다.
5. 30fps·120fps 거리, 무적 경계와 취소를 EditMode에서 검증했다.
6. 실제 Space 입력, 무적 상태와 Pause 차단을 PlayMode에서 검증했다.

## 검증 결과

| 항목 | 결과 |
|---|---|
| 30fps 총 회피 거리 | 4m |
| 120fps 총 회피 거리 | 4m |
| 무적 시작 경계 | 0.05초 포함 |
| 무적 종료 경계 | 0.25초 제외 |
| 회피 중 재입력 | 거부 |
| Pause 진입 | 진행 중 회피 취소 |
| EditMode 전체 | 32/32 passed |
| PlayMode 전체 | 11/11 passed |

## 사람의 판단

- 채택: 회피 타이밍을 MonoBehaviour에서 분리한 순수 상태 객체
- 채택: 이동 입력이 없으면 현재 캐릭터 전방 사용
- 채택: 마지막 프레임 초과시간을 잘라 총 이동 거리 보존
- 채택: 피해 시스템이 나중에 연결할 `IsInvulnerable` 공개 계약
- 보류: 스태미나·회피 쿨다운·애니메이션 — 현재 MVP 요구사항에 없음
- 보류: 실제 피해 무효화 — OpenSpec 3.1 체력·피해 규칙에서 연결

## 교육 포인트

- 무적 시간과 회피 전체 시간을 같은 값으로 만들 필요는 없다.
- 프레임 독립 이동은 마지막 프레임의 초과시간까지 처리해야 총 거리가 정확하다.
- 아직 없는 체력 시스템을 미리 만들지 않고도 무적 상태 계약을 테스트할 수 있다.
- Pause는 새 입력만 막는 것이 아니라 이미 진행 중인 동작의 취소 정책도 필요하다.

## 연결

- 회피 계약: [[../12_PLAYER_DODGE]]
- 입력 차단: [[../11_GAMEPLAY_INPUT_GATING]]
- 프롬프트: [[../PromptLog/2026-07-11_M1_player_dodge_v01]]
- OpenSpec tasks: [tasks.md](../../openspec/changes/build-action-rpg-vertical-slice/tasks.md)
