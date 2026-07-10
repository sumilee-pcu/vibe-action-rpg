---
title: M3 개발일지 — Player 탐지·추적·공격 쿨다운
date: 2026-07-11
milestone: M3
requirements:
  - FR-AI-001
  - FR-AI-002
  - FR-AI-003
  - AC-004
status: complete
tags:
  - devlog
  - enemy-combat
---

# M3 개발일지 — Player 탐지·추적·공격 쿨다운

## 목표

OpenSpec 4.4에 따라 적 상태 규칙과 NavMesh 어댑터를 Player 탐지·추적·공격 행동으로 연결한다.

## 시작 상태

- 기준 커밋: `9ce5766` — Bake enemy navigation sandbox
- OpenSpec: 24/64
- MeleeGrunt Agent는 수동 목적지로만 이동
- Player 체력 어댑터와 적 공격 쿨다운 없음

## 수행 내용

1. `EnemyBrain`이 Player 거리·생존·쿨다운을 상태 신호로 변환하게 했다.
2. Chase에서 Player 위치를 Agent 목적지로 갱신했다.
3. Attack 진입 시 Agent를 멈추고 Player ActorHealth에 16 피해를 적용했다.
4. 공격 완료 뒤 Chase로 재평가하고 1.2초 전 재공격을 거부했다.
5. Player ActorHealth를 PlayerMovementController와 연결해 회피 무적 계약을 실제 어댑터에 적용했다.
6. 기존 수동 NavMesh 이동 테스트는 EnemyBrain을 비활성화해 책임 충돌을 제거했다.

## 검증 결과

| 항목 | 결과 |
|---|---|
| 최초 상태 전이 | Idle → Chase |
| 공격 거리 | 1.4m에서 Attack |
| 첫 피해 | 100 → 84 |
| 쿨다운 중 | 추가 피해 없음 |
| 쿨다운 후 | 84 → 68 |
| EditMode 전체 | 53/53 passed |
| PlayMode 전체 | 17/17 passed |

## 사람의 판단

- 채택: 순수 상태 머신은 조건만 결정하고 EnemyBrain이 Unity 행동 수행
- 채택: 공격 완료 뒤 Chase에서 쿨다운·거리 재평가
- 채택: 쿨다운은 적 인스턴스가 소유
- 보류: 실제 적 공격 애니메이션·판정 창은 폴리싱 단계에서 교체
- 보류: 이탈·귀환은 OpenSpec 4.5에서 연결

## 연결

- 전투 계약: [[../23_ENEMY_DETECTION_COMBAT]]
- 상태 머신: [[../21_ENEMY_STATE_MACHINE]]
- 프롬프트: [[../PromptLog/2026-07-11_M3_enemy_detection_combat_v01]]
