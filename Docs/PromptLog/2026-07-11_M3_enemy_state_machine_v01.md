---
title: 프롬프트 기록 — M3 일반 적 상태 머신 v01
date: 2026-07-11
milestone: M3
requirements:
  - FR-AI-001
  - FR-AI-002
  - FR-AI-003
  - FR-AI-004
status: complete
tags:
  - prompt-log
  - enemy-state-machine
---

# 프롬프트 기록 — M3 일반 적 상태 머신 v01

## 사용자 원문 요청

```text
계속 진행
```

## AI 실행 범위

```text
OpenSpec 4.2를 구현한다.
Idle, Chase, Attack, Hit, Return, Dead를 열거형과 작은 순수 전이 함수로 만든다.
Dead는 최우선 종단 상태이고 Hit은 살아 있는 상태의 일반 행동을 중단한다.
상태가 실제로 바뀔 때만 전이 이벤트를 출력하고 핵심 흐름을 EditMode에서 검증한다.
```

## 채택

- 순수 C# EnemyStateMachine
- Unity 감지 결과를 표현하는 불변 Signals 구조체
- Dead·Hit 우선순위와 명시적 switch 전이
- 실제 변경에만 단일 이벤트 출력

## 기각·보류

- 상태별 클래스·비헤이비어 트리는 현재 복잡도에 과도해 기각
- NavMeshAgent와 물리 거리 계산은 4.3~4.4로 보류
- 실제 공격 애니메이션과 피해는 4.4로 보류

## 검증 결과

- 상태 전이 집중 테스트 5개 추가
- EditMode 53/53, PlayMode 15/15 통과

## 다음

OpenSpec 4.3에서 CombatSandbox NavMesh 베이크와 Agent 이동을 구성한다.

## 연결

- PRD: [[../01_PRD]]
- 상태 계약: [[../21_ENEMY_STATE_MACHINE]]
- 개발일지: [[../DevLog/2026-07-11_M3-enemy-state-machine]]
