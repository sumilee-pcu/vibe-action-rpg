---
title: M2 개발일지 — 실제 공격 타이밍 측정
date: 2026-07-11
milestone: M2
requirements:
  - FR-COMBAT-001
  - FR-COMBAT-003
  - AC-003
status: complete
tags:
  - devlog
  - attack-timing
---

# M2 개발일지 — 실제 공격 타이밍 측정

## 목표

OpenSpec 3.8에 따라 애니메이션의 시각 타격 시점과 실제 피해 적용 시점을 같은 경로로 묶고 수치로 검증한다.

## 시작 상태

- 기준 커밋: `69b1dec` — Add combat feedback and death transitions
- OpenSpec: 20/64
- Animation Event가 판정 창 상태만 변경하고 실제 물리 탐지는 수행하지 않음
- CombatSandbox에 피해 측정 대상 없음

## 수행 내용

1. `OpenHitWindow`에서 고정 버퍼 물리 범위 탐지를 실행했다.
2. `AttackHitResolver`에 유효 Collider 개수와 자기 자신 제외 경계를 추가했다.
3. `AttackTimingSample`로 공격 시작과 실제 피해 적용 시각을 기록했다.
4. CombatSandbox에 체력 100의 `TrainingTarget`을 추가했다.
5. 왼쪽 클릭 한 번으로 타깃 체력 100→85와 지연 시간을 함께 검증했다.
6. 측정값을 NUnit 결과 출력에 남겨 문서 수치와 실행 증거를 연결했다.

## 측정 결과

| 항목 | 결과 |
|---|---:|
| 설정 타격 시점 | 0.1500초 |
| 측정 피해 시점 | 0.1500초 |
| 오차 | 0.0000초 |
| 적용 대상 | 1개 |
| EditMode 전체 | 47/47 passed |
| PlayMode 전체 | 15/15 passed |

## 실패와 수정

- 실패: 일반 Collider 훈련 타깃이 기존 회피 테스트의 전진 경로를 차단해 PlayMode 14/15
- 원인: 전투 측정 도구가 플레이어 이동 규칙에 물리적 부작용을 추가함
- 수정: 훈련 타깃을 Trigger로 바꾸고 공격 쿼리만 Trigger를 포함
- 결과: 회피 4m와 공격 피해·타이밍 테스트 모두 통과

## M2 완료 판단

체력 불변조건, 데이터 정의, Animation Event, 실행별 단일 타격, 다중 Collider, 피격·사망 이벤트와 실제 타격 시점 측정까지 8개 작업을 완료했다. M3 적 AI로 이동할 수 있다.

## 연결

- 측정 기록: [[../19_ATTACK_TIMING_MEASUREMENT]]
- 문제 해결: [[../Troubleshooting/2026-07-11-training-target-trigger-collision]]
- 프롬프트: [[../PromptLog/2026-07-11_M2_attack_timing_measurement_v01]]
