---
title: M3 개발일지 — 일반 적 정의 데이터
date: 2026-07-11
milestone: M3
requirements:
  - FR-AI-001
  - FR-AI-002
  - FR-AI-003
  - FR-AI-004
  - FR-DATA-001
status: complete
tags:
  - devlog
  - enemy-definition
---

# M3 개발일지 — 일반 적 정의 데이터

## 목표

OpenSpec 4.1에 따라 이동, 탐지, 공격, 이탈과 보상 수치를 런타임 상태와 분리한 일반 적 정의를 만든다.

## 시작 상태

- 기준 커밋: `2588f8a` — Measure combat attack timing
- OpenSpec: 21/64
- Player ActorDefinition과 BasicAttack AttackDefinition만 존재
- 적 AI 전용 거리·정지·분리 정의 없음

## 수행 내용

1. `EnemyDefinition`이 ActorDefinition과 AttackDefinition을 조합하게 했다.
2. 탐지·공격·이탈·복귀·정지·분리 수치를 추가했다.
3. `MeleeGrunt` Actor·Attack·Enemy 에셋을 편집기 도구로 생성했다.
4. 거리 순서와 공격 쿨다운 불변조건을 `OnValidate`에서 보정했다.
5. 정의에 런타임 상태 필드가 없는지 기존 Reflection 검증을 확장했다.

## 검증 결과

| 항목 | 결과 |
|---|---|
| Actor·Attack 조합 | 참조 유효 |
| 이동속도·보상 | 3.5m/s, 25 XP |
| 거리 불변조건 | 공격 ≤ 탐지 < 이탈 |
| 런타임 상태 필드 | 없음 |
| EditMode 전체 | 48/48 passed |
| PlayMode 전체 | 15/15 passed |

## 사람의 판단

- 채택: EnemyDefinition이 기존 Actor·Attack 정의를 조합
- 채택: AI에만 필요한 거리·정지·분리 값만 EnemyDefinition에 추가
- 기각: 체력·공격력·이동속도·보상을 EnemyDefinition에 중복 저장
- 보류: 현재 상태·타깃·남은 쿨다운은 4.2 이후 인스턴스 런타임 상태로 구현

## 연결

- 데이터 계약: [[../20_ENEMY_DEFINITION]]
- 전투 정의: [[../15_COMBAT_DEFINITIONS]]
- 프롬프트: [[../PromptLog/2026-07-11_M3_enemy_definition_v01]]
