---
title: NavMeshAgent ResetPath 뒤 isStopped가 해제되는 문제
date: 2026-07-11
status: resolved
tags:
  - troubleshooting
  - navmesh-agent
  - reset-path
  - playmode-test
---

# NavMeshAgent ResetPath 뒤 isStopped가 해제되는 문제

## 증상

MeleeGrunt가 경로 이동과 위치 변화에는 성공했지만 `EnemyNavigationController.Stop()` 호출 직후 `NavMeshAgent.isStopped`가 false여서 PlayMode가 실패했다.

## 발생 조건

```csharp
agent.isStopped = true;
agent.ResetPath();
```

정지 플래그를 먼저 설정한 뒤 경로를 초기화하면 최종 정지 상태가 기대와 달라졌다.

## 최소 재현

1. 베이크된 NavMesh 위 Agent에 `SetDestination`을 호출한다.
2. 실제 이동을 한 프레임 이상 진행한다.
3. 위 순서로 정지와 경로 초기화를 호출한다.
4. `isStopped`가 false인지 확인한다.

## 해결

경로를 먼저 초기화하고 정의의 기본 정지거리를 복원한 뒤 마지막에 정지 플래그를 설정한다.

```csharp
agent.ResetPath();
agent.stoppingDistance = definition.NavigationStoppingDistance;
agent.isStopped = true;
```

## 회귀 검증

- PathComplete 이동 성공
- 실제 위치 0.25m 초과 변화
- Stop 이후 경로 제거
- Stop 이후 `isStopped=true`
- 슬롯·Return에서 바뀐 정지거리도 기본값으로 복원
- EditMode 54/54, PlayMode 20/20 통과

## 재발 방지

Unity 컴포넌트의 상태 변경 메서드는 플래그를 내부적으로 바꿀 수 있으므로 최종 불변조건을 마지막에 명시하고 PlayMode에서 관찰 가능한 상태를 검증한다.

## 연결

- NavMesh 계약: [[../22_ENEMY_NAVIGATION]]
- M3 검증: [[../27_M3_ENEMY_AI_VALIDATION]]
- 개발일지: [[../DevLog/2026-07-11_M3-validation-matrix]]
