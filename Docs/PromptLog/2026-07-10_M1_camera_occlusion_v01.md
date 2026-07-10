---
title: 프롬프트 기록 — M1 카메라 장애물 가림 처리 v01
date: 2026-07-10
milestone: M1
requirements:
  - FR-CAMERA-001
  - NFR-MAINT-001
  - NFR-MAINT-002
status: complete
tags:
  - prompt-log
  - cinemachine
  - camera
  - occlusion
---

# 프롬프트 기록 — M1 카메라 장애물 가림 처리 v01

## 목표

- OpenSpec의 다음 미완료 작업 2.4를 구현한다.
- 일반적인 벽과 기둥이 플레이어를 가리는 현상을 줄인다.
- 정면·측면·대각선 가림을 반복 재현 가능한 씬으로 만든다.
- 장애물 진입과 해제 후 복귀를 실제 Main Camera 출력에서 검증한다.

## 사용자 원문 요청

```text
계속
```

## AI가 해석한 실행 범위

```text
build-action-rpg-vertical-slice의 작업 2.4를 적용한다.
Cinemachine 3.1.5의 장애물 처리 API를 확인한 뒤 Third Person Camera에
Deoccluder를 구성한다. CombatSandbox에 정면 벽, 측면 기둥, 대각선 블록을
추가하고 시야 확보와 장애물 제거 후 복귀를 PlayMode에서 검증한다.
입력 상태 관리와 회피는 각각 작업 2.5와 2.6으로 남긴다.
```

## 실행 환경

- Unity: 6000.3.11f1
- Cinemachine: 3.1.5
- Render Pipeline: URP 17.3.0
- 기준 커밋: `79536b1`
- OpenSpec change: `build-action-rpg-vertical-slice`

## AI에게 적용한 제약

- 설치된 Cinemachine 패키지의 로컬 소스와 문서를 기준으로 API를 선택한다.
- 현재 요구사항과 직접 관련된 Deoccluder만 사용한다.
- 장애물에는 실제 Collider가 있어야 한다.
- Player를 장애물로 오인하지 않도록 Ignore Tag를 설정한다.
- 대표 가림 오브젝트는 Editor 도구로 반복 생성할 수 있어야 한다.
- 실제 Main Camera 위치와 Deoccluder 보정량을 PlayMode에서 검증한다.
- 명세에 없는 고정 응답시간을 임의의 완료 조건으로 만들지 않는다.
- 열린 원본 Unity 프로젝트를 강제 종료하지 않는다.

## AI 결과 요약

- Third Person Camera의 CinemachineDeoccluder 구성
- Default 레이어와 Player Ignore Tag 계약
- 0.25m 카메라 반지름과 Pull Camera Forward 전략
- 정면 벽, 측면 기둥과 대각선 블록 대표 장면
- 정면 시야, 측면 시야와 복귀 PlayMode 테스트 3개
- 가림 처리 계약, 개발일지와 프롬프트 기록

## 사람의 검토

### 채택

- CinemachineDeoccluder를 기존 가상 카메라 파이프라인에 추가
- 넓은 벽과 좁은 기둥을 서로 다른 자동 시나리오로 검증
- Camera Target에서 실제 Main Camera까지의 선분 검사
- 장애물 제거 전후 거리와 보정량의 상대 변화 검증
- 임시 Unity 복사본에서 씬 생성과 회귀 테스트 실행

### 수정

- Return Damping을 0.5에서 0.05로 낮췄다.
- Smoothing Time을 0.05에서 0으로 낮췄다.
- 고정 시간 후 5.4m를 요구한 절대거리 테스트를 상대 변화 테스트로 수정했다.
- 복귀 검증에 Deoccluder의 `GetCameraDisplacementDistance`를 추가했다.

### 기각

- Deoccluder와 Decollider를 이유 없이 동시에 추가하는 접근
- Collider 없이 화면 렌더링 결과만으로 가림을 추정하는 접근
- 모든 Unity 레이어를 장애물로 검사하는 접근
- 테스트를 통과시키기 위해 기능 변화가 거의 없는 낮은 임계값만 사용하는 접근

## 검증 결과

- 전체 EditMode: 19/19 통과
- 기존 카메라 PlayMode 회귀: 통과
- 정면 벽 시야 확보: 통과
- 측면 기둥 90° 시야 확보: 통과
- 장애물 제거 후 거리 증가·보정 감소: 통과
- 전체 PlayMode: 7/7 통과
- 새 컴파일 오류와 런타임 예외: 0개

## 다음 프롬프트에서 개선할 점

- 작업 2.5는 Gameplay와 UI 맵의 활성 상태를 하나의 소유자가 관리하도록 설계한다.
- 상태 전환 전후 입력 활성 여부를 표로 먼저 정의한다.
- 실제 환경 에셋을 도입할 때 CameraObstacle 전용 레이어와 투명 레이어를 검토한다.
- 카메라 복귀 시간을 제품 품질 지표로 사용할 경우 수치 인수 기준부터 추가한다.

## 연결

- PRD: [[../01_PRD]]
- 카메라 가림 계약: [[../10_CAMERA_OCCLUSION]]
- 3인칭 카메라: [[../09_THIRD_PERSON_CAMERA]]
- 개발일지: [[../DevLog/2026-07-10_M1-camera-occlusion]]
- OpenSpec tasks: [tasks.md](../../openspec/changes/build-action-rpg-vertical-slice/tasks.md)
