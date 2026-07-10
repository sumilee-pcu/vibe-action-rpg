---
title: 프롬프트 기록 — M3 NavMesh 베이크와 Agent 이동 v01
date: 2026-07-11
milestone: M3
requirements:
  - FR-AI-002
  - FR-AI-003
status: complete
tags:
  - prompt-log
  - navmesh
---

# 프롬프트 기록 — M3 NavMesh 베이크와 Agent 이동 v01

## 사용자 원문 요청

```text
계속 진행
```

## AI 실행 범위

```text
OpenSpec 4.3을 구현한다.
CombatSandbox Collider 지형을 NavMeshSurface로 베이크하고 데이터 에셋으로 저장한다.
MeleeGrunt NavMeshAgent를 EnemyDefinition 수치로 설정한다.
PlayMode에서 완전 경로, 실제 이동과 안전한 정지를 검증한다.
```

## 채택

- AI Navigation 2.0.11 NavMeshSurface
- Collider 기준 재현 가능한 편집기 베이크 도구
- EnemyDefinition → Agent 속성 어댑터
- 실제 위치 변화가 포함된 PlayMode 검증

## 수정·기각

- 씬 열기 전 정의 에셋 로드는 fake-null 위험으로 기각하고 씬을 먼저 여는 순서로 수정
- Agent 정지 후 ResetPath 순서는 정지 해제로 실패해 ResetPath 후 정지로 수정
- 런타임마다 NavMesh를 재베이크하는 방식은 현재 정적 샌드박스 범위에서 기각

## 검증 결과

- EditMode 53/53, PlayMode 16/16 통과
- NavMesh PathComplete, 실제 이동, 정지 상태 통과

## 다음

OpenSpec 4.4에서 플레이어 탐지·추적·공격 거리 정지와 공격 쿨다운을 연결한다.

## 연결

- PRD: [[../01_PRD]]
- 내비게이션 계약: [[../22_ENEMY_NAVIGATION]]
- 개발일지: [[../DevLog/2026-07-11_M3-enemy-navigation]]
