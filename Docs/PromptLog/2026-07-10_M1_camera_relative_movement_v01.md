---
title: 프롬프트 기록 — M1 카메라 기준 이동 v01
date: 2026-07-10
milestone: M1
requirements:
  - FR-PLAYER-001
  - FR-PLAYER-003
  - NFR-MAINT-001
  - NFR-MAINT-002
status: complete
tags:
  - prompt-log
  - player-control
  - movement
---

# 프롬프트 기록 — M1 카메라 기준 이동 v01

## 목표

- OpenSpec의 다음 미완료 작업 2.2를 구현한다.
- 카메라 기준 수평 이동과 이동 방향 회전을 제공한다.
- 30fps와 120fps에서 같은 시간 동안 이동 거리가 같음을 검증한다.
- CombatSandbox에서 실제 W 입력을 자동 재현한다.

## 사용자 원문 요청

~~~text
다음 진행
~~~

## AI가 해석한 실행 범위

~~~text
build-action-rpg-vertical-slice의 작업 2.2를 적용한다.
이동 수학과 Unity 어댑터를 분리하고, CharacterController 플레이어를
CombatSandbox에 배치한다. 카메라 추적과 회전은 작업 2.3으로 남긴다.
EditMode에서 프레임 독립성을, PlayMode에서 실제 W 입력과 방향 회전을 검증한다.
~~~

## 실행 환경

- Unity: 6000.3.11f1
- Input System: 1.19.0
- Render Pipeline: URP 17.3.0
- 입력 에셋: 'TinyVanguardInput'
- 기준 커밋: '67b9a25'
- OpenSpec change: 'build-action-rpg-vertical-slice'

## AI에게 적용한 제약

- 카메라 pitch가 수직 이동을 만들지 않아야 한다.
- 대각선 입력으로 이동 속도가 증가하지 않아야 한다.
- 속도와 회전량은 deltaTime을 사용해야 한다.
- 매 프레임 Camera.main, Find 또는 GetComponent를 호출하지 않는다.
- Cinemachine 구현은 작업 2.3까지 확장하지 않는다.
- 열린 실제 Unity 프로젝트의 lock 파일을 제거하지 않는다.
- 씬 구성은 Editor 도구로 반복 생성할 수 있어야 한다.
- 실패한 테스트 설계와 수정 판단을 교육 기록에 포함한다.

## AI 결과 요약

- 순수 이동 계산 API 3개
- CharacterController 기반 PlayerMovementController
- 반복 실행 가능한 CombatSandbox 구성 도구
- Ground, Player, Visual, Facing Marker 프로토타입
- 이동 계산 EditMode 테스트 6개
- 실제 W 입력 PlayMode 테스트 1개
- 이동 계약, 개발일지와 테스트 오류 기록

## 사람의 검토

### 채택

- 계산과 Unity 생명주기 어댑터를 분리한 구조
- 카메라 전방의 수평 투영과 입력 크기 제한
- 초당 이동·회전 수치에 deltaTime을 적용하는 방식
- 실제 열린 프로젝트 대신 임시 복사본에서 씬 생성·테스트
- 캡슐의 앞 방향을 보여주는 Facing Marker

### 수정

- 고정 20프레임 검증을 0.25초 경과시간 검증으로 수정했다.
- PlayerInput 기반 초기 구현을 InputActionAsset 직접 소비 방식으로 단순화했다.
- Press 직후 입력 상태 검사를 다음 Player Loop 이후 검사로 수정했다.
- 테스트 장치를 직접 QueueStateEvent로 제어하려던 시도를 InputTestFixture의 Press 방식으로 되돌렸다.

### 기각

- 작업 2.2에서 Cinemachine 카메라까지 한 번에 구현하는 접근
- Update에서 Camera.main이나 오브젝트 검색을 반복하는 접근
- Rigidbody와 CharacterController를 동시에 사용하는 접근
- 현재 로컬 싱글플레이에 필요하지 않은 PlayerInput 사용자 페어링

## 검증 결과

- 이동 수학 EditMode: 6/6 통과
- 전체 EditMode: 13/13 통과
- 실제 W 입력 PlayMode: 통과
- 전체 PlayMode: 2/2 통과
- 30fps, 120fps의 2초 이동: 모두 12m
- 새 컴파일 오류와 런타임 예외: 0개

## 다음 프롬프트에서 개선할 점

- UnityTest 입력은 다음 Player Loop에 적용된다는 조건을 처음부터 명시한다.
- 시간 기반 동작의 PlayMode 검증은 프레임 수보다 경과시간 또는 순수 계산 테스트를 사용한다.
- 작업 2.3은 Cinemachine 3.1.5 API와 URP 샌드박스 씬의 카메라 요구사항만 다룬다.
- 카메라 감도와 수직 제한은 Inspector 조정값과 테스트 가능한 계산으로 분리한다.

## 연결

- PRD: [[01_PRD]]
- 이동 계약: [[08_PLAYER_MOVEMENT]]
- 개발일지: [[../DevLog/2026-07-10_M1-camera-relative-movement]]
- 오류 기록: [[../Troubleshooting/2026-07-10-input-system-playmode-test-lifecycle]]
- OpenSpec tasks: [tasks.md](../../openspec/changes/build-action-rpg-vertical-slice/tasks.md)
