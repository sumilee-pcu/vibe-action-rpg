---
title: M2 개발일지 — 공격 실행별 단일 타격
date: 2026-07-11
milestone: M2
requirements:
  - FR-COMBAT-002
  - FR-COMBAT-004
status: complete
tags:
  - devlog
  - attack-execution
  - multi-collider
---

# M2 개발일지 — 공격 실행별 단일 타격

## 목표

OpenSpec 3.5·3.6에 따라 같은 적의 여러 Collider가 한 공격 판정과 겹쳐도 피해를 최대 한 번만 적용한다.

## 시작 상태

- 기준 커밋: `a34ffb6` — Connect attack animation hit window
- OpenSpec: 17/64
- 공격 Animation Event와 활성 판정 창 완료
- HealthState와 Actor·Attack 정의 완료
- 공격 실행별 대상 기억과 Unity 피해 어댑터는 없음

## 수행 내용

1. `AttackExecution`에 공격 순번과 대상 집합을 구현했다.
2. PlayerAttackController가 공격 시작마다 새 실행을 생성하도록 연결했다.
3. `ActorHealth`가 ActorDefinition과 HealthState를 연결하게 했다.
4. `AttackHitResolver`가 Collider 부모의 ActorHealth를 피해 대상으로 사용하게 했다.
5. 동일 객체 등록과 새 실행 재등록을 EditMode에서 검증했다.
6. 한 적의 두 Collider가 실제 피해를 한 번만 적용하는 PlayMode 시나리오를 검증했다.

## 검증 결과

| 항목 | 결과 |
|---|---|
| 한 실행 동일 대상 | 첫 등록 true, 두 번째 false |
| 새 실행 동일 대상 | 다시 true |
| 다중 Collider 첫 공격 | 100 → 85 |
| 다중 Collider 두 번째 공격 | 85 → 70 |
| EditMode 전체 | 47/47 passed |
| PlayMode 전체 | 14/14 passed |

## 사람의 판단

- 채택: Collider가 아닌 ActorHealth를 대상 정체성으로 사용
- 채택: 공격 실행 인스턴스가 대상 집합을 소유
- 채택: 피해 거부 여부와 무관하게 첫 시도에서 대상 등록
- 기각: 타격 후 Collider를 비활성화하는 방식
- 기각: ScriptableObject에 이미 타격한 대상 저장
- 보류: 실제 범위 OverlapSphere 실행은 피격 피드백 통합과 함께 연결

## 교육 포인트

- 물리 Collider 수와 게임 규칙상 대상 수는 다를 수 있다.
- 중복 타격 방지는 Collider가 아니라 피해 수신자 정체성을 기준으로 해야 한다.
- 실행 단위 상태는 새 공격마다 폐기해야 다음 공격이 정상 동작한다.

## 연결

- 계약: [[../17_ATTACK_EXECUTION]]
- 판정 창: [[../16_ATTACK_ANIMATION_WINDOW]]
- 프롬프트: [[../PromptLog/2026-07-11_M2_attack_execution_v01]]
