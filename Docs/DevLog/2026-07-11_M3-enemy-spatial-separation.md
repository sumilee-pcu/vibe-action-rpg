---
title: M3 개발일지 — 다섯 적 공간 분리
date: 2026-07-11
milestone: M3
requirements:
  - FR-AI-005
status: complete
tags:
  - devlog
  - enemy-separation
---

# M3 개발일지 — 다섯 적 공간 분리

## 목표

OpenSpec 4.7에 따라 다섯 근접 적이 Player를 동시에 추적할 때 한 점에 완전히 겹치는 현상을 완화한다.

## 시작 상태

- 기준 커밋: `f6b0b2d` — Guard dead enemy actions and rewards
- OpenSpec: 27/64
- 단일 MeleeGrunt만 활성, 모든 Chase 목적지는 Player 중심

## 수행 내용

1. Player 둘레 균등 원형 슬롯을 계산하는 `EnemyApproachSlots`를 만들었다.
2. EnemyBrain에 슬롯 인덱스·전체 수를 연결했다.
3. CombatSandbox에 동일 정의를 공유하는 MeleeGrunt 다섯 개를 구성했다.
4. Agent 회피 반경·우선순위와 고유 목적지를 함께 적용했다.
5. 순수 슬롯 거리와 실제 다섯 Agent 동시 접근을 검증했다.

## 검증 결과

| 항목 | 결과 |
|---|---|
| 슬롯 반지름 | 1.25m |
| 이론 인접 거리 | 1m 초과 |
| 다섯 Agent 경로 | 모두 PathComplete |
| 실제 쌍간 거리 | 모두 0.5m 초과 |
| EditMode 전체 | 54/54 passed |
| PlayMode 전체 | 20/20 passed |

## 실패와 수정

- 첫 회귀에서 수동 내비게이션 테스트의 정지 거리가 슬롯 추적 값 0.25m로 남았다. Stop 시 정의값 1.25m로 복원했다.
- 장애물을 우회하는 다섯 번째 적이 고정 1.5초 안에 도달하지 못했다. 적별 경로·이동량을 출력하고 최대 3초 조건 도달 검증으로 변경했다.

## 연결

- 공간 분리 계약: [[../26_ENEMY_SPATIAL_SEPARATION]]
- 내비게이션: [[../22_ENEMY_NAVIGATION]]
- 프롬프트: [[../PromptLog/2026-07-11_M3_enemy_spatial_separation_v01]]
