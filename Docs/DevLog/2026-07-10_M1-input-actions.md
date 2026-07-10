---
title: M1 개발일지 — Input Actions 정의
date: 2026-07-10
milestone: M1
requirements:
  - FR-PLAYER-001
  - FR-PLAYER-002
  - FR-PLAYER-005
  - FR-COMBAT-001
  - FR-COMBAT-006
  - FR-UI-005
status: complete
tags:
  - devlog
  - input-system
  - player-control
---

# M1 개발일지 — Input Actions 정의

## 목표

OpenSpec 작업 2.1에 따라 이동, 시점, 공격, 회피, 액티브 스킬 2종, 일시정지와 UI 입력을 키보드·마우스 MVP 계약으로 정의한다.

## 시작 상태

- 기준 커밋: '54bb2fc' — Initialize Unity action RPG vertical slice
- Universal 3D 템플릿의 Input Actions 에셋이 기본 액션 에셋으로 등록됨
- Player 맵에 Jump, Sprint, Crouch, XR, Touch, Joystick 등 현재 범위 밖의 액션과 바인딩이 포함됨
- UI 맵은 템플릿 기본 상태
- 프로젝트 에디터가 열려 있어 'Temp/UnityLockfile'이 존재함

## 수행 내용

1. 템플릿 액션 에셋의 '.meta' GUID를 보존하면서 프로젝트 전용 경로와 이름으로 이동했다.
2. 'Gameplay', 'System', 'UI' 세 맵으로 책임을 분리했다.
3. Gameplay에 Move, Look, Attack, Dodge, Skill1, Skill2를 정의했다.
4. Pause를 Gameplay와 분리된 System 맵에 정의했다.
5. UI에 Navigate, Submit, Cancel, Point, Click, ScrollWheel을 정의했다.
6. PRD 기준대로 'Keyboard&Mouse' Control Scheme만 유지하고 게임패드는 후속 범위로 남겼다.
7. 액션 이름, 타입, 맵 분리, 필수 바인딩과 Project Settings 기본 참조를 검사하는 EditMode 테스트 6개를 추가했다.
8. 실제 프로젝트 잠금을 건드리지 않기 위해 Assets, Packages, ProjectSettings만 임시 복사해 Unity 임포트와 테스트를 검증했다.

## 변경된 자산

- 입력: 'Assets/_Project/Input/TinyVanguardInput.inputactions'
- 테스트: 'InputActionsAssetTests.cs'
- 테스트 어셈블리: 'TinyVanguard.Tests.EditMode.asmdef'
- 설명: [[07_INPUT_ACTIONS]]
- 오류 기록: [[../Troubleshooting/2026-07-10-unity-run-tests-quit]]

## 검증 결과

| 항목 | 절차 | 기대 결과 | 실제 결과 | 판정 |
|---|---|---|---|---|
| JSON 구문 | jq 파싱 | 유효한 JSON | 성공 | 통과 |
| ID 고유성 | 맵·액션·바인딩 ID 비교 | 중복 0개 | 중복 0개 | 통과 |
| 에셋 임포트 | Unity 6000.3.11f1 임시 프로젝트 | ScriptedImporter 성공 | 성공 | 통과 |
| 입력 계약 테스트 | 'InputActionsAssetTests' | 6개 통과 | 6/6 passed | 통과 |
| 전체 EditMode 회귀 | Unity Test Framework | 기존 테스트 포함 전체 통과 | 7/7 passed | 통과 |
| 컴파일 | Unity 스크립트 컴파일 | C# 오류 0개 | 오류 0개 | 통과 |

Cinemachine 패키지 내부 HDRP 샘플 asmref 경고 3개는 M0에서 확인된 기존 패키지 경고이며 이번 변경으로 새 경고는 추가되지 않았다.

## 실패와 수정

### 테스트 결과 파일이 생성되지 않음

- 첫 명령에 '-runTests'와 '-quit'을 함께 전달했다.
- Unity는 임포트와 컴파일을 마친 직후 테스트 실행 전에 정상 종료했고 결과 XML을 만들지 않았다.
- '-quit'을 제거하고 다시 실행해 최종 입력 테스트 6개와 전체 EditMode 테스트 7개가 통과했다.
- 상세 기록: [[../Troubleshooting/2026-07-10-unity-run-tests-quit]]

## 사람의 판단

- 채택: 세션 상태별로 맵을 켜고 끌 수 있도록 Gameplay, System, UI를 분리했다.
- 채택: PRD의 스킬 2종 요구를 반영해 'Skill1', 'Skill2'를 별도 액션으로 정의했다.
- 수정: 템플릿의 범용 액션을 그대로 재사용하지 않고 이번 게임의 계약으로 축소했다.
- 기각: 게임패드·Touch·XR 바인딩을 미리 포함하는 접근은 MVP 범위가 아니므로 적용하지 않았다.
- 보류: C# 래퍼 자동 생성 여부는 입력 소비 코드가 생기는 작업 2.2에서 결정한다.

## 기능 완료 체크

- [x] 관련 PRD 요구사항과 OpenSpec 작업 2.1을 식별했다.
- [x] 키보드·마우스 MVP와 게임패드 제외 범위를 기록했다.
- [x] 액션 이름·타입·바인딩을 자동 테스트로 검증했다.
- [x] 기존 직렬화 참조를 '.meta' GUID 보존으로 유지했다.
- [x] 새 C# 컴파일 오류와 프로젝트 자체 경고가 없다.
- [x] 원문 프롬프트와 AI 판단을 기록했다.
- [x] 실패한 검증 명령과 수정 결과를 기록했다.
- [ ] PlayMode 수동 조작 — 소비 코드가 없는 정의 단계이므로 작업 2.2에서 적용한다.
- [ ] macOS·Windows 빌드 — 입력 소비 코드 통합 후 M1 완료 시 검증한다.

## 교육 포인트

- Input Action은 키 이름 목록이 아니라 런타임 코드와 UI 사이의 API 계약이다.
- 일시정지 입력을 Gameplay 안에 두면 Gameplay 맵을 끈 뒤 다시 켤 방법이 사라질 수 있다.
- “언젠가 지원할 장치”의 바인딩을 미리 넣으면 테스트 범위와 완료 정의도 함께 커진다.
- Unity CLI 옵션은 성공 종료 코드만 보지 말고 테스트 결과 XML이 실제 생성됐는지 확인해야 한다.

## 연결

- PRD: [[01_PRD]]
- 입력 계약: [[07_INPUT_ACTIONS]]
- 프롬프트: [[../PromptLog/2026-07-10_M1_input_actions_v01]]
- 오류 기록: [[../Troubleshooting/2026-07-10-unity-run-tests-quit]]
- OpenSpec tasks: [tasks.md](../../openspec/changes/build-action-rpg-vertical-slice/tasks.md)
