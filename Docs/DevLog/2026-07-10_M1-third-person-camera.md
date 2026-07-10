---
title: M1 개발일지 — Cinemachine 3인칭 카메라
date: 2026-07-10
milestone: M1
requirements:
  - FR-PLAYER-002
  - FR-CAMERA-001
  - NFR-MAINT-001
  - NFR-MAINT-002
  - NFR-DOC-001
status: complete
tags:
  - devlog
  - cinemachine
  - camera
  - player-control
---

# M1 개발일지 — Cinemachine 3인칭 카메라

## 목표

OpenSpec 작업 2.3에 따라 플레이어를 추적하는 Cinemachine 3인칭 카메라를 구성하고, 마우스 감도와 수직 각도 제한을 코드·씬·자동 테스트로 고정한다.

## 시작 상태

- 기준 커밋: `ab87b8b` — Implement camera-relative player movement
- Unity 6000.3.11f1, URP 17.3.0, Cinemachine 3.1.5
- Gameplay/Look는 `<Pointer>/delta`에 연결됨
- CombatSandbox는 고정 사선 Main Camera를 사용함
- 플레이어 이동은 Main Camera의 수평 전방을 기준으로 작동함
- 실제 Unity 프로젝트는 열려 있어 lock 파일이 존재함

## 수행 내용

1. 설치된 Cinemachine 3.1.5 소스에서 `CinemachineCamera`, `CinemachineOrbitalFollow`, `CinemachineRotationComposer`, `InputAxis` API를 확인했다.
2. 마우스 델타·감도·상하 제한을 계산하는 `CameraOrbitMath`를 추가했다.
3. Gameplay/Look를 읽어 Orbital Follow 축에 전달하는 `ThirdPersonCameraController`를 추가했다.
4. Main Camera에 Brain을, Player에 `Camera Target`을, 씬에 가상 카메라 파이프라인을 구성했다.
5. 카메라 구성을 반복 생성·검증하는 Editor 도구를 추가했다.
6. Player 재생성 도구가 카메라 참조까지 다시 구성하도록 연결했다.
7. 계산 EditMode 테스트 6개와 실제 마우스·추적 PlayMode 테스트 2개를 추가했다.
8. 열린 실제 프로젝트를 닫지 않고 임시 복사본에서 씬 생성과 전체 테스트를 수행했다.

## 변경된 자산

| 구분 | 자산 |
|---|---|
| 계산 | `CameraOrbitMath.cs` |
| Unity 어댑터 | `ThirdPersonCameraController.cs` |
| 씬 생성 도구 | `ThirdPersonCameraSandboxTools.cs` |
| 기존 도구 통합 | `PlayerMovementSandboxTools.cs` |
| 씬 | `CombatSandbox.unity` |
| EditMode | `CameraOrbitMathTests.cs` |
| PlayMode | `ThirdPersonCameraPlayModeTests.cs` |
| 설명 | [[../09_THIRD_PERSON_CAMERA]] |

## 검증 결과

| 항목 | 기대 결과 | 실제 결과 | 판정 |
|---|---|---|---|
| 감도 | X=100, 감도 0.12 → 12° | 12° | 통과 |
| 수직 상한 | 큰 위 입력이 65°에서 정지 | 65° | 통과 |
| 수직 하한 | 큰 아래 입력이 -20°에서 정지 | -20° | 통과 |
| 수평 순환 | 175° + 10° → -175° | -175° | 통과 |
| 대상 참조 | Follow·LookAt이 Camera Target | 일치 | 통과 |
| 실제 추적 | Player 이동 후 Main Camera 이동 | 0.261m/0.3초 | 통과 |
| 전체 EditMode | 기존 회귀 포함 | 19/19 passed | 통과 |
| 전체 PlayMode | 기존 회귀 포함 | 4/4 passed | 통과 |
| 컴파일·런타임 | 새 오류·예외 0개 | 0개 | 통과 |

Cinemachine 패키지 내부 HDRP 샘플 asmref 경고는 기존 패키지 경고이며 이번 변경으로 새 프로젝트 경고는 추가되지 않았다.

## 실패와 수정

### 감쇠 카메라의 짧은 시간 이동량을 과대 가정

- 첫 추적 테스트는 Player를 3m 이동한 뒤 0.3초 내 Main Camera가 0.5m보다 많이 움직일 것으로 가정했다.
- `CinemachineOrbitalFollow`의 기본 위치 감쇠 때문에 실제 이동은 0.2609m였고, 구성·입력 테스트는 모두 통과했지만 이 테스트만 실패했다.
- 요구사항은 “즉시 0.5m 이동”이 아니라 “플레이어를 추적”하는 것이므로, 추적 발생을 판별할 수 있는 0.1m로 임계값을 수정했다.
- 감쇠 속도 자체를 테스트 요구사항에 넣으려면 먼저 제품 명세에 응답시간이나 허용 오차를 정의해야 한다.

## 사람의 판단

- 채택: Cinemachine의 입력 Controller를 추가로 사용하지 않고 기존 Gameplay/Look 계약을 직접 소비
- 채택: 마우스 델타를 픽셀당 각도로 해석하고 `deltaTime`을 다시 곱하지 않음
- 채택: 카메라 축 계산과 MonoBehaviour 어댑터 분리
- 채택: 수평 축은 순환하고 수직 축만 제한
- 채택: 실제 캐릭터 루트 대신 높이 1.4m의 Camera Target 추적
- 보류: 공유 Gameplay 맵의 중앙 생명주기 관리 — 작업 2.5
- 기각: 작업 2.3에 Deoccluder와 장애물 충돌까지 포함하는 범위 확대 — 작업 2.4

## 기능 완료 체크

- [x] FR-PLAYER-002와 OpenSpec 작업 2.3을 식별했다.
- [x] 감도와 수직 제한을 Inspector 조정값으로 제공했다.
- [x] 마우스 입력을 실제 Cinemachine 궤도 축에 연결했다.
- [x] Main Camera 출력과 플레이어 추적을 PlayMode에서 검증했다.
- [x] 반복 실행 가능한 씬 구성 도구를 제공했다.
- [x] 실패한 테스트 가정과 수정 근거를 기록했다.
- [ ] 장애물 가림 방지 — 작업 2.4
- [ ] 상태별 입력 맵 전환 — 작업 2.5
- [ ] macOS·Windows 빌드 — M1 통합 완료 시 검증

## 교육 포인트

- AI에게 “카메라를 만들어 달라”고만 요청하기보다 입력 의미, 감도 단위, 허용 각도와 제외 범위를 명시해야 결과를 검증할 수 있다.
- Cinemachine은 실제 카메라 출력, 가상 카메라 상태, 위치 계산, 회전 구성을 서로 다른 컴포넌트로 나눈다.
- 마우스 델타와 속도 입력은 시간 의미가 다르므로 모든 Update 계산에 기계적으로 `deltaTime`을 곱하면 안 된다.
- 테스트 임계값은 구현의 우연한 수치가 아니라 명세에서 관찰 가능한 요구사항에 근거해야 한다.
- 씬을 자동 구성하는 도구는 기능 구현뿐 아니라 실습 재현성과 회귀 복구 수단이다.

## 연결

- PRD: [[../01_PRD]]
- 카메라 계약: [[../09_THIRD_PERSON_CAMERA]]
- 이동 계약: [[../08_PLAYER_MOVEMENT]]
- 프롬프트: [[../PromptLog/2026-07-10_M1_third_person_camera_v01]]
- OpenSpec tasks: [tasks.md](../../openspec/changes/build-action-rpg-vertical-slice/tasks.md)
