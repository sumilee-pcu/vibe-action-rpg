---
title: M2 개발일지 — 피격·사망·피해 출력 이벤트
date: 2026-07-11
milestone: M2
requirements:
  - FR-COMBAT-004
  - FR-COMBAT-005
status: complete
tags:
  - devlog
  - combat-feedback
---

# M2 개발일지 — 피격·사망·피해 출력 이벤트

## 목표

OpenSpec 3.7에 따라 적용된 피해를 피격 반응, 사망 전환과 데미지 숫자 UI의 공통 이벤트로 전달하고 중복 출력을 막는다.

## 시작 상태

- 기준 커밋: `5f8328a` — Enforce one hit per attack execution
- OpenSpec: 19/64
- HealthState의 피해·사망 불변조건과 공격 실행별 중복 타격 차단 완료
- Unity 피격 반응과 UI 소비용 피해 이벤트는 없음

## 수행 내용

1. `DamageAppliedEvent`에 실제 피해량, 대상, 위치와 치명 여부를 정의했다.
2. `ActorHealth`가 실제 피해 적용 때만 이벤트를 내보내도록 연결했다.
3. `ActorReactionController`가 Hit 반응과 Death 전환을 구독하도록 구현했다.
4. 사망 시 지정 행동을 끄고 Player 공격을 즉시 취소·거부하도록 연결했다.
5. 다중 Collider 공격에서도 피해 출력이 한 번인지 기존 PlayMode 테스트를 확장했다.
6. 비치명·치명·사망 후 추가 피해의 이벤트 횟수를 집중 검증했다.

## 검증 결과

| 항목 | 결과 |
|---|---|
| 비치명 피해 | DamageApplied 1회, Hit 반응 1회 |
| 치명 피해 | DamageApplied 1회, Died·Death 전환 각 1회 |
| 사망 후 피해 | 모든 출력 추가 발생 없음 |
| 다중 Collider | 공격 실행당 DamageApplied 1회 |
| EditMode 전체 | 47/47 passed |
| PlayMode 전체 | 15/15 passed |

## 사람의 판단

- 채택: UI 프리팹 대신 월드 위치를 포함한 순수 출력 이벤트 제공
- 채택: 치명 피해에서 일반 Hit와 Death 반응의 중복 실행 방지
- 채택: 이벤트 구독과 해제를 컴포넌트 활성 수명에 맞춰 대칭 처리
- 보류: 실제 월드 공간 숫자 생성·수명 관리는 OpenSpec 6.5에서 구현
- 보류: 실제 공격 범위 탐지와 프레임 타이밍 측정은 OpenSpec 3.8에서 구현

## 연결

- 계약: [[../18_COMBAT_FEEDBACK_EVENTS]]
- 공격 실행: [[../17_ATTACK_EXECUTION]]
- 프롬프트: [[../PromptLog/2026-07-11_M2_combat_feedback_events_v01]]
