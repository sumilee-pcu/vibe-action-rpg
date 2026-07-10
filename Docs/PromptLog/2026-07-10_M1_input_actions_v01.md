---
title: 프롬프트 기록 — M1 Input Actions v01
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
  - prompt-log
  - input-system
  - player-control
---

# 프롬프트 기록 — M1 Input Actions v01

## 목표

- OpenSpec의 다음 미완료 작업 2.1을 구현한다.
- 키보드·마우스 MVP 입력 계약을 정의한다.
- 일시정지와 결과 UI에서 게임플레이 입력을 분리할 수 있는 맵 구조를 만든다.
- 구현 판단과 자동 검증을 교육자료로 남긴다.

## 사용자 원문 요청

~~~text
좋아 다음과정으로
~~~

## AI가 해석한 실행 범위

~~~text
build-action-rpg-vertical-slice의 다음 미완료 작업인 2.1을 적용한다.
Move, Look, Attack, Dodge, Skill1, Skill2, Pause와 UI 액션을 정의하고,
PRD의 키보드·마우스 MVP 범위와 세션 상태별 입력 게이팅을 고려해 맵을 분리한다.
Unity 임포트와 EditMode 테스트를 검증한 뒤 프롬프트·개발·오류 기록을 남긴다.
~~~

## 실행 환경

- Unity: 6000.3.11f1
- Input System: 1.19.0
- Render Pipeline: URP 17.3.0
- 대상 입력: Keyboard & Mouse
- 기준 커밋: '54bb2fc'
- OpenSpec change: 'build-action-rpg-vertical-slice'

## AI에게 적용한 제약

- PRD와 OpenSpec을 입력 이름과 완료 조건의 원본으로 사용한다.
- 게임패드는 PRD의 미결정·후속 확장 범위이므로 추가하지 않는다.
- 일시정지 중에도 Pause 입력을 받을 수 있어야 한다.
- 액션 에셋의 기존 '.meta' GUID를 보존한다.
- 열린 Unity 프로젝트를 강제 종료하거나 잠금을 훼손하지 않는다.
- 액션 이름, 타입과 바인딩을 자동 테스트한다.

## AI 결과 요약

- 'Gameplay', 'System', 'UI' 입력 맵
- Gameplay 액션 6개, System 액션 1개, UI 액션 6개
- 키보드·마우스 바인딩 26개
- Input Actions 계약 테스트 6개
- 입력 계약 문서, 개발일지와 CLI 테스트 오류 기록

## 사람의 검토

### 채택

- Gameplay를 통째로 끌 수 있게 Pause와 UI를 별도 맵으로 분리한 구조
- PRD의 액티브 스킬 2종을 'Skill1', 'Skill2'로 명시한 구조
- 기존 기본 Input Actions 참조를 유지하는 '.meta' GUID 보존
- 실제 에디터 잠금 대신 임시 검증 복사본을 사용한 테스트 방식

### 수정

- Unity 템플릿의 Player 맵과 범용 장치 바인딩을 게임 요구사항 중심으로 축소했다.
- 첫 테스트 명령의 '-quit'이 테스트 전에 종료를 유발해 해당 옵션을 제거했다.
- Escape의 Pause와 UI Cancel 중복 실행을 피하기 위해 MVP Cancel을 Backspace로 분리했다.

### 기각

- 아직 결정되지 않은 게임패드, Touch와 XR 바인딩을 유지하는 접근
- 입력 소비 코드가 없는 단계에서 C# 래퍼를 자동 생성하는 접근
- Unity가 열려 있는 실제 프로젝트의 lock 파일을 제거하고 두 번째 에디터를 실행하는 접근

## 검증 결과

- JSON 파싱: 통과
- 내부 ID 고유성: 통과
- Unity Input Actions 임포트: 통과
- 입력 계약 EditMode 테스트: 6/6 통과
- 전체 EditMode 테스트: 7/7 통과
- 새 C# 컴파일 오류: 0개

## 다음 프롬프트에서 개선할 점

- 작업 시작 시 액션 이름, 기본 바인딩과 맵 활성화 표를 먼저 명시한다.
- CLI 테스트에서는 '-quit'을 사용하지 않고 결과 XML 존재 여부를 완료 조건에 포함한다.
- 작업 2.2는 이동 계산과 Unity 입력 어댑터를 분리해 프레임레이트 독립성을 자동 테스트한다.
- 화면에 실제 플레이어가 나타나기 전에는 에셋·코드 계약 테스트와 PlayMode 시각 검증을 구분한다.

## 연결

- PRD: [[01_PRD]]
- 입력 계약: [[07_INPUT_ACTIONS]]
- 개발일지: [[../DevLog/2026-07-10_M1-input-actions]]
- 오류 기록: [[../Troubleshooting/2026-07-10-unity-run-tests-quit]]
- OpenSpec tasks: [tasks.md](../../openspec/changes/build-action-rpg-vertical-slice/tasks.md)
