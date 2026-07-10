---
title: Unity 씬의 GameplayInputGate가 발견되지 않는 GUID 불일치
date: 2026-07-11
status: resolved
tags:
  - troubleshooting
  - unity
  - meta-guid
  - serialization
---

# Unity 씬의 GameplayInputGate가 발견되지 않는 GUID 불일치

## 증상

- EditMode 27개는 모두 통과한다.
- PlayMode에서 `Object.FindFirstObjectByType<GameplayInputGate>()`가 null을 반환한다.
- 임시 재생성 씬에서는 같은 테스트가 통과하지만 원본과 동일한 최소 변경 씬에서는 실패한다.

## 발생 조건

1. 새 스크립트를 원본과 임시 Unity 프로젝트에서 각각 임포트한다.
2. 임시 프로젝트가 만든 씬 YAML 블록을 원본 씬으로 복사한다.
3. 씬의 `m_Script.guid`는 임시 `.meta` GUID인데 원본 `.meta`는 다른 GUID를 가진다.

## 실패한 시도

- 이전 임시 프로젝트의 PlayMode 9/9 통과 결과만 근거로 완료하려 했다.
- 임시 씬의 `Game Session` 블록을 GUID 확인 없이 원본에 삽입했다.

## 근본 원인

Unity는 스크립트 참조를 파일명이나 클래스명으로 찾지 않고 `.meta`의 GUID로 직렬화한다. 임시 프로젝트가 독립적으로 만든 `.meta`와 원본 `.meta`의 GUID가 달라 원본 씬에서는 Missing Script가 되었다.

## 해결

1. 원본 `GameplayInputGate.cs.meta`의 GUID를 확인한다.
2. `CombatSandbox.unity`의 `m_Script.guid`를 원본 GUID로 교정한다.
3. `GameplayInputGateSandboxTools.ConfigureForBatch`를 임시 복사본에서 실행한다.
4. 같은 Game Session에 컴포넌트가 중복 추가되지 않고 검증되는지 확인한다.

## 회귀 테스트

- 원본과 동일한 임시 복사본의 EditMode: 27/27 통과
- PlayMode에서 GameplayInputGate 검색: 통과
- Pause·Victory·Defeat 시나리오: 모두 통과
- 전체 PlayMode: 9/9 통과

## 예방 규칙

- 새 Unity 자산을 옮길 때 파일과 `.meta`를 항상 함께 이동한다.
- 임시 프로젝트가 새로 생성한 `.meta` GUID를 원본 씬에 복사하지 않는다.
- 씬 YAML을 수동 편집한 뒤에는 클래스명 문자열이 아니라 실제 Unity 로드와 PlayMode 검색으로 참조를 검증한다.
