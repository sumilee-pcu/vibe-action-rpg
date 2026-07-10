---
title: M1 개발일지 — 카메라 장애물 가림 처리
date: 2026-07-10
milestone: M1
requirements:
  - FR-CAMERA-001
  - NFR-MAINT-001
  - NFR-MAINT-002
  - NFR-DOC-001
status: complete
tags:
  - devlog
  - cinemachine
  - camera
  - occlusion
---

# M1 개발일지 — 카메라 장애물 가림 처리

## 목표

OpenSpec 작업 2.4에 따라 플레이어와 카메라 사이의 일반적인 월드 장애물을 감지하고, 카메라가 장애물 앞쪽으로 이동해 시야를 확보한 뒤 장애물이 사라지면 원래 궤도로 복귀하게 한다.

## 시작 상태

- 기준 커밋: `79536b1` — Implement Cinemachine third-person camera
- Main Camera와 Cinemachine 3인칭 궤도 회전 완료
- Camera Target, 감도와 수직 제한 완료
- 벽이나 기둥이 카메라와 플레이어 사이에 들어오면 화면이 가려짐
- OpenSpec 진행률 9/64

## 수행 내용

1. 설치된 Cinemachine 3.1.5의 Deoccluder 소스와 로컬 패키지 문서를 확인했다.
2. Third Person Camera에 `CinemachineDeoccluder`를 추가했다.
3. Default 레이어 검사, Player 태그 제외, Camera Radius 0.25m를 설정했다.
4. 예측 가능한 `PullCameraForward` 전략을 선택했다.
5. 정면 벽, 측면 기둥과 대각선 블록을 CombatSandbox에 추가했다.
6. Editor 도구가 Deoccluder와 대표 가림 장면을 반복 생성·검증하도록 확장했다.
7. 정면·측면·복귀 PlayMode 시나리오 세 개를 추가했다.
8. 복귀 감쇠와 테스트 인수 기준을 실제 측정값으로 조정했다.

## 변경된 자산

| 구분 | 자산 |
|---|---|
| 씬 | `CombatSandbox.unity` |
| 씬 생성·검증 | `ThirdPersonCameraSandboxTools.cs` |
| 자동 검증 | `ThirdPersonCameraPlayModeTests.cs` |
| 기능 계약 | [[../10_CAMERA_OCCLUSION]] |

새 런타임 스크립트는 추가하지 않았다. Cinemachine 3.1.5가 제공하는 검증된 확장 기능을 현재 요구사항에 맞게 구성하는 것으로 충분했기 때문이다.

## 검증 결과

| 항목 | 기대 결과 | 실제 결과 | 판정 |
|---|---|---|---|
| 정면 벽 감지 | Deoccluder 보정 발생 | CameraWasDisplaced=true | 통과 |
| 정면 거리 | 6m보다 1m 이상 짧음 | 약 2.65m | 통과 |
| 정면 시야 | Target-Camera 사이에 벽 없음 | Direct Wall hit 0 | 통과 |
| 측면 기둥 | 수평 90°에서 보정 발생 | 보정·거리 단축 확인 | 통과 |
| 장애물 제거 | 거리 증가 | 1m 이상 증가 | 통과 |
| 보정 해제 | Deoccluder 보정량 감소 | 1m 이상 감소 | 통과 |
| 전체 EditMode | 기존 회귀 포함 | 19/19 passed | 통과 |
| 전체 PlayMode | 기존 회귀 포함 | 7/7 passed | 통과 |
| 새 컴파일·런타임 오류 | 0개 | 0개 | 통과 |

Cinemachine 패키지 내부 HDRP 샘플 asmref 경고는 기존 패키지 경고이며 이번 기능의 오류가 아니다.

## 실패와 수정

### 복귀 감쇠가 지나치게 느림

- 첫 설정은 Return Damping 0.5, Smoothing Time 0.05였다.
- 장애물을 끈 뒤 1.2초 동안 Camera Target 거리는 약 2.65m에서 2.90m로만 증가했다.
- Smoothing을 0으로, Return Damping을 0.2로 낮추자 약 3.28m까지 회복했다.
- Return Damping을 0.05로 낮추자 같은 시간에 약 5.11m까지 회복해 전투 카메라에 적절한 반응성을 확보했다.

### 고정 시간의 절대거리 테스트가 불안정함

- 초기 테스트는 장애물 제거 후 1.2초 또는 1.6초에 거리가 반드시 5.4m를 넘어야 한다고 가정했다.
- 배치 모드의 프레임 진행과 감쇠 중간 상태에 따라 4.90~5.11m로 달라져 같은 기능에서도 결과가 흔들렸다.
- OpenSpec은 정확한 복귀 시간을 요구하지 않으므로 절대 위치 대신 “거리 1m 이상 증가”와 “보정량 1m 이상 감소”를 검증했다.
- 응답시간을 품질 기준으로 관리하려면 먼저 PRD나 OpenSpec에 시간과 허용 오차를 명시해야 한다.

## 사람의 판단

- 채택: CinemachineDeoccluder의 Pull Camera Forward 전략
- 채택: Default 월드 레이어만 검사하고 Player 태그 제외
- 채택: 실제 씬 오브젝트로 정면·측면·대각선 가림을 재현
- 채택: 절대 중간 위치보다 거리·보정량의 상대 변화 검증
- 수정: Return Damping 0.5 → 0.05, Smoothing Time 0.05 → 0
- 기각: 현재 요구사항에 필요하지 않은 Decollider 중복 추가
- 보류: 환경 전용 CameraObstacle 레이어 — 실제 맵 에셋 통합 시 적용

## 기능 완료 체크

- [x] FR-CAMERA-001과 OpenSpec 작업 2.4를 식별했다.
- [x] 장애물 감지 레이어와 무시 태그를 명시했다.
- [x] 정면 벽과 측면 기둥을 자동 검증했다.
- [x] 장애물 제거 후 원래 궤도 방향으로 복귀함을 검증했다.
- [x] 씬 자동 구성 도구에 대표 가림 사례를 포함했다.
- [x] 실패한 감쇠값과 테스트 가정을 기록했다.
- [ ] 실제 맵 에셋의 복잡한 코너 검증 — 환경 에셋 통합 시 수행
- [ ] macOS·Windows 빌드 — M1 통합 완료 시 수행

## 교육 포인트

- 패키지 기능을 사용하더라도 레이어, 태그, 반지름과 감쇠는 제품 요구사항에 맞게 명시해야 한다.
- 장애물 처리는 “컴포넌트가 존재한다”가 아니라 실제 벽과 기둥에서 시야가 확보되는지 검증해야 한다.
- 감쇠 테스트는 프레임 중간의 절대 위치보다 시작 상태 대비 변화와 최종 방향을 먼저 검증하는 편이 안정적이다.
- 테스트 실패가 항상 구현 오류는 아니다. 명세에 없는 수치 가정을 테스트가 추가했는지 검토해야 한다.
- 대표 씬을 코드로 재생성할 수 있으면 학생마다 다른 수동 배치 때문에 생기는 실습 편차를 줄일 수 있다.

## 연결

- PRD: [[../01_PRD]]
- 카메라 가림 계약: [[../10_CAMERA_OCCLUSION]]
- 3인칭 카메라: [[../09_THIRD_PERSON_CAMERA]]
- 프롬프트: [[../PromptLog/2026-07-10_M1_camera_occlusion_v01]]
- OpenSpec tasks: [tasks.md](../../openspec/changes/build-action-rpg-vertical-slice/tasks.md)
