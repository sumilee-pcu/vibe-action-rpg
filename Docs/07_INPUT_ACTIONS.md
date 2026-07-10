---
title: 입력 액션 계약
status: active
updated: 2026-07-10
requirements:
  - FR-PLAYER-001
  - FR-PLAYER-002
  - FR-PLAYER-005
  - FR-COMBAT-001
  - FR-COMBAT-006
  - FR-UI-005
tags:
  - unity
  - input-system
  - player-control
---

# 입력 액션 계약

이 문서는 런타임 코드와 UI가 공통으로 사용하는 Input System 액션 이름과 기본 바인딩을 설명한다. 실제 원본은 'Assets/_Project/Input/TinyVanguardInput.inputactions'이며, 이 문서는 교육과 검토를 위한 파생 설명이다.

## MVP 제어 방식

- 입력 장치: 키보드와 마우스
- Control Scheme: 'Keyboard&Mouse'
- 게임패드: 액션 이름을 유지한 채 바인딩만 추가하는 후속 확장
- C# 래퍼 자동 생성: 현재 비활성화

PRD가 키보드·마우스를 MVP 기준으로 두고 있으므로 미결정 사항인 게임패드 지원을 이번 작업에서 암묵적으로 범위에 넣지 않았다.

## Gameplay 맵

| 액션 | 타입 | 기본 바인딩 | 목적 |
|---|---|---|---|
| 'Move' | Value / Vector2 | WASD | 카메라 기준 이동 벡터 |
| 'Look' | Value / Vector2 | Mouse Delta | 3인칭 카메라 회전 입력 |
| 'Attack' | Button | Left Mouse | 기본 공격 |
| 'Dodge' | Button | Space | 회피 시작 |
| 'Skill1' | Button | Q | 첫 번째 액티브 스킬 |
| 'Skill2' | Button | E | 두 번째 액티브 스킬 |

'Skill1'과 'Skill2'를 처음부터 분리한 이유는 PRD의 MVP가 서로 다른 액티브 스킬 2종을 요구하기 때문이다. 실제 스킬 데이터와 쿨다운 상태는 이후 작업에서 연결한다.

## System 맵

| 액션 | 타입 | 기본 바인딩 | 목적 |
|---|---|---|---|
| 'Pause' | Button | Escape | 플레이 중 일시정지, 일시정지 상태 해제 |

'Pause'를 Gameplay에서 분리하면 일시정지 중 Gameplay 맵을 꺼도 해제 입력은 받을 수 있다.

## UI 맵

| 액션 | 타입 | 기본 바인딩 | 목적 |
|---|---|---|---|
| 'Navigate' | PassThrough / Vector2 | WASD, Arrow Keys | 선택 항목 이동 |
| 'Submit' | Button | Enter | 선택 확정 |
| 'Cancel' | Button | Backspace | 하위 UI 취소 |
| 'Point' | PassThrough / Vector2 | Mouse Position | 포인터 위치 |
| 'Click' | PassThrough / Button | Left Mouse | UI 클릭 |
| 'ScrollWheel' | PassThrough / Vector2 | Mouse Scroll | 스크롤 |

Escape는 'Pause' 전용으로 예약했다. 'Cancel'까지 Escape에 중복 바인딩하면 일시정지 메뉴에서 한 입력으로 취소와 일시정지 해제가 함께 실행될 수 있어 MVP에서는 Backspace로 분리했다.

## 세션 상태별 활성화 목표

이 표는 OpenSpec 작업 2.5에서 구현할 입력 게이팅 계약이다.

| 세션 상태 | Gameplay | System | UI |
|---|---:|---:|---:|
| Playing | 켬 | 켬 | 끔 |
| Paused | 끔 | 켬 | 켬 |
| Victory | 끔 | 끔 | 켬 |
| Defeat | 끔 | 끔 | 켬 |

HUD는 값을 표시할 뿐 직접 입력을 받지 않으므로 Playing 상태에서 UI 맵을 항상 켜둘 필요가 없다. 이렇게 해야 WASD가 이동과 숨겨진 UI 탐색을 동시에 실행하지 않는다.

## 변경 안전 규칙

1. 액션 이름과 맵 이름은 런타임 API 계약으로 취급한다.
2. 이름을 바꾸면 'InputActionsAssetTests'와 모든 소비 코드를 같은 커밋에서 갱신한다.
3. 게임패드는 새 맵을 복제하지 않고 기존 액션에 binding group을 추가한다.
4. Gameplay 입력을 끄는 기능은 개별 액션 조건문보다 맵 단위 활성화로 구현한다.
5. 감도와 이동 속도는 바인딩 프로세서에 고정하지 않고 카메라·플레이어 설정 데이터로 둔다.

## 런타임 소비 방식

작업 2.2부터 'PlayerMovementController'가 직렬화된 'TinyVanguardInput' 에셋의 Gameplay 맵을 직접 활성화하고 Move 액션을 읽는다. 현재 게임은 로컬 싱글플레이 한 명만 필요하므로 PlayerInput의 사용자 페어링과 런타임 액션 복제 기능은 사용하지 않는다.

이 선택은 맵 단위 입력 차단을 방해하지 않는다. 작업 2.5에서는 같은 Gameplay 맵의 활성 상태를 세션 상태에 맞춰 중앙에서 제어한다.

## 검증

- JSON 구조와 모든 내부 ID의 중복 여부 확인
- Unity 6000.3.11f1 임시 검증 프로젝트에서 Input Actions 임포트 확인
- 'InputActionsAssetTests' 6개 통과
- 전체 EditMode 테스트 7개 통과

## 연결

- PRD: [[01_PRD]]
- 개발일지: [[DevLog/2026-07-10_M1-input-actions]]
- 프롬프트: [[PromptLog/2026-07-10_M1_input_actions_v01]]
- 플레이어 이동: [[08_PLAYER_MOVEMENT]]
- OpenSpec: [player-control spec](../openspec/changes/build-action-rpg-vertical-slice/specs/player-control/spec.md)
