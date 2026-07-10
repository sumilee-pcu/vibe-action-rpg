---
title: M3 개발일지 — 일반 적 상태 머신
date: 2026-07-11
milestone: M3
requirements:
  - FR-AI-001
  - FR-AI-002
  - FR-AI-003
  - FR-AI-004
status: complete
tags:
  - devlog
  - enemy-state-machine
---

# M3 개발일지 — 일반 적 상태 머신

## 목표

OpenSpec 4.2에 따라 Idle, Chase, Attack, Hit, Return, Dead 상태와 전이 우선순위를 순수 C#으로 구현한다.

## 시작 상태

- 기준 커밋: `5faa883` — Add melee enemy definition data
- OpenSpec: 22/64
- MeleeGrunt 정의 에셋은 있으나 현재 상태와 전이 규칙은 없음

## 수행 내용

1. `EnemyState` 열거형으로 여섯 상태를 명시했다.
2. `EnemyStateSignals`로 Unity 감지 결과와 순수 전이 입력을 분리했다.
3. `EnemyStateMachine`에 사망·피격 우선순위와 상태별 전이를 구현했다.
4. 실제 상태 변경 때만 `StateChanged`와 전이 횟수를 출력하게 했다.
5. 정상 흐름, 이탈·귀환, 피격·회복, 공격 완료와 Dead 종단성을 검증했다.

## 검증 결과

| 시나리오 | 결과 |
|---|---|
| Idle → Chase → Attack | 통과 |
| 이탈 → Return → Idle | 통과 |
| Chase → Hit → Chase | 통과 |
| Attack 완료 → Chase | 통과 |
| Dead 이후 일반 신호 | 전이 없음 |
| EditMode 전체 | 53/53 passed |
| PlayMode 전체 | 15/15 passed |

## 사람의 판단

- 채택: 열거형과 작은 명시적 전이 함수
- 채택: Dead > Hit > 상태별 조건 순서
- 채택: Attack 완료 뒤 Chase로 돌아가 다음 Tick에서 거리·쿨다운 재평가
- 기각: 초기부터 상태 패턴 클래스와 비헤이비어 트리 도입
- 보류: NavMesh·거리 계산·공격 실행은 4.3~4.4 Unity 어댑터에서 연결

## 연결

- 상태 계약: [[../21_ENEMY_STATE_MACHINE]]
- 적 정의: [[../20_ENEMY_DEFINITION]]
- 프롬프트: [[../PromptLog/2026-07-11_M3_enemy_state_machine_v01]]
