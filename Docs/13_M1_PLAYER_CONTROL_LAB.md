---
title: M1 플레이어 조작 통합 실습·검증
status: complete
updated: 2026-07-11
milestone: M1
requirements:
  - FR-PLAYER-001
  - FR-PLAYER-002
  - FR-PLAYER-003
  - FR-PLAYER-005
  - FR-CAMERA-001
  - FR-UI-005
tags:
  - curriculum
  - lab
  - player-control
---

# M1 플레이어 조작 통합 실습·검증

## 학습 목표

학습자는 Input Actions에서 실제 CharacterController 이동, Cinemachine 추적·가림, 상태별 입력 차단, 프레임 독립 회피까지의 데이터 흐름을 설명하고 검증할 수 있다.

## 기능·증거 추적표

| OpenSpec | PRD | 구현·계약 | 자동 검증 | 프롬프트·개발일지 |
|---|---|---|---|---|
| 2.1 | 입력 기반 | [[07_INPUT_ACTIONS]] | `InputActionsAssetTests` | M1 input-actions 기록 |
| 2.2 | FR-PLAYER-001,003 | [[08_PLAYER_MOVEMENT]] | `CameraRelativeMovementTests`, `PlayerMovementPlayModeTests` | M1 camera-relative-movement 기록 |
| 2.3 | FR-PLAYER-002 | [[09_THIRD_PERSON_CAMERA]] | `CameraOrbitMathTests`, 카메라 PlayMode | M1 third-person-camera 기록 |
| 2.4 | FR-CAMERA-001 | [[10_CAMERA_OCCLUSION]] | 정면·측면·복귀 PlayMode | M1 camera-occlusion 기록 |
| 2.5 | FR-UI-005 | [[11_GAMEPLAY_INPUT_GATING]] | 입력 정책·Pause·결과 상태 | [[PromptLog/2026-07-11_M1_gameplay_input_gating_v01]] |
| 2.6 | FR-PLAYER-005 | [[12_PLAYER_DODGE]] | 30/120fps·Space·Pause | [[PromptLog/2026-07-11_M1_player_dodge_v01]] |

## 통합 수동 시나리오

| 순서 | 조작 | 기대 결과 | 확인 개념 |
|---:|---|---|---|
| 1 | CombatSandbox Play | 콘솔 오류 없이 시작 | 씬·참조 무결성 |
| 2 | WASD | 카메라 기준 이동과 방향 회전 | 입력 벡터 변환 |
| 3 | 마우스 이동 | 수평 궤도와 -20°~65° 수직 제한 | 감도·각도 제한 |
| 4 | 벽·기둥 주변 이동 | 시야 확보 후 원래 궤도로 복귀 | Deoccluder |
| 5 | Escape | 이동·카메라 차단, UI 입력·커서 활성 | 단일 입력 게이트 |
| 6 | Escape 재입력 | Playing 복귀 | 상태 전이 |
| 7 | WASD+Space | 입력 방향으로 약 4m 회피 | 프레임 독립 이동 |
| 8 | Space 연타 | 활성 회피가 중첩되지 않음 | 중복 시작 거부 |
| 9 | 회피 중 Pause | 회피 즉시 취소 | 진행 중 행동 취소 정책 |

## 자동 검증 기준선

- EditMode: **32/32 passed**
- PlayMode: **11/11 passed**
- OpenSpec strict validation: 통과
- M1 최종 런타임 오류: 0개

## 대표 실패·수정 사례

1. Unity 테스트 명령에 `-quit`을 함께 사용해 결과 파일 생성 전에 종료됨.
2. PlayerInput 런타임 복사본과 액션 참조 수명주기가 경쟁해 Move 값이 0이 됨.
3. 카메라 복귀를 특정 시점의 절대거리로 검증해 감쇠 테스트가 흔들림.
4. 임시 프로젝트의 `.meta GUID`를 원본 씬에 사용해 Missing Script가 발생함.
5. CharacterController가 활성인 Player Transform을 테스트가 직접 이동해 카메라 추적 검증이 불안정함.

각 사례는 `Docs/Troubleshooting`과 해당 개발일지에 증상, 원인, 해결, 회귀 방지를 기록했다.

## 학습자 성찰 질문

1. 이동 Controller가 Gameplay 맵을 직접 활성화하면 Pause 설계가 왜 깨지는가?
2. 무적 종료 시점을 포함하지 않는 반개구간으로 정의한 이유는 무엇인가?
3. 30fps와 120fps에서 같은 회피 거리를 보장하려면 마지막 프레임을 어떻게 처리해야 하는가?
4. Cinemachine 추적 테스트에서 PlayerMovementController를 꺼야 했던 이유는 무엇인가?
5. 테스트 실패가 구현 오류인지 테스트 가정 오류인지 어떤 증거로 구분할 수 있는가?

## M1 완료 판정

- [x] 입력 계약과 기본 바인딩 기록
- [x] 카메라 기준 이동·방향 회전
- [x] 3인칭 카메라 회전·제한
- [x] 대표 장애물 가림·복귀
- [x] Pause·Victory·Defeat 입력 차단
- [x] 회피 이동·무적 상태 계약
- [x] 프롬프트·개발일지·실패 사례·수동 시나리오 연결

FR-PLAYER-004의 공격 중 이동 허용량은 공격 데이터가 생기는 M2에서 검증한다. AC-004의 실제 피해 무효화도 체력·피해 규칙과 연결되는 OpenSpec 3.1에서 최종 완료한다.

## 다음 단계

OpenSpec 3.1에서 순수 C# 체력, 피해, 사망 단일 발생 규칙을 구현하고 `PlayerMovementController.IsInvulnerable`을 피해 거부 조건에 연결한다.
