---
title: M3 개발일지 — 사망 행동·보상 차단
date: 2026-07-11
milestone: M3
requirements:
  - FR-AI-001
  - FR-AI-005
  - AC-006
status: complete
tags:
  - devlog
  - enemy-death
---

# M3 개발일지 — 사망 행동·보상 차단

## 목표

OpenSpec 4.6에 따라 사망한 적이 이동·공격하지 않고 보상 신호를 반복 출력하지 않게 한다.

## 시작 상태

- 기준 커밋: `563d946` — Return disengaged enemies home
- OpenSpec: 26/64
- 상태 머신 Dead 종단 규칙은 있으나 ActorHealth 사망 이벤트와 보상 신호 미연결

## 수행 내용

1. EnemyBrain이 ActorHealth.Died를 활성 수명에 맞춰 구독·해제하게 했다.
2. 최초 사망에서 즉시 Dead 전이와 Agent 정지를 실행했다.
3. 경험치 시스템용 `RewardAvailable(int)` 신호를 한 번 출력했다.
4. 중복 사망 호출 방어와 처리·보상 신호 횟수를 인스턴스 상태로 관리했다.
5. 사망 후 추가 피해와 미래 Tick에서도 공격·보상이 늘지 않는지 검증했다.

## 검증 결과

| 항목 | 결과 |
|---|---|
| Dead 전이 | 치명 피해 즉시 |
| Agent | 정지 |
| 사망 처리 | 1회 |
| 보상 신호 | 25 XP, 1회 |
| 사망 후 공격 | 증가 없음 |
| EditMode 전체 | 53/53 passed |
| PlayMode 전체 | 19/19 passed |

## 사람의 판단

- 채택: ActorHealth Died와 EnemyBrain 중복 방어의 이중 경계
- 채택: Player 진행도와 결합하지 않은 보상 가능 신호
- 보류: 실제 Player 경험치 증가는 OpenSpec 5.4에서 연결
- 기각: Dead 컴포넌트를 즉시 Destroy해 상태 검증과 사망 연출을 불가능하게 하는 방식

## 연결

- 사망 계약: [[../25_ENEMY_DEATH_GUARD]]
- 체력 규칙: [[../14_HEALTH_DAMAGE_DEATH]]
- 프롬프트: [[../PromptLog/2026-07-11_M3_enemy_death_guard_v01]]
