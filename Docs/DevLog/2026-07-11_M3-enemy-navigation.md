---
title: M3 개발일지 — NavMesh 베이크와 Agent 이동
date: 2026-07-11
milestone: M3
requirements:
  - FR-AI-002
  - FR-AI-003
status: complete
tags:
  - devlog
  - navmesh
---

# M3 개발일지 — NavMesh 베이크와 Agent 이동

## 목표

OpenSpec 4.3에 따라 CombatSandbox에 재현 가능한 NavMesh 베이크와 MeleeGrunt Agent 이동 기반을 구성한다.

## 시작 상태

- 기준 커밋: `33ab1ff` — Implement enemy state transitions
- OpenSpec: 23/64
- AI Navigation 2.0.11 설치 상태
- 순수 상태 머신은 있으나 씬 NavMesh 데이터와 Agent 없음

## 수행 내용

1. Editor 어셈블리에 `Unity.AI.Navigation` 참조를 추가했다.
2. `NavMeshSurface`를 Collider 기준으로 베이크하고 데이터 에셋을 저장했다.
3. MeleeGrunt에 ActorHealth, Reaction, NavMeshAgent와 NavigationController를 구성했다.
4. 이동속도·정지거리·반경을 EnemyDefinition에서 Agent로 적용했다.
5. PlayMode에서 완전 경로, 실제 이동과 정지를 검증했다.

## 검증 결과

| 항목 | 결과 |
|---|---|
| NavMeshData 에셋 | 생성·씬 참조 유효 |
| 시작·목적지 샘플 | 성공 |
| 경로 상태 | PathComplete |
| 0.35초 이동 | 0.25m 초과 |
| 정지 | 경로 제거·isStopped true |
| EditMode 전체 | 53/53 passed |
| PlayMode 전체 | 16/16 passed |

## 실패와 수정

- 씬을 연 뒤 이전에 로드한 EnemyDefinition이 fake-null이 되어 Agent Speed가 0으로 저장되는 문제를 발견했다.
- 씬을 먼저 열고 정의 에셋을 다시 로드하는 순서로 수정했다.
- `isStopped=true` 뒤 `ResetPath()`를 호출하면 정지 플래그가 해제되어, `ResetPath()` 후 정지하도록 순서를 변경했다.

## 연결

- 내비게이션 계약: [[../22_ENEMY_NAVIGATION]]
- 문제 해결: [[../Troubleshooting/2026-07-11-editor-scene-open-asset-reference]]
- 프롬프트: [[../PromptLog/2026-07-11_M3_enemy_navigation_v01]]
