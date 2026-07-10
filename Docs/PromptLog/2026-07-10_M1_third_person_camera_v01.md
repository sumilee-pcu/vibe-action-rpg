---
title: 프롬프트 기록 — M1 Cinemachine 3인칭 카메라 v01
date: 2026-07-10
milestone: M1
requirements:
  - FR-PLAYER-002
  - FR-CAMERA-001
  - NFR-MAINT-001
  - NFR-MAINT-002
status: complete
tags:
  - prompt-log
  - cinemachine
  - camera
  - player-control
---

# 프롬프트 기록 — M1 Cinemachine 3인칭 카메라 v01

## 목표

- OpenSpec의 다음 미완료 작업 2.3을 구현한다.
- Cinemachine 3.1.5로 플레이어 추적과 마우스 궤도 회전을 제공한다.
- 수평·수직 감도와 수직 제한을 명시적인 조정값으로 둔다.
- 실제 씬의 마우스 입력, 축 제한과 추적을 자동 검증한다.

## 사용자 원문 요청

```text
계속
```

## AI가 해석한 실행 범위

```text
build-action-rpg-vertical-slice의 작업 2.3을 적용한다.
Gameplay/Look를 Cinemachine Orbital Follow에 연결하고, Main Camera Brain,
Player Camera Target, 가상 카메라와 Rotation Composer를 CombatSandbox에 구성한다.
감도와 상하 제한은 테스트 가능한 계산과 Inspector 설정으로 분리한다.
장애물 가림 방지는 작업 2.4로 남긴다.
```

## 실행 환경

- Unity: 6000.3.11f1
- Cinemachine: 3.1.5
- Input System: 1.19.0
- Render Pipeline: URP 17.3.0
- 입력 에셋: `TinyVanguardInput`
- 기준 커밋: `ab87b8b`
- OpenSpec change: `build-action-rpg-vertical-slice`

## AI에게 적용한 제약

- 설치된 Cinemachine 패키지의 실제 API를 먼저 확인한다.
- `Gameplay/Look` 계약을 재사용하고 별도의 중복 입력 에셋을 만들지 않는다.
- 마우스 델타에 `deltaTime`을 중복 적용하지 않는다.
- 수평 축은 순환하고 수직 축은 명시적 범위에서 제한한다.
- 카메라 계산과 Unity 생명주기 코드를 분리한다.
- Main Camera와 가상 카메라의 책임을 분리한다.
- 열린 실제 Unity 프로젝트를 강제 종료하지 않는다.
- 장애물 회피는 작업 2.4까지 확장하지 않는다.
- 씬 구성, 테스트 결과와 실패한 가정을 교육 기록에 포함한다.

## AI 결과 요약

- 순수 궤도 계산 API 1개
- Gameplay/Look 기반 `ThirdPersonCameraController`
- Cinemachine Brain·Camera·Orbital Follow·Rotation Composer 구성
- Player의 `Camera Target` 기준점
- 반복 실행 가능한 카메라 샌드박스 구성 도구
- 카메라 계산 EditMode 테스트 6개
- 실제 마우스 회전·플레이어 추적 PlayMode 테스트 2개
- 카메라 계약, 개발일지와 프롬프트 기록

## 사람의 검토

### 채택

- 기존 InputActionAsset을 직접 소비하는 현재 싱글플레이 구조
- `CameraOrbitMath`와 MonoBehaviour 어댑터의 책임 분리
- Sphere 궤도, 반지름 6m, 시작 수직 20°
- X/Y 감도를 각각 0.12°/pixel, 0.08°/pixel로 두는 기준선
- 수직 -20°~65° 제한과 수평 -180°~180° 순환
- 열린 프로젝트 대신 임시 복사본에서 씬 생성과 테스트 실행

### 수정

- 추적 테스트의 0.3초 이동량 임계값을 0.5m에서 0.1m로 수정했다.
- Player 재생성 후 Camera Target 참조가 끊기지 않도록 기존 샌드박스 도구가 카메라 구성도 다시 호출하게 했다.
- 커서 제어가 EditMode 자산 구성 중 실행되지 않도록 Play 상태 조건을 추가했다.

### 기각

- `CinemachineInputAxisController`를 별도로 추가해 입력 경로를 이중화하는 접근
- 마우스 델타에 `deltaTime`을 다시 곱하는 접근
- Main Camera를 Player 자식으로 만들어 직접 회전하는 접근
- 작업 2.3에서 Deoccluder·카메라 충돌까지 한 번에 구현하는 접근

## 검증 결과

- 카메라 계산 EditMode: 6/6 통과
- 전체 EditMode: 19/19 통과
- 실제 마우스 궤도와 수직 상한: 통과
- 실제 Main Camera 플레이어 추적: 통과
- 전체 PlayMode: 4/4 통과
- 새 컴파일 오류와 런타임 예외: 0개

## 다음 프롬프트에서 개선할 점

- 작업 2.4는 최소 카메라 거리, 충돌 레이어와 복귀 감쇠를 먼저 정의한다.
- 장애물 테스트 씬에는 플레이어와 카메라 사이를 가리는 벽을 재현 가능하게 배치한다.
- 카메라 반응시간을 자동 검증하려면 수치 인수 기준을 PRD나 OpenSpec에 먼저 추가한다.
- 작업 2.5에서 Gameplay 맵 활성화 책임을 하나의 상태 관리자에 모은다.

## 연결

- PRD: [[../01_PRD]]
- 카메라 계약: [[../09_THIRD_PERSON_CAMERA]]
- 이동 계약: [[../08_PLAYER_MOVEMENT]]
- 개발일지: [[../DevLog/2026-07-10_M1-third-person-camera]]
- OpenSpec tasks: [tasks.md](../../openspec/changes/build-action-rpg-vertical-slice/tasks.md)
