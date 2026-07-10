---
title: 프롬프트 기록 — M3 일반 적 정의 데이터 v01
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
  - prompt-log
  - enemy-definition
---

# 프롬프트 기록 — M3 일반 적 정의 데이터 v01

## 사용자 원문 요청

```text
계속 진행
```

## AI 실행 범위

```text
OpenSpec 4.1을 구현한다.
EnemyDefinition은 ActorDefinition과 AttackDefinition을 조합하고
탐지·공격·이탈·복귀·정지·분리 수치만 추가한다.
현재 상태·타깃·쿨다운 같은 런타임 값은 저장하지 않는다.
MeleeGrunt 기본 에셋과 거리 불변조건 테스트를 제공한다.
```

## 채택

- Actor·Attack 정의 조합
- AI 전용 거리 데이터만 별도 소유
- 편집기 도구 기반 재현 가능한 기본 에셋 생성
- Reflection 기반 런타임 상태 필드 금지 검증

## 기각·보류

- 기존 ActorDefinition 수치 중복은 기각
- 상태 머신과 현재 타깃은 OpenSpec 4.2 이후로 보류
- NavMeshAgent 연결은 OpenSpec 4.3으로 보류

## 검증 결과

- EditMode 48/48, PlayMode 15/15 통과
- MeleeGrunt Actor·Attack·Enemy 에셋 참조와 거리 불변조건 통과

## 다음

OpenSpec 4.2에서 Idle·Chase·Attack·Hit·Return·Dead 상태 전이를 순수 C# 규칙으로 구현한다.

## 연결

- PRD: [[../01_PRD]]
- 데이터 계약: [[../20_ENEMY_DEFINITION]]
- 개발일지: [[../DevLog/2026-07-11_M3-enemy-definition]]
