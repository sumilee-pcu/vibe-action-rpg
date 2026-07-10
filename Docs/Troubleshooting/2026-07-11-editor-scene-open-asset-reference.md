---
title: Unity Editor 씬 전환 뒤 ScriptableObject 참조가 fake-null이 되는 문제
date: 2026-07-11
status: resolved
tags:
  - troubleshooting
  - unity-editor
  - scriptable-object
  - navmesh
---

# Unity Editor 씬 전환 뒤 ScriptableObject 참조가 fake-null이 되는 문제

## 증상

NavMesh 샌드박스 자동 생성은 성공했지만 저장된 `NavMeshAgent`의 Speed가 정의값 3.5가 아닌 0이었다. 최초 검증도 0과 0을 비교해 잘못 통과할 수 있는 상태였다.

## 발생 조건

1. `AssetDatabase.LoadAssetAtPath`로 EnemyDefinition을 로드한다.
2. `EditorSceneManager.OpenScene`으로 다른 씬을 연다.
3. 씬 전환 중 사용되지 않는 에셋이 언로드된다.
4. 이전 로컬 변수는 C# 참조처럼 보이지만 Unity Object 비교에서는 fake-null이 된다.
5. `EnemyDefinition.MoveSpeed`의 null 안전 반환값 0이 Agent에 기록된다.

## 실패한 접근

- 런타임 `Configure()`를 한 번 더 호출하는 것만으로는 이미 무효화된 정의 참조를 복구하지 못했다.
- 객체끼리의 단순 동등성 검증은 두 참조가 모두 fake-null일 때 결함을 놓칠 수 있었다.

## 해결

씬을 먼저 연 뒤 해당 씬 구성에 필요한 EnemyDefinition을 다시 로드했다. 편집기 도구가 Agent의 직렬화 속도를 명시적으로 설정하고 저장된 YAML의 Speed 3.5, Stopping Distance 1.25를 확인했다.

## 회귀 검증

- NavMesh 데이터 에셋 생성
- Agent Speed 3.5, Stopping Distance 1.25 저장
- PathComplete
- 실제 이동 후 정지
- EditMode 53/53, PlayMode 16/16 통과

## 재발 방지

편집기 자동화에서 씬을 열거나 바꾸는 작업은 먼저 수행하고, 그 이후 필요한 에셋을 로드한다. Unity Object 참조 검증은 동등성뿐 아니라 핵심 직렬화 수치와 저장 결과도 함께 확인한다.

## 연결

- 내비게이션 계약: [[../22_ENEMY_NAVIGATION]]
- 개발일지: [[../DevLog/2026-07-11_M3-enemy-navigation]]
