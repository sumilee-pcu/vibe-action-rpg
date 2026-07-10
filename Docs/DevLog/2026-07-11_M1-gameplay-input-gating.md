---
title: M1 개발일지 — 게임플레이 입력 차단
date: 2026-07-11
milestone: M1
requirements:
  - FR-UI-005
  - FR-PROG-004
  - FR-PROG-005
  - NFR-MAINT-002
  - NFR-DOC-001
status: complete
tags:
  - devlog
  - input-system
  - session-state
---

# M1 개발일지 — 게임플레이 입력 차단

## 목표

OpenSpec 2.5에 따라 일시정지·승리·패배 상태에서 Gameplay 입력을 차단하고 UI와 Pause 입력만 유지한다. 입력 맵과 커서의 활성화 책임을 하나의 상태 게이트에 모은다.

## 시작 상태

- 기준 커밋: `2ba8b42` — Add camera obstacle handling
- OpenSpec 진행률: 10/64
- 이동 Controller와 카메라 Controller가 Gameplay 맵을 각각 활성화함
- 중단된 작업 트리에 입력 게이트 초안과 테스트가 남아 있었음
- 원본 `CombatSandbox`에는 Game Session이 아직 유효하게 직렬화되지 않음

## 수행 내용

1. Playing, Paused, Victory, Defeat 상태를 `GameSessionState`로 정의했다.
2. 상태별 입력 맵 조합과 Pause 전이 규칙을 순수 `GameplayInputPolicy`로 분리했다.
3. `GameplayInputGate`가 Gameplay, System, UI 맵과 커서를 단독 관리하도록 구현했다.
4. 이동·카메라 Controller에서 맵 활성화와 커서 관리 책임을 제거했다.
5. CombatSandbox에 `Game Session` 루트와 입력 게이트를 추가했다.
6. Editor 도구가 해당 구성과 필수 Input Actions를 반복 검증하도록 만들었다.
7. EditMode 정책 테스트와 실제 키보드 입력 PlayMode 테스트를 추가했다.
8. 씬 GUID 불일치와 기존 카메라 회귀 테스트의 비결정성을 수정했다.

## 변경된 자산

| 구분 | 자산 |
|---|---|
| 상태·정책 | `GameSessionState.cs`, `GameplayInputPolicy.cs` |
| Unity 어댑터 | `GameplayInputGate.cs` |
| 기존 소비자 | `PlayerMovementController.cs`, `ThirdPersonCameraController.cs` |
| 씬 | `CombatSandbox.unity` |
| Editor 도구 | `GameplayInputGateSandboxTools.cs` 및 기존 샌드박스 도구 |
| 테스트 | `GameplayInputPolicyTests.cs`, `GameplayInputGatePlayModeTests.cs` |
| 회귀 안정화 | `ThirdPersonCameraPlayModeTests.cs` |

## 검증

| 항목 | 기대 결과 | 실제 결과 | 판정 |
|---|---|---|---|
| 상태 정책 | 네 상태의 맵 조합 일치 | 8개 정책 사례 통과 | 통과 |
| Pause | Escape로 Playing ↔ Paused | 실제 키보드 입력 통과 | 통과 |
| 정지 중 이동 | Paused에서 W 이동 없음 | 수평 변위 0.01m 미만 | 통과 |
| 재개 | Playing 복귀 후 W 이동 | 수평 변위 0.1m 초과 | 통과 |
| 결과 상태 | Victory·Defeat에서 Pause 거부 | 상태 유지 | 통과 |
| 전체 Gameplay 차단 | 모든 Gameplay 액션 비활성 | 전체 비활성 확인 | 통과 |
| EditMode 전체 | 기존 회귀 포함 | 27/27 passed | 통과 |
| PlayMode 전체 | 이동·카메라·가림 포함 | 9/9 passed | 통과 |

## 문제와 해결

### 임시 씬의 스크립트 GUID 불일치

이전 임시 프로젝트가 생성한 `GameplayInputGate`의 GUID를 원본 씬에 옮겼지만, 원본 `.meta`는 다른 GUID를 가지고 있었다. Unity는 컴포넌트를 Missing Script로 읽어 PlayMode에서 게이트를 찾지 못했다. 원본 `.meta`의 GUID를 씬에 사용하고 전용 Editor 검증을 실행해 해결했다.

### 카메라 추적 회귀 테스트 흔들림

기존 테스트는 가림용 벽이 활성화된 상태에서 플레이어 이동 후 0.3초의 카메라 변위를 측정했다. Cinemachine 감쇠와 Deoccluder 상태에 따라 0.08~0.26m로 달라졌다. 가림 샘플을 끄고 추적만 격리한 뒤 0.75초 동안 0.25m 이상 이동하는지 검증하도록 수정했다.

## 사람의 판단

- 채택: 입력 맵 활성화와 커서를 `GameplayInputGate` 하나가 소유
- 채택: 정책 계산은 순수 C#로 분리하고 Unity 어댑터는 적용만 담당
- 채택: Playing ↔ Paused만 Pause 토글 허용
- 수정: 임시 프로젝트의 씬 GUID를 복사하지 않고 원본 `.meta`를 기준으로 직렬화
- 수정: 카메라 회귀 테스트에서 가림과 추적 책임을 분리
- 보류: 실제 승리·패배 UI와 시간 정지 — OpenSpec 7.2~7.5 범위

## 기능 완료 체크

- [x] FR-UI-005와 OpenSpec 2.5를 식별했다.
- [x] 상태별 입력 맵과 커서 규칙을 문서화했다.
- [x] 입력 활성화 단일 소유자를 구현했다.
- [x] Pause·Victory·Defeat 경계 상태를 자동 검증했다.
- [x] EditMode와 PlayMode 전체 회귀가 통과했다.
- [x] 프롬프트·실패 사례·사람의 판단을 기록했다.
- [ ] 결과 UI 연결 — OpenSpec 7.3~7.4에서 수행
- [ ] Windows 빌드 입력 검증 — OpenSpec 8.7에서 수행

## 교육 포인트

- Input Action을 읽는 컴포넌트와 Action Map을 켜고 끄는 소유자를 분리해야 한다.
- 상태 정책을 순수 함수로 분리하면 Unity 씬을 열지 않고도 경계값을 빠르게 검증할 수 있다.
- Unity 씬의 스크립트 참조는 파일명이 아니라 `.meta GUID`로 연결된다.
- 감쇠가 있는 카메라 테스트는 다른 기능의 장애물을 제거하고 책임을 격리해야 안정적이다.

## 연결

- PRD: [[../01_PRD]]
- 입력 차단 계약: [[../11_GAMEPLAY_INPUT_GATING]]
- 프롬프트: [[../PromptLog/2026-07-11_M1_gameplay_input_gating_v01]]
- GUID 오류: [[../Troubleshooting/2026-07-11-input-gate-scene-guid-mismatch]]
- 카메라 회귀 오류: [[../Troubleshooting/2026-07-11-cinemachine-follow-test-flakiness]]
- OpenSpec tasks: [tasks.md](../../openspec/changes/build-action-rpg-vertical-slice/tasks.md)
