---
title: M3 적 AI 검증 매트릭스
status: complete
updated: 2026-07-11
requirements:
  - FR-AI-001
  - FR-AI-002
  - FR-AI-003
  - FR-AI-004
  - FR-AI-005
  - AC-006
tags:
  - enemy
  - validation
  - test-matrix
  - milestone
---

# M3 적 AI 검증 매트릭스

OpenSpec 4.1~4.8의 일반 적 데이터, 상태 전이, NavMesh 행동, 사망 경계와 다중 적 공간 분리를 하나의 검증 표로 통합한다.

## 상태 전이 매트릭스

| 현재 상태 | 조건 | 기대 상태·행동 | 자동 증거 | 결과 |
|---|---|---|---|---|
| Idle | 살아 있는 Player가 7m 안 | Chase·Player 방향 경로 | EnemyStateMachineTests, EnemyCombatPlayModeTests | PASS |
| Chase | Player 1.5m 안·쿨다운 완료 | Attack·Agent 정지·피해 16 | EnemyCombatPlayModeTests | PASS |
| Attack | 공격 완료 | Chase·거리·쿨다운 재평가 | EnemyStateMachineTests | PASS |
| Chase | 쿨다운 1.2초 미완료 | Chase 유지·추가 피해 없음 | EnemyCombatPlayModeTests | PASS |
| Chase | 피격 | Hit·일반 행동 중단 | EnemyStateMachineTests | PASS |
| Hit | 회복 미완료 | Hit 유지 | EnemyStateMachineTests | PASS |
| Hit | 회복 완료·타깃 유효 | Chase 또는 Attack | EnemyStateMachineTests | PASS |
| Chase/Attack | Player와 Home 거리 12m 초과 | Return·Home 목적지 | EnemyReturnPlayModeTests | PASS |
| Return | Home 0.25m 안 | Idle·Agent 정지 | EnemyReturnPlayModeTests | PASS |
| 살아 있는 모든 상태 | 체력 0 | Dead·Agent 정지·보상 신호 1회 | EnemyDeathPlayModeTests | PASS |
| Dead | 탐지·피격·미래 Tick | 상태·공격·보상 변화 없음 | EnemyStateMachineTests, EnemyDeathPlayModeTests | PASS |

## 데이터·내비게이션·다중 적 매트릭스

| 검증 대상 | 기대 | 증거 | 결과 |
|---|---|---|---|
| EnemyDefinition | 공격 ≤ 탐지 < 이탈, 런타임 상태 없음 | CombatDefinitionTests | PASS |
| NavMesh 베이크 | 데이터 에셋·시작점·목적지 유효 | EnemyNavigationPlayModeTests | PASS |
| Agent 이동 | PathComplete·0.25m 초과 실제 이동 | EnemyNavigationPlayModeTests | PASS |
| 다섯 접근 슬롯 | 반지름 1.25m·이론 쌍간 1m 초과 | EnemyApproachSlotsTests | PASS |
| 다섯 Agent 동시 접근 | 모두 PathComplete·실제 쌍간 0.5m 초과 | EnemySeparationPlayModeTests | PASS |
| 공격 중 회피 어댑터 | PlayerMovement 무적 전달 | ActorHealth·기존 Dodge 회귀 | PASS |

## 최종 자동 검증

- EditMode **54/54 passed**
- PlayMode **20/20 passed**
- OpenSpec strict validation **passed**

## 대표 문제 해결 사례

1. [[Troubleshooting/2026-07-11-editor-scene-open-asset-reference]] — 씬 전환 뒤 정의 에셋 fake-null로 Agent Speed 0 저장
2. [[Troubleshooting/2026-07-11-navmesh-reset-path-stop-order]] — `isStopped`와 `ResetPath()` 호출 순서로 정지 상태 해제
3. [[Troubleshooting/2026-07-11-training-target-trigger-collision]] — 전투 측정 타깃이 Player 회피 경로 차단

## 기능 완료 체크

| 항목 | 상태 | 근거 |
|---|---|---|
| 요구사항 추적 | 완료 | PRD·OpenSpec·Docs 20~27 |
| 정의와 런타임 상태 분리 | 완료 | EnemyDefinition·EnemyBrain |
| 자동 검증 | 완료 | EditMode 54, PlayMode 20 |
| 사망·비활성 경계 | 완료 | Dead 종단·이벤트 구독 대칭 |
| 교육 기록 | 완료 | PromptLog·DevLog·Troubleshooting |
| 플랫폼 | 완료 | Unity 6000.3.11f1 Apple Silicon Editor |

## 연결

- PRD: [[01_PRD]]
- 로드맵: [[02_ROADMAP]]
- 적 정의: [[20_ENEMY_DEFINITION]]
- 상태 머신: [[21_ENEMY_STATE_MACHINE]]
- NavMesh: [[22_ENEMY_NAVIGATION]]
- 탐지·공격: [[23_ENEMY_DETECTION_COMBAT]]
- 복귀: [[24_ENEMY_RETURN_HOME]]
- 사망: [[25_ENEMY_DEATH_GUARD]]
- 공간 분리: [[26_ENEMY_SPATIAL_SEPARATION]]
- 개발일지: [[DevLog/2026-07-11_M3-validation-matrix]]
- 프롬프트: [[PromptLog/2026-07-11_M3_validation_matrix_v01]]
