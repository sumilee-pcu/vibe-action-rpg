---
title: M3 개발일지 — 이탈과 홈 복귀
date: 2026-07-11
milestone: M3
requirements:
  - FR-AI-004
status: complete
tags:
  - devlog
  - enemy-return
---

# M3 개발일지 — 이탈과 홈 복귀

## 목표

OpenSpec 4.5에 따라 전투 허용 영역을 벗어난 Player 추적을 중단하고 적을 초기 홈 위치로 복귀시킨다.

## 시작 상태

- 기준 커밋: `4f953e5` — Connect enemy detection and attacks
- OpenSpec: 25/64
- 탐지·추적·공격은 동작하지만 이탈 없이 계속 추적

## 수행 내용

1. EnemyBrain이 Awake 시 홈 위치를 저장하게 했다.
2. Player와 홈의 거리를 DisengageRange와 비교했다.
3. 이탈 신호를 상태 머신에 전달해 Return으로 전환했다.
4. Return에서 홈 목적지를 설정하고 정지 거리를 0.25m로 변경했다.
5. 홈 허용 반경 도달 시 Idle과 Agent 정지를 검증했다.

## 검증 결과

| 항목 | 결과 |
|---|---|
| 홈 기준 이탈 거리 | 12m 초과 시 Return |
| Return 목적지 | 홈 0.083m 이내 |
| Return 정지 거리 | 0.25m |
| 홈 도착 상태 | Idle·정지 |
| 복귀 중 공격 | 0회 |
| EditMode 전체 | 53/53 passed |
| PlayMode 전체 | 18/18 passed |

## 수정 판단

최초 테스트는 NavMesh 목적지가 홈과 0.05m 이내여야 한다고 가정했지만, 베이크 격자 보정값은 0.083m였다. 구현값을 숨기기 위해 임계값을 임의 확대하지 않고 제품 정의의 HomeTolerance 0.25m를 테스트 기준으로 사용했다.

## 연결

- 복귀 계약: [[../24_ENEMY_RETURN_HOME]]
- 상태 머신: [[../21_ENEMY_STATE_MACHINE]]
- 프롬프트: [[../PromptLog/2026-07-11_M3_enemy_return_home_v01]]
